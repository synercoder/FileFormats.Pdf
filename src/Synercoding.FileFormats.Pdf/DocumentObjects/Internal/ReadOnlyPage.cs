using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using Synercoding.FileFormats.Pdf.Primitives.Internal;

namespace Synercoding.FileFormats.Pdf.DocumentObjects.Internal;

internal class ReadOnlyPage : IReadOnlyPage
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;
    private readonly IPdfLogger _logger;

    internal ReadOnlyPage(PdfObjectId id, IPdfDictionary nodeDictionary, ObjectReader objectReader, PageTreeNode? parent = null)
    {
        Id = id;
        _pdfDictionary = nodeDictionary;
        _objectReader = objectReader;
        _logger = objectReader.Settings.Logger;

        _pdfDictionary.ValidateDictionaryType<ReadOnlyPage>(id, objectReader, PdfNames.Page);
    }

    public PdfObjectId Id { get; }
    public PageTreeNode? Parent { get; private set; }

    public Rectangle MediaBox
    {
        get
        {
            if (_pdfDictionary.TryGetValue<IPdfArray>(PdfNames.MediaBox, _objectReader, out var mediaBoxArray))
            {
                if (mediaBoxArray.TryGetAsRectangle(_objectReader, UserUnit, out var mediaBox))
                    return mediaBox.Value;

                _logger.LogWarning<ReadOnlyPage>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                    PdfNames.MediaBox, Id);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"The {PdfNames.MediaBox} of dictionary {Id} could not be parsed into a rectangle.");
            }

            var parentNode = Parent;
            while (parentNode != null)
            {
                if (parentNode.MediaBox is Rectangle parentRectangle)
                {
                    if (UserUnit is null or 1)
                        return parentRectangle;

                    return new Rectangle(
                        llx: parentRectangle.LLX.Raw,
                        lly: parentRectangle.LLY.Raw,
                        urx: parentRectangle.URX.Raw,
                        ury: parentRectangle.URY.Raw,
                        unit: Unit.Pixels(UserUnit.Value / 72)
                    );
                }

                parentNode = parentNode.Parent;
            }

            _logger.LogWarning<ReadOnlyPage>("Page {Id} or it's parents did not have a required {RequiredKey}.",
                Id, PdfNames.MediaBox);
            throw new ParseException($"Page {Id} or it's parents did not have a required {PdfNames.MediaBox} value.");
        }
    }

    public Rectangle? CropBox
        => _readRectangleFromPageOrParents(PdfNames.CropBox, parent => parent.CropBox);

    public Rectangle? BleedBox
        => _readRectangleFromPageOrParents(PdfNames.BleedBox);

    public Rectangle? TrimBox
        => _readRectangleFromPageOrParents(PdfNames.TrimBox);

    public Rectangle? ArtBox
        => _readRectangleFromPageOrParents(PdfNames.ArtBox);

    public IReadOnlyResources Resources
    {
        get
        {
            if (_pdfDictionary.TryGetValue<PdfReference>(PdfNames.Resources, out var resourceReference)
                && _objectReader.TryGet<IPdfDictionary>(resourceReference.Id, out var indirectResourcesDictionary))
                return new ReadOnlyResources(resourceReference.Id, indirectResourcesDictionary, _objectReader);
            else if (_pdfDictionary.TryGetValue<IPdfDictionary>(PdfNames.Resources, out var directDictionary))
                return new ReadOnlyResources(null, directDictionary, _objectReader);

            var parentNode = Parent;
            while (parentNode != null)
            {
                if (parentNode.Resources != null)
                    return parentNode.Resources;

                parentNode = parentNode.Parent;
            }

            _logger.LogWarning<ReadOnlyPage>("Page {Id} or it's parents did not have a required {RequiredKey}.",
                Id, PdfNames.Resources);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"Page {Id} or it's parents did not have a required {PdfNames.Resources} dictionary.");

            return new ReadOnlyResources(null, ReadOnlyPdfDictionary.Empty, _objectReader);
        }
    }

    public int? Rotate
    {
        get
        {
            if (_pdfDictionary.TryGetValue<PdfNumber>(PdfNames.Rotate, out var rotateNumber))
            {
                if (rotateNumber.LongValue % 90 == 0)
                    return rotateNumber;

                _logger.LogWarning<PageTreeNode>("While reading the /Rotate key of PdfDictionary {Id}, the returned value was not a multiple of 90 (it was {RotateValue}).",
                    Id,
                    rotateNumber);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"While reading the /Rotate key of PdfDictionary {Id}, the returned value was not a multiple of 90 (it was {rotateNumber}).");
            }

            var parentNode = Parent;
            while (parentNode != null)
            {
                if (parentNode.Rotate.HasValue)
                    return parentNode.Rotate.Value;

                parentNode = parentNode.Parent;
            }

            return null;
        }
    }

    public double? UserUnit
    {
        get
        {
            if (!_pdfDictionary.TryGetValue<PdfNumber>(PdfNames.UserUnit, out var userUnit))
                return null;

            return userUnit.Value;
        }
    }

    private Rectangle? _readRectangleFromPageOrParents(PdfName key, Func<PageTreeNode, Rectangle?>? fromParent = null)
    {
        if (_pdfDictionary.TryGetValue<IPdfArray>(key, _objectReader, out var rectangleArray))
        {
            if (rectangleArray.TryGetAsRectangle(_objectReader, UserUnit, out var rectangle))
                return rectangle;

            _logger.LogWarning<ReadOnlyPage>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                key, Id);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"The {key} of dictionary {Id} could not be parsed into a rectangle.");
        }

        if (fromParent is null)
            return null;

        var parentNode = Parent;
        while (parentNode != null)
        {
            if (fromParent(parentNode) is Rectangle parentRectangle)
            {
                if (UserUnit is null or 1)
                    return parentRectangle;

                return new Rectangle(
                    llx: parentRectangle.LLX.Raw,
                    lly: parentRectangle.LLY.Raw,
                    urx: parentRectangle.URX.Raw,
                    ury: parentRectangle.URY.Raw,
                    unit: Unit.Pixels(UserUnit.Value / 72)
                );
            }

            parentNode = parentNode.Parent;
        }

        return null;
    }
}
