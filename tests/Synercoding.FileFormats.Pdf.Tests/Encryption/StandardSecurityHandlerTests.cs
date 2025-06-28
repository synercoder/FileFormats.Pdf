using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;
using System;
using Xunit;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class StandardSecurityHandlerTests
{
    private static readonly byte[] TestFileId = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];

    [Fact]
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StandardSecurityHandler(null!, TestFileId));
    }

    [Fact]
    public void Test_Constructor_NullFileId_ThrowsArgumentNullException()
    {
        var dictionary = _createValidDictionary();

        Assert.Throws<ArgumentNullException>(() => new StandardSecurityHandler(dictionary, null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("testpassword")]
    [InlineData("user123")]
    public void Test_AuthenticateUserPassword_ValidPassword_ReturnsResult(string password)
    {
        var dictionary = _createValidDictionaryWithPassword(password);
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateUserPassword(password);

        Assert.Equal(AccessLevel.UserAccess, result.AccessLevel);
        Assert.NotNull(result.EncryptionKey);
        Assert.NotEmpty(result.EncryptionKey);
    }

    [Theory]
    [InlineData("testpassword")]
    [InlineData("owner123")]
    public void Test_AuthenticateOwnerPassword_ValidPassword_ReturnsResult(string password)
    {
        var dictionary = _createValidDictionaryWithOwnerPassword(password);
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateOwnerPassword(password);

        Assert.Equal(AccessLevel.OwnerAccess, result.AccessLevel);
        Assert.NotNull(result.EncryptionKey);
        Assert.NotEmpty(result.EncryptionKey);
    }

    [Fact]
    public void Test_AuthenticateUserPassword_WrongPassword_ReturnsEncrypted()
    {
        var dictionary = _createValidDictionaryWithPassword("correctpassword");
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateUserPassword("wrongpassword");

        Assert.Equal(AccessLevel.Encrypted, result.AccessLevel);
    }

    [Fact]
    public void Test_AuthenticateUserPasswordR6_ValidPassword()
    {
        var dictionary = _createValidR6Dictionary();
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateUserPassword("testpassword");

        Assert.Equal(AccessLevel.Encrypted, result.AccessLevel);
    }

    [Fact]
    public void Test_AuthenticateOwnerPasswordR6_ValidPassword()
    {
        var dictionary = _createValidR6Dictionary();
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateOwnerPassword("testpassword");

        Assert.Equal(AccessLevel.Encrypted, result.AccessLevel);
    }

    [Theory]
    [InlineData(2, 40)]   // 40 bits = 5 bytes
    [InlineData(3, 128)]  // 128 bits = 16 bytes  
    [InlineData(4, 128)]  // 128 bits = 16 bytes
    public void Test_AuthenticateUserPassword_DifferentRevisions(int revision, int keyLengthBits)
    {
        const string TEST_PASSWORD = "testpassword";
        var dictionary = _createValidDictionaryWithRevision(revision, keyLengthBits, TEST_PASSWORD);
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateUserPassword(TEST_PASSWORD);

        Assert.Equal(AccessLevel.UserAccess, result.AccessLevel);
    }

    [Fact]
    public void Test_GetEncryptionKey_AfterAuthentication_ReturnsValidKey()
    {
        const string TEST_PASSWORD = "testpassword";
        var dictionary = _createValidDictionaryWithPassword(TEST_PASSWORD);
        var handler = new StandardSecurityHandler(dictionary, TestFileId);

        var result = handler.AuthenticateUserPassword(TEST_PASSWORD);
        var key = result.EncryptionKey;

        Assert.NotNull(key);
        Assert.True(key.Length > 0);
    }

    [Fact]
    public void Test_AuthenticateUserPassword_RealAES128Data_ValidPassword()
    {
        var (dictionary, fileId, _, userPassword) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(userPassword);

        Assert.Equal(AccessLevel.UserAccess, result.AccessLevel);
    }

    [Fact]
    public void Test_AuthenticateOwnerPassword_RealAES128Data_ValidPassword()
    {
        var (dictionary, fileId, ownerPassword, _) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(ownerPassword);

        Assert.Equal(AccessLevel.OwnerAccess, result.AccessLevel);
    }

    [Fact]
    public void Test_AuthenticateUserPassword_RealRC4128Data_ValidPassword()
    {
        var (dictionary, fileId, _, userPassword) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(userPassword);

        Assert.Equal(AccessLevel.UserAccess, result.AccessLevel);
    }

    [Fact]
    public void Test_AuthenticateOwnerPassword_RealRC4128Data_ValidPassword()
    {
        var (dictionary, fileId, ownerPassword, _) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(ownerPassword);

        Assert.Equal(AccessLevel.OwnerAccess, result.AccessLevel);
    }

    [Theory]
    [InlineData("wrongpassword")]
    [InlineData("")]
    [InlineData("invalidpass")]
    public void Test_AuthenticateUserPassword_RealAES128Data_WrongPassword(string wrongPassword)
    {
        var (dictionary, fileId, _, _) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(wrongPassword);

        Assert.NotEqual(AccessLevel.UserAccess, result.AccessLevel);
    }

    [Theory]
    [InlineData("wrongpassword")]
    [InlineData("")]
    [InlineData("invalidpass")]
    public void Test_AuthenticateOwnerPassword_RealAES128Data_WrongPassword(string wrongPassword)
    {
        var (dictionary, fileId, _, _) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(wrongPassword);

        Assert.NotEqual(AccessLevel.OwnerAccess, result.AccessLevel);
    }

    [Theory]
    [InlineData("wrongpassword")]
    [InlineData("")]
    [InlineData("invalidpass")]
    public void Test_AuthenticateUserPassword_RealRC4128Data_WrongPassword(string wrongPassword)
    {
        var (dictionary, fileId, _, _) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(wrongPassword);

        Assert.NotEqual(AccessLevel.UserAccess, result.AccessLevel);
    }

    [Theory]
    [InlineData("wrongpassword")]
    [InlineData("")]
    [InlineData("invalidpass")]
    public void Test_AuthenticateOwnerPassword_RealRC4128Data_WrongPassword(string wrongPassword)
    {
        var (dictionary, fileId, _, _) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(wrongPassword);

        Assert.NotEqual(AccessLevel.OwnerAccess, result.AccessLevel);
    }

    [Fact]
    public void Test_GetEncryptionKey_RealAES128Data_AfterUserAuthentication()
    {
        var (dictionary, fileId, _, userPassword) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(userPassword);
        var key = result.EncryptionKey;

        Assert.NotNull(key);
        Assert.Equal(128, result.KeyLength);
        Assert.Equal(EncryptionMethod.AES, result.StreamEncryptionMethod);
        Assert.Equal(EncryptionMethod.AES, result.TextEncryptionMethod);
        Assert.Equal(16, key.Length); // AES-128 key length
    }

    [Fact]
    public void Test_GetEncryptionKey_RealRC4128Data_AfterUserAuthentication()
    {
        var (dictionary, fileId, _, userPassword) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateUserPassword(userPassword);
        var key = result.EncryptionKey;

        Assert.NotNull(key);
        Assert.Equal(128, result.KeyLength);
        Assert.Equal(EncryptionMethod.RC4, result.StreamEncryptionMethod);
        Assert.Equal(EncryptionMethod.RC4, result.TextEncryptionMethod);
        Assert.Equal(16, key.Length); // RC4-128 key length
    }

    [Fact]
    public void Test_GetEncryptionKey_RealAES128Data_AfterOwnerAuthentication()
    {
        var (dictionary, fileId, ownerPassword, _) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(ownerPassword);
        var key = result.EncryptionKey;

        Assert.NotNull(key);
        Assert.Equal(128, result.KeyLength);
        Assert.Equal(EncryptionMethod.AES, result.StreamEncryptionMethod);
        Assert.Equal(EncryptionMethod.AES, result.TextEncryptionMethod);
        Assert.Equal(16, key.Length); // AES-128 key length
    }

    [Fact]
    public void Test_GetEncryptionKey_RealRC4128Data_AfterOwnerAuthentication()
    {
        var (dictionary, fileId, ownerPassword, _) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);

        var result = handler.AuthenticateOwnerPassword(ownerPassword);
        var key = result.EncryptionKey;

        Assert.NotNull(key);
        Assert.Equal(128, result.KeyLength);
        Assert.Equal(EncryptionMethod.RC4, result.StreamEncryptionMethod);
        Assert.Equal(EncryptionMethod.RC4, result.TextEncryptionMethod);
        Assert.Equal(16, key.Length); // RC4-128 key length
    }

    [Fact]
    public void Debug_TestRealPdfKeyDerivation()
    {
        var (dictionary, fileId, ownerPassword, userPassword) = _createAes128BitDictionary();
        var keyLength = dictionary.Length.HasValue ? dictionary.Length.Value / 8 : 16;

        var computedUserKey = KeyDerivation.ComputeUserKey(
            userPassword,
            dictionary.O,
            dictionary.P,
            fileId,
            dictionary.R,
            keyLength,
            dictionary.EncryptMetadata ?? true);

        // According to PDF spec Algorithm 5 step f, only first 16 bytes matter for validation
        var firstBytesMatch = dictionary.U[..16].SequenceEqual(computedUserKey[..16]);
        var computedHex = Convert.ToHexString(computedUserKey[0..16]);
        var expectedHex = Convert.ToHexString(dictionary.U[0..16]);

        Assert.True(firstBytesMatch, $"First 16 bytes - Expected: {expectedHex}, Got: {computedHex}");
    }

    private static StandardEncryptionDictionary _createValidDictionary()
    {
        var keyLength = 16;
        var ownerKey = KeyDerivation.ComputeOwnerKey("", "defaultpassword", 3, keyLength);
        var userKey = KeyDerivation.ComputeUserKey("defaultpassword", ownerKey, (UserAccessPermissions)(-44), TestFileId, 3, keyLength, true);

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(2),
            [PdfNames.R] = new PdfNumber(3),
            [PdfNames.Length] = new PdfNumber(keyLength * 8),
            [PdfNames.O] = new PdfString(ownerKey, true),
            [PdfNames.U] = new PdfString(userKey, true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };
        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static StandardEncryptionDictionary _createValidDictionaryWithPassword(string password)
    {
        var keyLength = 16;
        var ownerKey = KeyDerivation.ComputeOwnerKey("", password, 3, keyLength);
        var userKey = KeyDerivation.ComputeUserKey(password, ownerKey, (UserAccessPermissions)(-44), TestFileId, 3, keyLength, true);

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(2),
            [PdfNames.R] = new PdfNumber(3),
            [PdfNames.Length] = new PdfNumber(keyLength * 8),
            [PdfNames.O] = new PdfString(ownerKey, true),
            [PdfNames.U] = new PdfString(userKey, true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };
        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static StandardEncryptionDictionary _createValidDictionaryWithOwnerPassword(string password)
    {
        var keyLength = 16;
        var ownerKey = KeyDerivation.ComputeOwnerKey(password, "userpass", 3, keyLength);
        var userKey = KeyDerivation.ComputeUserKey("userpass", ownerKey, (UserAccessPermissions)(-44), TestFileId, 3, keyLength, true);

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(2),
            [PdfNames.R] = new PdfNumber(3),
            [PdfNames.Length] = new PdfNumber(keyLength * 8),
            [PdfNames.O] = new PdfString(ownerKey, true),
            [PdfNames.U] = new PdfString(userKey, true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };
        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static StandardEncryptionDictionary _createValidR6Dictionary()
    {
        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(5),
            [PdfNames.R] = new PdfNumber(6),
            [PdfNames.Length] = new PdfNumber(256),
            [PdfNames.O] = new PdfString(new byte[48], true),
            [PdfNames.U] = new PdfString(new byte[48], true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };
        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static StandardEncryptionDictionary _createValidDictionaryWithRevision(int revision, int keyLengthBits, string password)
    {
        var keyLengthBytes = keyLengthBits / 8;
        var ownerKey = KeyDerivation.ComputeOwnerKey("", password, revision, keyLengthBytes);
        var userKey = KeyDerivation.ComputeUserKey(password, ownerKey, (UserAccessPermissions)(-44), TestFileId, revision, keyLengthBytes, true);

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(revision >= 4 ? 4 : 2),
            [PdfNames.R] = new PdfNumber(revision),
            [PdfNames.Length] = new PdfNumber(keyLengthBits),
            [PdfNames.O] = new PdfString(ownerKey, true),
            [PdfNames.U] = new PdfString(userKey, true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };
        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static (StandardEncryptionDictionary Dictionary, byte[] FileId, string OwnerPassword, string UserPassword) _createAes128BitDictionary()
    {
        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(4),
            [PdfNames.R] = new PdfNumber(4),
            [PdfNames.Length] = new PdfNumber(128),
            [PdfNames.O] = new PdfString(new byte[] {
                0xB3, 0x09, 0x2B, 0x53, 0xDA, 0xC3, 0x58, 0x76,
                0x6A, 0x56, 0x00, 0x8F, 0xC7, 0x02, 0x2B, 0x8F,
                0x51, 0xD2, 0x23, 0xB5, 0xEA, 0xDC, 0x53, 0x12,
                0x99, 0x27, 0x7F, 0x2F, 0x49, 0xEE, 0xBA, 0x3F
            }, true),
            [PdfNames.U] = new PdfString(new byte[]
            {
                0xCC, 0x74, 0x57, 0x94, 0xCA, 0xA6, 0x00, 0xA6,
                0x68, 0xB1, 0xFC, 0x5B, 0x54, 0xB8, 0xF0, 0x36,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }, true),
            [PdfNames.P] = new PdfNumber(-3392),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true),
            [PdfNames.StmF] = PdfName.Get("StdCF"),
            [PdfNames.StrF] = PdfName.Get("StdCF"),
            [PdfNames.CF] = new PdfDictionary()
            {
                [PdfName.Get("StdCF")] = new PdfDictionary()
                {
                    [PdfName.Get("AuthEvent")] = PdfName.Get("DocOpen"),
                    [PdfNames.CFM] = PdfNames.AESV2,
                    [PdfNames.Length] = new PdfNumber(16)
                }
            }
        };
        var dictionary = new StandardEncryptionDictionary(pdfDictionary, null!);

        var fileId = new byte[]
        {
            0x52, 0xB9, 0x35, 0x67, 0x16, 0x2C, 0x77, 0x47,
            0xB9, 0x96, 0xCB, 0xF6, 0xDF, 0xE5, 0x7A, 0x79
        };

        return (dictionary, fileId, "ChangePW", "OpenPW");
    }

    private static (StandardEncryptionDictionary Dictionary, byte[] FileId, string OwnerPassword, string UserPassword) _createRc4128BitDictionary()
    {
        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(4),
            [PdfNames.R] = new PdfNumber(4),
            [PdfNames.Length] = new PdfNumber(128),
            [PdfNames.O] = new PdfString(new byte[] {
                0xB3, 0x09, 0x2B, 0x53, 0xDA, 0xC3, 0x58, 0x76,
                0x6A, 0x56, 0x00, 0x8F, 0xC7, 0x02, 0x2B, 0x8F,
                0x51, 0xD2, 0x23, 0xB5, 0xEA, 0xDC, 0x53, 0x12,
                0x99, 0x27, 0x7F, 0x2F, 0x49, 0xEE, 0xBA, 0x3F
            }, true),
            [PdfNames.U] = new PdfString(new byte[]
            {
                0xCC, 0x74, 0x57, 0x94, 0xCA, 0xA6, 0x00, 0xA6,
                0x68, 0xB1, 0xFC, 0x5B, 0x54, 0xB8, 0xF0, 0x36,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }, true),
            [PdfNames.P] = new PdfNumber(-3392),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true),
            [PdfNames.StmF] = PdfName.Get("StdCF"),
            [PdfNames.StrF] = PdfName.Get("StdCF"),
            [PdfNames.CF] = new PdfDictionary()
            {
                [PdfName.Get("StdCF")] = new PdfDictionary()
                {
                    [PdfName.Get("AuthEvent")] = PdfName.Get("DocOpen"),
                    [PdfNames.CFM] = PdfNames.V2,
                    [PdfNames.Length] = new PdfNumber(16)
                }
            }
        };
        var dictionary = new StandardEncryptionDictionary(pdfDictionary, null!);
        var fileId = new byte[]
        {
            0x52, 0xB9, 0x35, 0x67, 0x16, 0x2C, 0x77, 0x47,
            0xB9, 0x96, 0xCB, 0xF6, 0xDF, 0xE5, 0x7A, 0x79
        };

        return (dictionary, fileId, "ChangePW", "OpenPW");
    }
}
