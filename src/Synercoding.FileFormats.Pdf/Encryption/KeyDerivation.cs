using Synercoding.FileFormats.Pdf.Parsing;
using System.Security.Cryptography;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Encryption;

public static class KeyDerivation
{
    private static readonly byte[] _paddingBytes =
    [
        0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
        0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
        0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
        0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A
    ];

    public static byte[] ComputeOwnerKey(string? ownerPassword, string userPassword, int revision, int keyLength)
    {
        if (string.IsNullOrEmpty(ownerPassword))
            ownerPassword = userPassword;
        return ComputeOwnerKey(PadPassword(ownerPassword), PadPassword(userPassword), revision, keyLength);
    }

    public static byte[] ComputeOwnerKey(byte[]? ownerPassword, byte[] userPassword, int revision, int keyLength)
    {
        if (userPassword is null)
            throw new ArgumentNullException(nameof(userPassword));
        if (userPassword.Length != 32)
            throw new ArgumentOutOfRangeException(nameof(userPassword));
        if (ownerPassword is not null && ownerPassword.Length != 32)
            throw new ArgumentOutOfRangeException(nameof(ownerPassword));

        var hash = MD5.HashData(ownerPassword ?? userPassword);

        if (revision >= 3)
            for (int i = 0; i < 50; i++)
                hash = MD5.HashData(hash);

        var key = hash[..keyLength];

        var encrypted = RC4.Process(key, userPassword);

        if (revision >= 3)
        {
            for (int i = 1; i <= 19; i++)
            {
                var tempKey = new byte[key.Length];
                for (int j = 0; j < tempKey.Length; j++)
                {
                    tempKey[j] = (byte)( hash[j] ^ i );
                }
                encrypted = RC4.Process(tempKey, encrypted);
            }
        }

        return encrypted;
    }

    public static byte[] ComputeUserKey(string userPassword, byte[] ownerKey, UserAccessPermissions permissions, byte[] fileId, int revision, int keyLength, bool encryptMetadata = true)
    {
        var encryptionKey = ComputeEncryptionKey(userPassword, ownerKey, permissions, fileId, revision, keyLength, encryptMetadata);

        if (revision == 2)
            return RC4.Process(encryptionKey, PadPassword(userPassword));

        var hash = MD5.HashData([.. _paddingBytes, .. fileId]);

        var encrypted = RC4.Process(encryptionKey, hash);
        for (byte i = 1; i <= 19; i++)
        {
            var newKey = encryptionKey
                .Select(b => (byte)( b ^ i ))
                .ToArray();
            encrypted = RC4.Process(newKey, encrypted);
        }

        return [.. encrypted, .. _paddingBytes[..16]];
    }

    public static byte[] ComputeUserKey(byte[] userKey, byte[] ownerKey, UserAccessPermissions permissions, byte[] fileId, int revision, int keyLength, bool encryptMetadata = true)
    {
        var encryptionKey = ComputeEncryptionKey(userKey, ownerKey, permissions, fileId, revision, keyLength, encryptMetadata);

        if (revision == 2)
            return RC4.Process(encryptionKey, userKey);

        var hash = MD5.HashData([.. _paddingBytes, .. fileId]);

        var encrypted = RC4.Process(encryptionKey, hash);
        for (byte i = 1; i <= 19; i++)
        {
            var newKey = encryptionKey
                .Select(b => (byte)( b ^ i ))
                .ToArray();
            encrypted = RC4.Process(newKey, encrypted);
        }

        return [.. encrypted, .. _paddingBytes[..16]];
    }

    public static byte[] ComputeEncryptionKey(string password, byte[] ownerKey, UserAccessPermissions permissions, byte[] fileId, int revision, int keyLength, bool encryptMetadata = true)
    {
        var paddedPassword = PadPassword(password);

        return ComputeEncryptionKey(paddedPassword, ownerKey, permissions, fileId, revision, keyLength, encryptMetadata);
    }

    public static byte[] ComputeEncryptionKey(byte[] userKey, byte[] ownerKey, UserAccessPermissions permissions, byte[] fileId, int revision, int keyLength, bool encryptMetadata = true)
    {
        var permissionsBytes = BitConverter.GetBytes((int)permissions);
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(permissionsBytes);

        // Concatenate all data first, then hash in one operation per PDF spec Algorithm 3
        byte[] hashInput = [.. userKey, .. ownerKey, .. permissionsBytes, .. fileId];

        if (revision >= 4 && !encryptMetadata)
            hashInput = [.. hashInput, 0xFF, 0xFF, 0xFF, 0xFF];

        var hash = MD5.HashData(hashInput);

        if (revision == 2)
            return hash[..5];

        if (revision >= 3)
            for (int i = 0; i < 50; i++)
                hash = MD5.HashData(hash[..keyLength]);

        return hash[..keyLength];
    }

    public static byte[] ComputeObjectKey(byte[] encryptionKey, int objectNumber, int generation, bool useAes = false)
    {
        // For AES-256 (32-byte keys), use the full encryption key
        if (encryptionKey.Length == 32)
            return encryptionKey;

        var objectBytes = BitConverter.GetBytes(objectNumber);
        var generationBytes = BitConverter.GetBytes(generation);

        byte[] combined = [.. encryptionKey, .. objectBytes[..3], .. generationBytes[..2]];

        if (useAes)
            combined = [.. combined, 0x73, 0x41, 0x6C, 0x54];

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(combined);

        var keyLength = Math.Min(encryptionKey.Length + 5, 16);
        return hash[..keyLength];
    }

    public static byte[] ComputeUserPasswordHashR6(string password, byte[] salt, byte[]? userKeyValidation = null)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] input = [.. passwordBytes, .. salt];

        if (userKeyValidation != null)
            input = [.. input, .. userKeyValidation];

        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(input);
    }

    public static byte[] ComputeOwnerPasswordHashR6(string password, byte[] salt, byte[] userKeyValidation)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] input = [.. passwordBytes, .. salt, .. userKeyValidation];

        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(input);
    }

    public static byte[] ComputeUserEncryptionKeyR6(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] input = [.. passwordBytes, .. salt];

        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(input);
    }

    public static byte[] ComputeOwnerEncryptionKeyR6(string password, byte[] salt, byte[] userKeyValidation)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] input = [.. passwordBytes, .. salt, .. userKeyValidation];

        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(input);
    }

    internal static byte[] PadPassword(string password)
    {
        _ = password ?? throw new ArgumentNullException(nameof(password));

        var result = new byte[32];
        var passwordBytes = PDFDocEncoding.Encode(password ?? string.Empty);

        var copyLength = Math.Min(passwordBytes.Length, 32);
        Array.Copy(passwordBytes, result, copyLength);

        if (copyLength < 32)
        {
            Array.Copy(_paddingBytes, 0, result, copyLength, 32 - copyLength);
        }

        return result;
    }
}
