using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public class PageTreeNode
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;
    private readonly IPdfLogger _logger;

    // Cache fields
    private int? _count;
    private IReadOnlyList<Either<PageTreeNode, Page>>? _kids;

    internal static PageTreeNode GetRoot(PdfReference pdfReference, ObjectReader objectReader)
    {
        if (!objectReader.TryGet<IPdfDictionary>(pdfReference.Id, out var dictionary))
        {
            objectReader.Settings.Logger.LogWarning<PageTreeNode>("Could not retrieve a dictionary with reference {Id}", pdfReference.Id);
            throw new ParseException($"Could not retrieve a dictionary with reference {pdfReference.Id}");
        }

        return new PageTreeNode(pdfReference.Id, dictionary, objectReader);
    }

    internal PageTreeNode(PdfObjectId id, IPdfDictionary nodeDictionary, ObjectReader objectReader, PageTreeNode? parent = null)
    {
        Id = id;
        Parent = parent;
        _pdfDictionary = nodeDictionary ?? throw new ArgumentNullException(nameof(nodeDictionary));
        _objectReader = objectReader ?? throw new ArgumentNullException(nameof(objectReader));
        _logger = objectReader.Settings.Logger;

        _pdfDictionary.ValidateDictionaryType<PageTreeNode>(id, objectReader, PdfNames.Pages);
    }

    public PdfObjectId Id { get; }
    public PageTreeNode? Parent { get; }

    public IReadOnlyList<Either<PageTreeNode, Page>> Kids
    {
        get
        {
            if (_kids is not null)
                return _kids;

            _kids = _loadKids();

            return _kids;
        }
    }

    /// <summary>
    ///  The number of leaf nodes (page objects) that are descendants of this node within the page tree.
    /// </summary>
    public int Count
    {
        get
        {
            if (_count.HasValue)
                return _count.Value;

            int count = 0;

            foreach (var kid in Kids)
            {
                if (kid.TryGetFirst(out var node))
                    count += node.Count;
                else if (kid.TryGetSecond(out _))
                    count++;
            }

            if (!_pdfDictionary.TryGetValue<PdfNumber>(PdfNames.Count, out var countNumber))
            {
                _logger.LogWarning<PageTreeNode>("Could not retrieve a number for the /Count property in page tree node dictionary {Id}.", Id);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"Could not retrieve a number for the /Count property in page tree node dictionary {Id}.");
            }

            if (countNumber != default && countNumber != count)
            {
                _logger.LogWarning<PageTreeNode>("The /Count property ({RetrievedCount}) is not equal to the actual count ({ActualCount}) in page tree node {Id}.",
                    countNumber.LongValue, count, Id);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"The /Count property ({countNumber.LongValue}) is not equal to the actual count ({count}) in page tree node {Id}.");
            }

            _count = count;

            return count;
        }
    }

    /// <summary>
    /// A rectangle, expressed in default user space units,
    /// that shall define the boundaries of the physical
    /// medium on which the page shall be displayed or printed.
    /// </summary>
    public Rectangle? MediaBox
    {
        get
        {
            Rectangle? mediaBox = null;
            if (_pdfDictionary.TryGetValue<IPdfArray>(PdfNames.MediaBox, _objectReader, out var mediaBoxArray))
            {
                if (!mediaBoxArray.TryGetAsRectangle(_objectReader, out mediaBox))
                {
                    _logger.LogWarning<PageTreeNode>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                        PdfNames.MediaBox, Id);
                    if (_objectReader.Settings.Strict)
                        throw new ParseException($"The {PdfNames.MediaBox} of dictionary {Id} could not be parsed into a rectangle.");
                }
                return mediaBox;
            }

            return null;
        }
    }

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the visible region of default user
    ///  space.When the page is displayed or printed, its
    ///  contents shall be clipped (cropped) to this rectangle.
    /// </summary>
    public Rectangle? CropBox
    {
        get
        {
            Rectangle? cropBox = null;
            if (_pdfDictionary.TryGetValue<IPdfArray>(PdfNames.CropBox, _objectReader, out var cropBoxArray))
            {
                if (!cropBoxArray.TryGetAsRectangle(_objectReader, out cropBox))
                {
                    _logger.LogWarning<PageTreeNode>("The {Key} of dictionary {Id} could not be parsed into a rectangle.",
                        PdfNames.CropBox, Id);
                    if (_objectReader.Settings.Strict)
                        throw new ParseException($"The {PdfNames.CropBox} of dictionary {Id} could not be parsed into a rectangle.");
                }
                return cropBox;
            }

            return null;
        }
    }

    /// <summary>
    ///  A dictionary containing any resources required by the page contents
    /// </summary>
    public IPdfDictionary? Resources
    {
        get
        {
            _ = _pdfDictionary.TryGetValue<IPdfDictionary>(PdfNames.Resources, _objectReader, out var resourcesDictionary);
            return resourcesDictionary;
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

                _logger.LogWarning<PageTreeNode>("While reading the /Rotate property of PdfDictionary {Id}, the returned value was not a multiple of 90 (it was {RotateValue}).",
                    Id,
                    rotateNumber);
            }

            return null;
        }
    }

    private IReadOnlyList<Either<PageTreeNode, Page>> _loadKids()
    {
        if (!_pdfDictionary.TryGetValue<IPdfArray>(PdfNames.Kids, out var kidsArray))
            throw new ParseException($"The retrieved page tree node dictionary (Object id {Id}) does not contain the required key {PdfNames.Kids}");

        var kids = new List<Either<PageTreeNode, Page>>();
        foreach (var kid in kidsArray)
        {
            if (kid is not PdfReference nodeReference)
            {
                _logger.LogWarning<PageTreeNode>("While reading the /Kids array of object {ObjectId}, a non-reference value was encountered: {kid}",
                    Id, kid);
                if (_objectReader.Settings.Strict)
                    throw new ParseException($"While reading the /Kids array of object {Id}, a non-reference value was encountered: {kid}");
                continue;
            }

            if (_tryGet(nodeReference, out var kidResult))
                kids.Add(kidResult);
        }

        return kids;
    }

    private bool _tryGet(PdfReference pdfReference, [NotNullWhen(true)] out Either<PageTreeNode, Page>? result)
    {
        result = null;

        if (!_objectReader.TryGet<IPdfDictionary>(pdfReference.Id, out var dictionary))
        {
            _logger.LogWarning<PageTreeNode>("Could not retrieve a dictionary with reference {Id}", pdfReference.Id);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"Could not retrieve a dictionary with reference {pdfReference.Id}");

            return false;
        }

        if (!dictionary.TryGetValue<PdfName>(PdfNames.Type, _objectReader, out var actualType))
        {
            _logger.LogWarning<PageTreeNode>("The dictionary (id {Id}) does not contain the required key {MissingKey}",
                pdfReference.Id, PdfNames.Type);
            if (_objectReader.Settings.Strict)
                throw new ParseException($"The dictionary (id {pdfReference.Id}) does not contain the required key {PdfNames.Type}");
            return false;
        }

        if (actualType == PdfNames.Pages)
        {
            result = new PageTreeNode(pdfReference.Id, dictionary, _objectReader, this);
            return true;
        }
        if (actualType == PdfNames.Page)
        {
            result = new Page(pdfReference.Id, dictionary, _objectReader, this);
            return true;
        }

        return false;
    }
}
