using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Encryption;

internal sealed class StandardEncryptionDictionary : EncryptDictionary
{
    internal StandardEncryptionDictionary(IPdfPrimitive trailerValue, ObjectReader objectReader)
        : base(trailerValue, objectReader)
    { }

    public int R
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.R, _objectReader, out var rNumber))
                throw new ParseException("No /R value was provided in the Encrypt dictionary with a number value.");

            if (rNumber.IsFractional)
                throw new ParseException("The number for the /R key in the Encrypt dictionary was fractional.");

            return rNumber;
        }
    }

    public byte[] O
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfString>(PdfNames.O, _objectReader, out var oString))
                throw new ParseException("No /O value was provided in the Encrypt dictionary with a byte string value.");

            if (!oString.IsHex && _objectReader.Settings.Strict)
                throw new ParseException("The /O pdf string is not a hex-string.");

            var oBytes = oString.Raw;

            if (R <= 4 && oBytes.Length != 32)
                throw new ParseException($"The /O value is not 32 bytes long while /R = {R}");

            if (R == 6 && oBytes.Length != 48)
                throw new ParseException("The /O value is not 48 bytes long while /R = 6");

            return oString.Raw;
        }
    }

    public byte[] U
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfString>(PdfNames.U, _objectReader, out var oString))
                throw new ParseException("No /U value was provided in the Encrypt dictionary with a byte string value.");

            if (!oString.IsHex && _objectReader.Settings.Strict)
                throw new ParseException("The /U pdf string is not a hex-string.");

            var oBytes = oString.Raw;

            if (R <= 4 && oBytes.Length != 32)
                throw new ParseException($"The /U value is not 32 bytes long while /R = {R}");

            if (R == 6 && oBytes.Length != 48)
                throw new ParseException("The /U value is not 48 bytes long while /R = 6");

            return oString.Raw;
        }
    }

    public byte[]? OE
    {
        get
        {
            if (R != 6)
                return null;

            if (!_dictionary.TryGetValue<PdfString>(PdfNames.OE, _objectReader, out var oeString))
                throw new ParseException("No /OE value was provided in the Encrypt dictionary with a byte string value.");

            if (!oeString.IsHex && _objectReader.Settings.Strict)
                throw new ParseException("The /OE pdf string is not a hex-string.");

            var oeBytes = oeString.Raw;

            if (oeBytes.Length != 32)
                throw new ParseException($"The /OE value is not 32 bytes long.");

            return oeString.Raw;
        }
    }

    public byte[]? UE
    {
        get
        {
            if (R != 6)
                return null;

            if (!_dictionary.TryGetValue<PdfString>(PdfNames.UE, _objectReader, out var ueString))
                throw new ParseException("No /UE value was provided in the Encrypt dictionary with a byte string value.");

            if (!ueString.IsHex && _objectReader.Settings.Strict)
                throw new ParseException("The /UE pdf string is not a hex-string.");

            var ueBytes = ueString.Raw;

            if (ueBytes.Length != 32)
                throw new ParseException($"The /UE value is not 32 bytes long.");

            return ueString.Raw;
        }
    }

    public UserAccessPermissions P
    {
        get
        {
            if (!_dictionary.TryGetValue<PdfNumber>(PdfNames.P, _objectReader, out var pNumber))
                throw new ParseException("No /P value was provided in the Encrypt dictionary with a number value.");

            if (pNumber.IsFractional)
                throw new ParseException("The number for the /P key in the Encrypt dictionary was fractional.");

            return (UserAccessPermissions)pNumber.LongValue;
        }
    }

    public byte[]? Perms
    {
        get
        {
            if (R != 6)
                return null;

            if (!_dictionary.TryGetValue<PdfString>(PdfNames.Perms, _objectReader, out var permsString))
                throw new ParseException("No /Perms value was provided in the Encrypt dictionary with a byte string value.");

            if (!permsString.IsHex && _objectReader.Settings.Strict)
                throw new ParseException("The /Perms pdf string is not a hex-string.");

            var permsBytes = permsString.Raw;

            return permsString.Raw;
        }
    }

    public bool? EncryptMetadata
    {
        get
        {
            if (V != EncryptionAlgorithm.RC4OrAESKey128Bits && V != EncryptionAlgorithm.AES256BitsKey)
                return null;

            if (!_dictionary.TryGetValue<PdfBoolean>(PdfNames.EncryptMetadata, _objectReader, out var encryptMetaBool))
                return true;

            return encryptMetaBool;
        }
    }
}
