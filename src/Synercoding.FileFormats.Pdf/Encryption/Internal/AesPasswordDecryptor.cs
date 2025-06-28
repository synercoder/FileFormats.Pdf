using Synercoding.FileFormats.Pdf.Primitives;
using System.Security.Cryptography;

namespace Synercoding.FileFormats.Pdf.Encryption.Internal;

internal class AesPasswordDecryptor : IDecryptor
{
    private readonly byte[] _decryptionKey;

    public AesPasswordDecryptor(byte[] decryptionKey)
    {
        _decryptionKey = decryptionKey;
    }

    public PdfString Decrypt(PdfString rawValue, PdfObjectId id)
    {
        var decrypted = _decrypt(rawValue.Raw, id);
        return new PdfString(decrypted, rawValue.IsHex);
    }

    public IPdfStreamObject Decrypt(IPdfStreamObject stream, PdfObjectId id)
    {
        var decrypted = _decrypt(stream.RawData, id);
        return new DecryptedStreamWrapper(stream, decrypted);
    }

    private byte[] _decrypt(byte[] input, PdfObjectId id)
    {
        if (input.Length <= 16)
            throw new ArgumentException("Input must be more than 16 bytes (first 16 bytes is the IV, rest the encrypted value).");

        using var aes = Aes.Create();
        aes.Key = _getKey(id);
        var iv = input[..16];
        return aes.DecryptCbc(input[16..], iv, PaddingMode.PKCS7);
    }

    private byte[] _getKey(PdfObjectId id)
    {
        var key = _decryptionKey
            .Concat(BitConverter.GetBytes(id.ObjectNumber).Take(3))
            .Concat(BitConverter.GetBytes(id.Generation).Take(2))
            .Concat(new byte[] { 0x73, 0x41, 0x6C, 0x54 })
            .ToArray();

        var take = Math.Min(16, key.Length);

        return MD5.HashData(key)
            .Take(take)
            .ToArray();
    }
}
