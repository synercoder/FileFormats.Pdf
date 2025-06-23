using Synercoding.FileFormats.Pdf.Primitives;
using System.Security.Cryptography;

namespace Synercoding.FileFormats.Pdf.Encryption.Internal;

internal class RC4PasswordDecryptor : IDecryptor
{
    private readonly byte[] _decryptionKey;

    public RC4PasswordDecryptor(byte[] decryptionKey)
    {
        _decryptionKey = decryptionKey;
    }

    public PdfString Decrypt(PdfString rawValue, PdfObjectId id)
    {
        var key = _decryptionKey
            .Concat(BitConverter.GetBytes(id.ObjectNumber).Take(3))
            .Concat(BitConverter.GetBytes(id.Generation).Take(2))
            .ToArray();

        var rc4Key = MD5.HashData(key)
            .Take(key.Length)
            .ToArray();

        return new PdfString(RC4.Process(rc4Key, rawValue.Raw), rawValue.IsHex);
    }

    public IPdfStreamObject Decrypt(IPdfStreamObject stream, PdfObjectId id)
    {
        return stream;
    }
}
