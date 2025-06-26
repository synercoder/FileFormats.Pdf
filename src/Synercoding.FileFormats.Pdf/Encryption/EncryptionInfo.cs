using Synercoding.FileFormats.Pdf.Encryption.Internal;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Synercoding.FileFormats.Pdf.Encryption;

public class EncryptionInfo
{
    private readonly StandardEncryptionDictionary _encryptionDictionary;
    private readonly ObjectReader _objectReader;

    internal EncryptionInfo()
    {
        // This constructor should only be used when no /Encrypt value was found
        // in the trailer, meaning the pdf is not encrypted.
        _encryptionDictionary = null!;
        _objectReader = null!;

        // Set that the pdf is not encrypted
        Level = AccessLevel.NotEncrypted;

        // Set all available permission flags
        Permissions = Enum.GetValues<UserAccessPermissions>()
            .Aggregate((acc, permission) => acc | permission);

    }

    internal EncryptionInfo(StandardEncryptionDictionary encryptionDictionary, ObjectReader objectReader)
    {
        _encryptionDictionary = encryptionDictionary;
        _objectReader = objectReader;
        Level = AccessLevel.Encrypted;
    }

    public UserAccessPermissions Permissions { get; private set; }
    public AccessLevel Level { get; private set; }

    internal IDecryptor? Decryptor { get; private set; }

    [MemberNotNullWhen(true, nameof(Decryptor))]
    public bool Decrypt(string? password)
        => Decrypt(_padOrTruncate(password));

    [MemberNotNullWhen(true, nameof(Decryptor))]
    internal bool Decrypt(byte[] password)
    {
        if (Level == AccessLevel.NotEncrypted)
            throw new InvalidOperationException("Can't decrypt a pdf that is not encrypted.");

        password = _padOrTruncate(password);

        var userKey = _processUserPassword(password, out var encryptionKey);
        if (_validate(userKey))
        {
            Decryptor = new RC4PasswordDecryptor(encryptionKey);
            Level = AccessLevel.UserAccess;
            Permissions = _encryptionDictionary.P;

            return true;
        }

        var possibleEncryptionKey = _computeEncryptionKey(password);
        if (_encryptionDictionary.R == 2)
        {
            userKey = RC4.Process(possibleEncryptionKey, _encryptionDictionary.O);
            if (_validate(userKey))
            {
                Decryptor = new RC4PasswordDecryptor(userKey);
                Level = AccessLevel.UserAccess;
                Permissions = _encryptionDictionary.P;

                return true;
            }
            return false;
        }

        var encryptedUserKey = RC4.Process(possibleEncryptionKey, _encryptionDictionary.O);
        for (int i = 19; i >= 0; i--)
        {
            var decryptionKey = possibleEncryptionKey
                .Select(b => (byte)( b ^ i ))
                .ToArray();
            encryptedUserKey = RC4.Process(decryptionKey, encryptedUserKey);
        }

        userKey = _processUserPassword(encryptedUserKey, out encryptionKey);

        if (_validate(userKey))
        {
            Decryptor = new RC4PasswordDecryptor(encryptionKey);
            Level = AccessLevel.OwnerAccess;
            Permissions = Enum.GetValues<UserAccessPermissions>()
                .Aggregate((result, permission) => result | permission);

            return true;
        }

        return false;

        bool _validate(byte[] input)
        {
            if (input.SequenceEqual(_encryptionDictionary.U))
                return true;

            if (_encryptionDictionary.R >= 3 && input.SequenceEqual(_encryptionDictionary.U.Take(16)))
                return true;

            return false;
        }
    }

    private byte[] _padOrTruncate(string? password)
        => _padOrTruncate(PDFDocEncoding.Encode(password ?? ""));

    private byte[] _padOrTruncate(byte[] password)
    {
        byte[] padString =
        [
            0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
            0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
            0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
            0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A
        ];

        return password
                .Take(32)
                .Concat(padString)
                .Take(32)
                .ToArray();
    }

    private byte[] _computeEncryptionKey(byte[] password)
    {
        var id = _objectReader.Trailer.ID?.OriginalId
            ?? throw new ParseException("The ID value must be present and valid to decrypt the PDF.");

        var hashInput = password
            .Concat(_encryptionDictionary.O)
            .Concat(BitConverter.GetBytes((int)_encryptionDictionary.P))
            .Concat(id);

        if (_encryptionDictionary.R >= 4 && _encryptionDictionary.EncryptMetadata == false)
            hashInput = hashInput
                .Concat(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        var hash = MD5.HashData(hashInput.ToArray());

        var length = ( _encryptionDictionary.Length ?? 40 ) / 8;

        if (_encryptionDictionary.R >= 3)
            for (int i = 0; i < 50; i++)
                hash = MD5.HashData(hash.Take(length).ToArray());

        if (_encryptionDictionary.R == 2)
            return hash.Take(5).ToArray();

        return hash.Take(length).ToArray();
    }

    private byte[] _processUserPassword(byte[] password, out byte[] encryptionKey)
    {
        if (_encryptionDictionary.R == 2)
            return _processUserPasswordRevision2(password, out encryptionKey);

        var id = _objectReader.Trailer.ID?.OriginalId
            ?? throw new ParseException("The ID value must be present and valid to decrypt the PDF.");

        encryptionKey = _computeEncryptionKey(password);

        var hash = MD5.HashData([.. password, .. id]);

        var encrypted = RC4.Process(encryptionKey, hash);
        for (byte i = 1; i <= 19; i++)
        {
            var newKey = encryptionKey
                .Select(b => (byte)( b ^ i ))
                .ToArray();
            encrypted = RC4.Process(newKey, encrypted);
        }

        return encrypted;
    }

    private byte[] _processUserPasswordRevision2(byte[] password, out byte[] encryptionKey)
    {
        encryptionKey = _computeEncryptionKey(password);
        return RC4.Process(encryptionKey, password);
    }
}


public enum AccessLevel
{
    NotEncrypted,
    Encrypted,
    UserAccess,
    OwnerAccess
}
