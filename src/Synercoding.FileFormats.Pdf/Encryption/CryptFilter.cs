using Synercoding.FileFormats.Pdf.Exceptions;
using System.Security.Cryptography;

namespace Synercoding.FileFormats.Pdf.Encryption;

public static class CryptFilter
{
    public static byte[] DecryptWithRC4(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));

        return RC4.Process(key, data);
    }

    public static byte[] DecryptWithAES128(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 16) throw new EncryptionException($"AES-128 requires a 16-byte key, but got {key.Length} bytes.");

        return _decryptWithAES(data, key);
    }

    public static byte[] DecryptWithAES256(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 32) throw new EncryptionException($"AES-256 requires a 32-byte key, but got {key.Length} bytes.");

        return _decryptWithAES(data, key);
    }

    public static byte[] EncryptWithRC4(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));

        return RC4.Process(key, data);
    }

    public static byte[] EncryptWithAES128(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 16) throw new EncryptionException($"AES-128 requires a 16-byte key, but got {key.Length} bytes.");

        return _encryptWithAES(data, key);
    }

    public static byte[] EncryptWithAES256(byte[] data, byte[] key)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 32) throw new EncryptionException($"AES-256 requires a 32-byte key, but got {key.Length} bytes.");

        return _encryptWithAES(data, key);
    }

    private static byte[] _encryptWithAES(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.GenerateIV();

        return [.. aes.IV, .. aes.EncryptCbc(data, aes.IV, PaddingMode.PKCS7)];
    }

    private static byte[] _decryptWithAES(byte[] data, byte[] key)
    {
        if (data.Length < 16) throw new EncryptionException("AES encrypted data must be at least 16 bytes (IV size).");

        var iv = data[..16];

        var encryptedData = data[16..];

        using var aes = Aes.Create();
        aes.Key = key;

        return aes.DecryptCbc(encryptedData, iv);
    }
}
