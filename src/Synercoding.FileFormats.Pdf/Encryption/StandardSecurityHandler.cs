using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using System.Security.Cryptography;

namespace Synercoding.FileFormats.Pdf.Encryption;

internal class StandardSecurityHandler
{
    private readonly StandardEncryptionDictionary _dictionary;
    private readonly byte[] _fileId;

    public StandardSecurityHandler(StandardEncryptionDictionary dictionary, byte[] fileId)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _fileId = fileId ?? throw new ArgumentNullException(nameof(fileId));
    }

    public DecryptionResult Authenticate(string? password)
    {
        var result = AuthenticateUserPassword(password);
        if (result.AccessLevel == AccessLevel.UserAccess)
            return result;

        return AuthenticateOwnerPassword(password);
    }

    public DecryptionResult AuthenticateUserPassword(string? password)
    {
        password ??= string.Empty;

        if (_dictionary.R == 6)
            return _authenticateUserPasswordR6(password);

        return _authenticateUserPasswordR2R5(KeyDerivation.PadPassword(password));
    }

    public DecryptionResult AuthenticateOwnerPassword(string? password)
    {
        password ??= string.Empty;

        if (_dictionary.R == 6)
            return _authenticateOwnerPasswordR6(password);

        return _authenticateOwnerPasswordR2R5(KeyDerivation.PadPassword(password));
    }

    private DecryptionResult _authenticateUserPasswordR2R5(byte[] password)
    {
        var keyLength = _getKeyLength();
        var computedUserKey = KeyDerivation.ComputeUserKey(
            password,
            _dictionary.O,
            _dictionary.P,
            _fileId,
            _dictionary.R,
            keyLength,
            _dictionary.EncryptMetadata ?? true);

        var isValid = _dictionary.R >= 3
            ? _arraysEqual(_dictionary.U[..16], computedUserKey[..16])
            : _arraysEqual(_dictionary.U, computedUserKey);

        if (!isValid)
            return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);

        var encryptionKey = KeyDerivation.ComputeEncryptionKey(
            password,
            _dictionary.O,
            _dictionary.P,
            _fileId,
            _dictionary.R,
            keyLength,
            _dictionary.EncryptMetadata ?? true);

        return DecryptionResult.Success(_dictionary, AccessLevel.UserAccess, encryptionKey);
    }

    private DecryptionResult _authenticateOwnerPasswordR2R5(byte[] password)
    {
        // Step 1: Compute the key from the owner password
        var hash = MD5.HashData(password);

        if (_dictionary.R >= 3)
            for (int i = 0; i < 50; i++)
                hash = MD5.HashData(hash);

        var keyLength = _getKeyLength();

        var key = hash[..keyLength];

        // Step 2: Decrypt the O entry to recover the user password
        var userPassword = Array.Empty<byte>();
        if (_dictionary.R == 2)
        {
            userPassword = RC4.Process(key, _dictionary.O);
        }
        else if (_dictionary.R >= 3)
        {
            userPassword = _dictionary.O;
            for (int i = 19; i >= 0; i--)
            {
                var tempKey = new byte[key.Length];
                for (int j = 0; j < tempKey.Length; j++)
                {
                    tempKey[j] = (byte)( hash[j] ^ i );
                }
                userPassword = RC4.Process(tempKey, userPassword);
            }
        }

        // Step 4: Test if the recovered user password is valid
        if (_authenticateUserPasswordR2R5(userPassword) is DecryptionResult userResult
            && userResult.AccessLevel == AccessLevel.UserAccess)
            return userResult.AsOwner();

        return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);
    }

    private DecryptionResult _authenticateUserPasswordR6(string password)
    {
        if (_dictionary.U == null || _dictionary.U.Length < 48)
            return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);

        var salt = new byte[8];
        Array.Copy(_dictionary.U, 32, salt, 0, 8);

        var hash = KeyDerivation.ComputeUserPasswordHashR6(password, salt);
        var validationHash = new byte[32];
        Array.Copy(_dictionary.U, 0, validationHash, 0, 32);

        var isValid = _arraysEqual(hash, validationHash);

        if (!isValid)
            return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);

        var keySalt = new byte[8];
        Array.Copy(_dictionary.U, 40, keySalt, 0, 8);
        var encryptionKey = KeyDerivation.ComputeUserEncryptionKeyR6(password, keySalt);

        return DecryptionResult.Success(_dictionary, AccessLevel.UserAccess, encryptionKey);
    }

    private DecryptionResult _authenticateOwnerPasswordR6(string password)
    {
        if (_dictionary.O == null || _dictionary.O.Length < 48 || _dictionary.U == null || _dictionary.U.Length < 48)
            return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);

        var salt = new byte[8];
        Array.Copy(_dictionary.O, 32, salt, 0, 8);

        var userValidation = new byte[48];
        Array.Copy(_dictionary.U, 0, userValidation, 0, 48);

        var hash = KeyDerivation.ComputeOwnerPasswordHashR6(password, salt, userValidation);
        var validationHash = new byte[32];
        Array.Copy(_dictionary.O, 0, validationHash, 0, 32);

        var isValid = _arraysEqual(hash, validationHash);

        if (!isValid)
            return DecryptionResult.Fail(_dictionary, _getKeyLength() * 8);

        var keySalt = new byte[8];
        Array.Copy(_dictionary.O, 40, keySalt, 0, 8);
        var encryptionKey = KeyDerivation.ComputeOwnerEncryptionKeyR6(password, keySalt, userValidation);

        return DecryptionResult.Success(_dictionary, AccessLevel.OwnerAccess, encryptionKey);
    }

    /// <summary>
    /// Get the key length in bytes
    /// </summary>
    /// <returns>The number of bytes in the key.</returns>
    private int _getKeyLength()
    {
        var lengthInBits = _dictionary.Length
            ?? ( _dictionary.V == 1 ? 40 : 128 );
        return lengthInBits / 8; // Convert bits to bytes
    }

    private static bool _arraysEqual(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length) return false;

        return array1.SequenceEqual(array2);
    }
}
