using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class Trailer
{
    private readonly IPdfDictionary _dictionary;

    public Trailer(IPdfDictionary dictionary)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
    }

    public int Size
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfInteger>(PdfNames.Size, out var size) || size.Value < 0)
                throw new ParseException("Trailer did not contain a /Size key with a zero or higher integer value.");

            return size;
        }
    }

    public long? Prev
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfInteger>(PdfNames.Prev, out var prev))
                return null;

            return prev;
        }
    }

    public PdfReference Root
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfReference>(PdfNames.Root, out var root))
                throw new ParseException("Trailer did not contain a /Root key with a reference value.");

            return root;
        }
    }

    public IPdfPrimitive? Encrypt
    {
        get
        {
            if (!_dictionary.TryGetValue(PdfNames.Encrypt, out var encrypt))
                throw new ParseException("Trailer did not contain a /encrypt key.");

            return encrypt;
        }
    }

    public PdfReference? Info
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfReference>(PdfNames.Info, out var info))
                throw new ParseException("Trailer did not contain a /Info key with a reference value.");

            return info;
        }
    }

    public PdfArray? ID
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfArray>(PdfNames.ID, out var idArray))
                return null;

            if (idArray.Count != 2)
                throw new ParseException($"An ID array should contain 2 values, found array contains {idArray.Count} values.");

            return idArray;
        }
    }

    public long? XRefStm
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfInteger>(PdfNames.XRefStm, out var xrefStm))
                return null;

            return xrefStm.Value;
        }
    }
}
