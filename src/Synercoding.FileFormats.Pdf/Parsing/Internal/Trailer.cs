using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class Trailer
{
    private readonly IPdfDictionary _dictionary;
    private readonly ReaderSettings _readerSettings;

    public Trailer(IPdfDictionary dictionary, ReaderSettings readerSettings)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _readerSettings = readerSettings ?? throw new ArgumentNullException(nameof(readerSettings));
    }

    public int Size
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.Size, out var size) || size.Value < 0)
                throw new ParseException("Trailer did not contain a /Size key with a zero or higher integer value.");

            return size;
        }
    }

    public long? Prev
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.Prev, out var prev))
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
                return null;

            return encrypt;
        }
    }

    public PdfReference? Info
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfReference>(PdfNames.Info, out var info))
                return null;

            return info;
        }
    }

    public (byte[] OriginalId, byte[] LastVersionId)? ID
    {
        get
        {
            if (!_dictionary.TryGetValue<IPdfArray>(PdfNames.ID, out var idArray))
                return null;

            var throwException = Encrypt is not null || _readerSettings.Strict;

            if (idArray.Count != 2)
            {
                _readerSettings.Logger.LogWarning<Trailer>("An ID array should contain 2 values, found array contains {Count} values.", idArray.Count);
                if (throwException)
                    throw new ParseException($"An ID array should contain 2 values, found array contains {idArray.Count} values.");
            }

            if (!idArray.TryGetValue<PdfString>(0, out var id1) || !id1.IsHex)
            {
                _readerSettings.Logger.LogWarning<Trailer>("An ID array should contain 2 byte strings, first array item was not a byte string.");
                if (throwException)
                    throw new ParseException("An ID array should contain 2 byte string, first array item was not a byte string.");

                return null;
            }

            if (!idArray.TryGetValue<PdfString>(1, out var id2) || !id2.IsHex)
            {
                _readerSettings.Logger.LogWarning<Trailer>("An ID array should contain 2 byte strings, second array item was not a byte string.");
                if (throwException)
                    throw new ParseException("An ID array should contain 2 byte string, second array item was not a byte string.");

                return null;
            }

            return (id1.Raw, id2.Raw);
        }
    }

    public long? XRefStm
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.XRefStm, out var xrefStm))
                return null;

            return xrefStm.LongValue;
        }
    }
}
