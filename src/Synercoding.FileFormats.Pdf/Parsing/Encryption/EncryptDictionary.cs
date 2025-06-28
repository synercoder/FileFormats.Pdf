using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Encryption;

internal abstract class EncryptDictionary
{
    protected readonly IPdfDictionary _dictionary;
    protected readonly ObjectReader _objectReader;

    internal EncryptDictionary(IPdfPrimitive trailerValue, ObjectReader objectReader)
    {
        if (trailerValue is IPdfDictionary encryptDictionary)
            _dictionary = encryptDictionary;
        else if (trailerValue is PdfReference encryptRef)
            if (objectReader.TryGet<IPdfDictionary>(encryptRef.Id, out var indirectDictionary))
                _dictionary = indirectDictionary;
            else
                throw new ArgumentException("The /Encrypt reference did not point to a dictionary.", nameof(trailerValue));
        else
            throw new ArgumentException("The /Encrypt value was not a reference or dictionary.", nameof(trailerValue));

        _objectReader = objectReader;
    }

    public PdfName Filter
    {
        get
        {
            if (_dictionary.TryGetValue<PdfName>(PdfNames.Filter, _objectReader, out var filter))
                return filter;

            throw new ParseException("Could not determine the security handler for this document. No /Filter value was provided in the Encrypt dictionary.");
        }
    }

    public PdfName? SubFilter
    {
        get
        {
            if (_dictionary.TryGetValue<PdfName>(PdfNames.SubFilter, _objectReader, out var filter))
                return filter;

            return null;
        }
    }

    public int V
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.V, _objectReader, out var vNumber))
                throw new ParseException("Could not determine the encryption algorithm for this document. No /V value was provided in the Encrypt dictionary with a number value.");

            if (vNumber.IsFractional)
                throw new ParseException("The number for the /V key in the Encrypt dictionary was fractional.");

            return (int)vNumber.LongValue;
        }
    }

    public int? Length
    {
        get
        {
            if (V != 2 && V != 3)
                return null;

            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.Length, _objectReader, out var lengthNumber))
                return 40;

            if (lengthNumber.IsFractional)
                throw new ParseException("The number for the /Length key in the Encrypt dictionary was fractional.");

            if (lengthNumber.LongValue % 8 != 0)
                throw new ParseException("The key length should be a multiple of 8.");

            if (lengthNumber < 40 || lengthNumber > 128)
                throw new ParseException("The key length should be in range 40 to 128.");

            return lengthNumber;
        }
    }

    public IDictionary<PdfName, IPdfDictionary>? CF
    {
        get
        {
            if (V != 4 && V != 5)
                return null;

            if (!_dictionary.TryGetValue<IPdfDictionary>(PdfNames.CF, _objectReader, out var cfDictionary))
                return null;

            var cf = new Dictionary<PdfName, IPdfDictionary>();
            foreach (var (key, value) in cfDictionary)
                if (value is IPdfDictionary cryptFilterDictionary)
                    cf.Add(key, cryptFilterDictionary);

            return cf.AsReadOnly();
        }
    }

    public PdfName? StmF
    {
        get
        {
            if (V != 4 && V != 5)
                return null;

            if (_dictionary.TryGetValue<PdfName>(PdfNames.StmF, _objectReader, out var stmfName))
                return stmfName;
            return PdfNames.Identity;
        }
    }

    public PdfName? StrF
    {
        get
        {
            if (V != 4 && V != 5)
                return null;

            if (_dictionary.TryGetValue<PdfName>(PdfNames.StrF, _objectReader, out var strfName))
                return strfName;
            return PdfNames.Identity;
        }
    }

    public PdfName? EFF
    {
        get
        {
            if (V != 4 && V != 5)
                return null;

            if (_dictionary.TryGetValue<PdfName>(PdfNames.EFF, _objectReader, out var effName))
                return effName;

            return StmF;
        }
    }
}
