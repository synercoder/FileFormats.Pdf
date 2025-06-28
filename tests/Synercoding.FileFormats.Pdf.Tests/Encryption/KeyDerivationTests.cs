using Synercoding.FileFormats.Pdf.Encryption;
using System.Text;
using Xunit;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class KeyDerivationTests
{
    private static readonly byte[] _testFileId = [0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0];

    [Theory]
    [InlineData("", "", 2)]
    [InlineData("owner", "user", 2)]
    [InlineData("owner", "user", 3)]
    [InlineData("owner", "user", 4)]
    public void Test_ComputeOwnerKey_ValidInputs(string ownerPassword, string userPassword, int revision)
    {
        var result = KeyDerivation.ComputeOwnerKey(ownerPassword, userPassword, revision, 16);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("", 2, 5)]
    [InlineData("user", 2, 5)]
    [InlineData("user", 3, 16)]
    [InlineData("user", 4, 16)]
    public void Test_ComputeUserKey_ValidInputs(string userPassword, int revision, int keyLength)
    {
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var encryptMetadata = true;

        var result = KeyDerivation.ComputeUserKey(userPassword, ownerKey, permissions, _testFileId, revision, keyLength, encryptMetadata);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("", 2, 5)]
    [InlineData("user", 2, 5)]
    [InlineData("user", 3, 16)]
    [InlineData("user", 4, 16)]
    public void Test_ComputeEncryptionKey_ValidInputs(string password, int revision, int keyLength)
    {
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var encryptMetadata = true;

        var result = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, encryptMetadata);

        Assert.NotNull(result);
        Assert.Equal(keyLength, result.Length);
    }

    [Theory]
    [InlineData(1, 0, false)]
    [InlineData(1, 0, true)]
    [InlineData(100, 5, false)]
    [InlineData(100, 5, true)]
    public void Test_ComputeObjectKey_ValidInputs(int objectNumber, int generation, bool useAes)
    {
        var encryptionKey = new byte[16];

        var result = KeyDerivation.ComputeObjectKey(encryptionKey, objectNumber, generation, useAes);

        Assert.NotNull(result);
        Assert.True(result.Length <= 16);
        Assert.True(result.Length >= encryptionKey.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("password")]
    [InlineData("user123")]
    public void Test_ComputeUserPasswordHashR6_ValidInputs(string password)
    {
        var salt = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        var userKeyValidation = new byte[48];

        var result = KeyDerivation.ComputeUserPasswordHashR6(password, salt, userKeyValidation);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("password")]
    [InlineData("owner123")]
    public void Test_ComputeOwnerPasswordHashR6_ValidInputs(string password)
    {
        var salt = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        var userKeyValidation = new byte[48];

        var result = KeyDerivation.ComputeOwnerPasswordHashR6(password, salt, userKeyValidation);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("password")]
    [InlineData("user123")]
    public void Test_ComputeUserEncryptionKeyR6_ValidInputs(string password)
    {
        var salt = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

        var result = KeyDerivation.ComputeUserEncryptionKeyR6(password, salt);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("password")]
    [InlineData("owner123")]
    public void Test_ComputeOwnerEncryptionKeyR6_ValidInputs(string password)
    {
        var salt = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        var userKeyValidation = new byte[48];

        var result = KeyDerivation.ComputeOwnerEncryptionKeyR6(password, salt, userKeyValidation);

        Assert.NotNull(result);
        Assert.Equal(32, result.Length);
    }

    [Fact]
    public void Test_ComputeOwnerKey_EmptyOwnerPassword_UsesUserPassword()
    {
        var ownerPassword = "";
        var userPassword = "testuser";
        var revision = 3;

        var result1 = KeyDerivation.ComputeOwnerKey(ownerPassword, userPassword, revision, 16);
        var result2 = KeyDerivation.ComputeOwnerKey(userPassword, userPassword, revision, 16);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Test_ComputeObjectKey_AESFlag_AddsCorrectSalt()
    {
        var encryptionKey = new byte[16];
        var objectNumber = 1;
        var generation = 0;

        var resultRC4 = KeyDerivation.ComputeObjectKey(encryptionKey, objectNumber, generation, false);
        var resultAES = KeyDerivation.ComputeObjectKey(encryptionKey, objectNumber, generation, true);

        Assert.NotEqual(resultRC4, resultAES);
    }

    [Fact]
    public void Test_ComputeEncryptionKey_WithoutMetadataEncryption()
    {
        var password = "test";
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var revision = 4;
        var keyLength = 16;

        var resultWithMetadata = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, true);
        var resultWithoutMetadata = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, false);

        Assert.NotEqual(resultWithMetadata, resultWithoutMetadata);
    }

    [Theory]
    [InlineData(0, 0, 16)]
    [InlineData(1, 0, 16)]
    [InlineData(255, 0, 16)]
    [InlineData(65535, 0, 16)]
    [InlineData(16777215, 0, 16)]
    [InlineData(1, 255, 16)]
    [InlineData(1, 65535, 16)]
    public void Test_ComputeObjectKey_LargeBoundaryValues(int objectNumber, int generation, int encryptionKeyLength)
    {
        var encryptionKey = new byte[encryptionKeyLength];

        var result = KeyDerivation.ComputeObjectKey(encryptionKey, objectNumber, generation, false);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Test_ComputeUserKey_Revision2_Returns32Bytes()
    {
        var userPassword = "test";
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var revision = 2;
        var keyLength = 5;

        var result = KeyDerivation.ComputeUserKey(userPassword, ownerKey, permissions, _testFileId, revision, keyLength, true);

        Assert.Equal(32, result.Length);
    }

    [Fact]
    public void Test_ComputeUserKey_Revision3_Returns32Bytes()
    {
        var userPassword = "test";
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var revision = 3;
        var keyLength = 16;

        var result = KeyDerivation.ComputeUserKey(userPassword, ownerKey, permissions, _testFileId, revision, keyLength, true);

        Assert.Equal(32, result.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("test")]
    [InlineData("verylongpasswordthatexceeds32characters")]
    public void Test_KeyDerivation_DifferentPasswordLengths(string password)
    {
        var ownerKey = new byte[32];
        var permissions = (UserAccessPermissions)( -44 );
        var revision = 3;
        var keyLength = 16;

        var result = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, true);

        Assert.NotNull(result);
        Assert.Equal(keyLength, result.Length);
    }

    [Fact]
    public void Test_KeyDerivation_ConsistentResults()
    {
        var password = "test";
        var ownerKey = new byte[32] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
        var permissions = (UserAccessPermissions)( -44 );
        var revision = 3;
        var keyLength = 16;

        var result1 = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, true);
        var result2 = KeyDerivation.ComputeEncryptionKey(password, ownerKey, permissions, _testFileId, revision, keyLength, true);

        Assert.Equal(result1, result2);
    }
}
