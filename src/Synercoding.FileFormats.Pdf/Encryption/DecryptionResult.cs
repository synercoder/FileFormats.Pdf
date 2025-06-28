using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Encryption;

public class DecryptionResult
{
    private readonly StandardEncryptionDictionary? _encryptionDictionary;

    private DecryptionResult(
        StandardEncryptionDictionary? encryptionDictionary,
        AccessLevel accessLevel,
        byte[] key,
        int? keyLength = null)
    {
        _encryptionDictionary = encryptionDictionary;
        EncryptionKey = key;
        KeyLength = keyLength ?? key.Length * 8;
        AccessLevel = accessLevel;

        if (encryptionDictionary != null)
        {
            TextEncryptionMethod = encryptionDictionary.GetDefaultStringFilter() switch
            {
                var p when p == PdfNames.None => EncryptionMethod.None,
                var p when p == PdfNames.V2 => EncryptionMethod.RC4,
                var p when p == PdfNames.V4 => EncryptionMethod.RC4,
                var p when p == PdfNames.AESV2 => EncryptionMethod.AES,
                var p when p == PdfNames.AESV3 => EncryptionMethod.AES,
                _ => throw new EncryptionException("Unknown encryption filter.")
            };
            StreamEncryptionMethod = encryptionDictionary.GetDefaultStringFilter() switch
            {
                var p when p == PdfNames.None => EncryptionMethod.None,
                var p when p == PdfNames.V2 => EncryptionMethod.RC4,
                var p when p == PdfNames.V4 => EncryptionMethod.RC4,
                var p when p == PdfNames.AESV2 => EncryptionMethod.AES,
                var p when p == PdfNames.AESV3 => EncryptionMethod.AES,
                _ => throw new EncryptionException("Unknown encryption filter.")
            };
        }
    }

    public AccessLevel AccessLevel { get; }
    internal byte[] EncryptionKey { get; }
    public int KeyLength { get; }
    public UserAccessPermissions Permissions
        => _encryptionDictionary is not null
        ? _encryptionDictionary.P
        : (UserAccessPermissions)0b1111_1111_1100;

    public EncryptionMethod TextEncryptionMethod { get; }
    public EncryptionMethod StreamEncryptionMethod { get; }

    public IDecryptor GetDecryptor()
    {
        if (AccessLevel == AccessLevel.NotEncrypted || _encryptionDictionary is null)
            throw new EncryptionException("Can't get a decryptor if the pdf is not encrypted.");
        if (AccessLevel == AccessLevel.Encrypted)
            throw new EncryptionException("Can't get a decryptor because a valid decrypting key was not provided.");

        return new PasswordDecryptor(_encryptionDictionary, EncryptionKey);
    }

    internal DecryptionResult AsOwner()
        => Success(_encryptionDictionary!, AccessLevel.OwnerAccess, EncryptionKey);

    internal static DecryptionResult Success(
        StandardEncryptionDictionary encryptionDictionary,
        AccessLevel accessLevel,
        byte[] key)
        => new DecryptionResult(encryptionDictionary, accessLevel, key);

    internal static DecryptionResult Fail(StandardEncryptionDictionary standardEncryptionDictionary, int keyLength)
        => new DecryptionResult(standardEncryptionDictionary, AccessLevel.Encrypted, Array.Empty<byte>(), keyLength);

    internal static DecryptionResult NotEncrypted()
        => new DecryptionResult(null!, AccessLevel.NotEncrypted, Array.Empty<byte>());
}
