using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;

namespace Synercoding.FileFormats.Pdf.Encryption;

public class PasswordDecryptor : IDecryptor
{
    private readonly StandardEncryptionDictionary _dictionary;
    private readonly byte[] _encryptionKey;
    private readonly PdfName _defaultStringFilter;
    private readonly PdfName _defaultStreamFilter;

    internal PasswordDecryptor(StandardEncryptionDictionary dictionary, byte[] encryptionKey)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
        
        _defaultStringFilter = _dictionary.GetDefaultStringFilter();
        _defaultStreamFilter = _dictionary.GetDefaultStreamFilter();
    }

    public PdfString Decrypt(PdfString rawValue, PdfObjectId id)
    {
        if (rawValue is null) throw new ArgumentNullException(nameof(rawValue));

        if (_defaultStringFilter == PdfNames.None)
            return rawValue;

        var isAes = _defaultStringFilter == PdfNames.AESV2
            || _defaultStringFilter == PdfNames.AESV3;

        var objectKey = KeyDerivation.ComputeObjectKey(
            _encryptionKey, 
            id.ObjectNumber, 
            id.Generation,
            isAes);

        var decryptedBytes = _decryptData(rawValue.Raw, objectKey, _defaultStringFilter);
        
        return new PdfString(decryptedBytes, rawValue.IsHex);
    }

    public IPdfStreamObject Decrypt(IPdfStreamObject stream, PdfObjectId id)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));

        var streamFilter = _getStreamFilter(stream);

        if (streamFilter == PdfNames.None)
            return stream;

        var isAes = streamFilter == PdfNames.AESV2
            || streamFilter == PdfNames.AESV3;

        var objectKey = KeyDerivation.ComputeObjectKey(
            _encryptionKey,
            id.ObjectNumber,
            id.Generation,
            isAes);

        var decryptedData = _decryptData(stream.RawData, objectKey, streamFilter);
        
        return new ReadOnlyPdfStreamObject(stream.AsPureDictionary(), decryptedData);
    }

    private byte[] _decryptData(byte[] data, byte[] key, PdfName filter)
    {
        return filter switch
        {
            var n when n == PdfNames.V2 => CryptFilter.DecryptWithRC4(data, key),
            var n when n == PdfNames.V4 => CryptFilter.DecryptWithRC4(data, key),
            var n when n == PdfNames.AESV2 => CryptFilter.DecryptWithAES128(data, key),
            var n when n == PdfNames.AESV3 => CryptFilter.DecryptWithAES256(data, key),
            _ => throw new EncryptionException($"Unsupported crypt filter: {filter}")
        };
    }

    private PdfName _getStreamFilter(IPdfStreamObject stream)
    {
        if (stream.TryGetValue(PdfNames.Filter, out var filterValue)
            && filterValue is PdfName filterName
            && _dictionary.CF?.ContainsKey(filterName) == true)
        {
            return filterName;
        }

        return _defaultStreamFilter;
    }
}
