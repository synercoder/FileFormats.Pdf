using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using Synercoding.FileFormats.Pdf.Primitives.Internal;

namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public class Page
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;
    private readonly IPdfLogger _logger;

    internal Page(PdfObjectId id, IPdfDictionary nodeDictionary, ObjectReader objectReader, PageTreeNode? parent = null)
    {
        Id = id;
        _pdfDictionary = nodeDictionary;
        _objectReader = objectReader;
        _logger = objectReader.Settings.Logger;

        _pdfDictionary.ValidateDictionaryType<Page>(id, objectReader, PdfNames.Page);
    }

    public PdfObjectId Id { get; }
    public PageTreeNode? Parent { get; private set; }

    /// <summary>
    /// A rectangle, expressed in default user space units,
    /// that shall define the boundaries of the physical
    /// medium on which the page shall be displayed or printed.
    /// </summary>
    public Rectangle MediaBox
    {
        get
        {
            if (_pdfDictionary.TryGetValue<IPdfArray>(PdfNames.MediaBox, _objectReader, out var mediaBoxArray))
            {
                if (mediaBoxArray.TryGetAsRectangle(_objectReader, out var mediaBox))
                    return mediaBox.Value;

                _logger.LogWarning<Page>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                    PdfNames.MediaBox, Id);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"The {PdfNames.MediaBox} of dictionary {Id} could not be parsed into a rectangle.");
            }

            var parentNode = Parent;
            while (parentNode != null)
            {
                if (parentNode.MediaBox.HasValue)
                    return parentNode.MediaBox.Value;

                parentNode = parentNode.Parent;
            }

            _logger.LogWarning<Page>("Page {Id} or it's parents did not have a required {RequiredKey}.",
                Id, PdfNames.MediaBox);
            throw new ParseException($"Page {Id} or it's parents did not have a required {PdfNames.MediaBox} value.");
        }
    }

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the visible region of default user
    ///  space.When the page is displayed or printed, its
    ///  contents shall be clipped (cropped) to this rectangle.
    /// </summary>
    public Rectangle? CropBox
        => _readRectangleFromPageOrParents(PdfNames.CropBox, parent => parent.CropBox);

    /// <summary>
    /// A rectangle, expressed in default user space units,
    /// that shall define the region to which the contents
    /// of the page shall be clipped when output in a production
    /// environment.
    /// </summary>
    public Rectangle? BleedBox
        => _readRectangleFromPageOrParents(PdfNames.BleedBox);

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the intended dimensions of the
    ///  finished page after trimming.
    /// </summary>
    public Rectangle? TrimBox
        => _readRectangleFromPageOrParents(PdfNames.TrimBox);

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the extent of the page’s meaningful
    ///  content(including potential white-space) as intended
    ///  by the page’s creator.
    /// </summary>
    public Rectangle? ArtBox
        => _readRectangleFromPageOrParents(PdfNames.ArtBox);

    private Rectangle? _readRectangleFromPageOrParents(PdfName key, Func<PageTreeNode, Rectangle?>? fromParent = null)
    {
        if (_pdfDictionary.TryGetValue<IPdfArray>(key, _objectReader, out var rectangleArray))
        {
            if (rectangleArray.TryGetAsRectangle(_objectReader, out var rectangle))
                return rectangle;

            _logger.LogWarning<Page>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                key, Id);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"The {key} of dictionary {Id} could not be parsed into a rectangle.");
        }

        if (fromParent is null)
            return null;

        var parentNode = Parent;
        while (parentNode != null)
        {
            if (fromParent(parentNode) is Rectangle rect)
                return rect;

            parentNode = parentNode.Parent;
        }

        return null;
    }

    /// <summary>
    ///  A dictionary containing any resources required by the page contents
    /// </summary>
    public IPdfDictionary Resources
    {
        get
        {
            if (_pdfDictionary.TryGetValue<IPdfDictionary>(PdfNames.Resources, _objectReader, out var resourcesDictionary))
                return resourcesDictionary;

            var parentNode = Parent;
            while (parentNode != null)
            {
                if (parentNode.Resources != null)
                    return parentNode.Resources;

                parentNode = parentNode.Parent;
            }

            _logger.LogWarning<Page>("Page {Id} or it's parents did not have a required {RequiredKey}.",
                Id, PdfNames.Resources);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"Page {Id} or it's parents did not have a required {PdfNames.Resources} dictionary.");

            return ReadOnlyPdfDictionary.Empty;
        }
    }

    /// <summary>
    ///  The number of degrees by which the page shall
    ///  be rotated clockwise when displayed or printed.
    ///  The value shall be a multiple of 90.
    /// </summary>
    public int? Rotate
    {
        get
        {
            if (_pdfDictionary.TryGetValue<PdfNumber>(PdfNames.Rotate, out var rotateNumber))
            {
                if (rotateNumber.LongValue % 90 == 0)
                {
                    return rotateNumber;
                }

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
}
