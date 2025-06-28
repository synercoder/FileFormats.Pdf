using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Exceptions;
using Xunit;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class CryptFilterTests
{
    private static readonly byte[] _testData = "Hello, World! This is test data for encryption."u8.ToArray();
    private static readonly byte[] _testKey128 = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];
    private static readonly byte[] _testKey256 = [
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
        0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
        0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
    ];

    [Fact]
    public void Test_DecryptWithRC4_ValidInputs()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var result = CryptFilter.DecryptWithRC4(_testData, key);

        Assert.NotNull(result);
        Assert.Equal(_testData.Length, result.Length);
    }

    [Fact]
    public void Test_DecryptWithRC4_NullData_ThrowsArgumentNullException()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithRC4(null!, key));
    }

    [Fact]
    public void Test_DecryptWithRC4_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithRC4(_testData, null!));
    }

    [Fact]
    public void Test_DecryptWithAES128_ValidInputs()
    {
        var encryptedData = CryptFilter.EncryptWithAES128(_testData, _testKey128);

        var result = CryptFilter.DecryptWithAES128(encryptedData, _testKey128);

        Assert.Equal(_testData, result);
    }

    [Fact]
    public void Test_DecryptWithAES128_InvalidKeyLength_ThrowsEncryptionException()
    {
        var invalidKey = new byte[] { 0x01, 0x02, 0x03 };

        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES128(_testData, invalidKey));
    }

    [Fact]
    public void Test_DecryptWithAES128_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithAES128(null!, _testKey128));
    }

    [Fact]
    public void Test_DecryptWithAES128_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithAES128(_testData, null!));
    }

    [Fact]
    public void Test_DecryptWithAES256_ValidInputs()
    {
        var encryptedData = CryptFilter.EncryptWithAES256(_testData, _testKey256);

        var result = CryptFilter.DecryptWithAES256(encryptedData, _testKey256);

        Assert.Equal(_testData, result);
    }

    [Fact]
    public void Test_DecryptWithAES256_InvalidKeyLength_ThrowsEncryptionException()
    {
        var invalidKey = new byte[] { 0x01, 0x02, 0x03 };

        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES256(_testData, invalidKey));
    }

    [Fact]
    public void Test_DecryptWithAES256_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithAES256(null!, _testKey256));
    }

    [Fact]
    public void Test_DecryptWithAES256_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.DecryptWithAES256(_testData, null!));
    }

    [Fact]
    public void Test_EncryptWithRC4_ValidInputs()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var result = CryptFilter.EncryptWithRC4(_testData, key);

        Assert.NotNull(result);
        Assert.Equal(_testData.Length, result.Length);
    }

    [Fact]
    public void Test_EncryptWithRC4_NullData_ThrowsArgumentNullException()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithRC4(null!, key));
    }

    [Fact]
    public void Test_EncryptWithRC4_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithRC4(_testData, null!));
    }

    [Fact]
    public void Test_EncryptWithAES128_ValidInputs()
    {
        var result = CryptFilter.EncryptWithAES128(_testData, _testKey128);

        Assert.NotNull(result);
        Assert.True(result.Length >= 16);
        Assert.NotEqual(_testData, result);
    }

    [Fact]
    public void Test_EncryptWithAES128_InvalidKeyLength_ThrowsEncryptionException()
    {
        var invalidKey = new byte[] { 0x01, 0x02, 0x03 };

        Assert.Throws<EncryptionException>(() => CryptFilter.EncryptWithAES128(_testData, invalidKey));
    }

    [Fact]
    public void Test_EncryptWithAES128_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithAES128(null!, _testKey128));
    }

    [Fact]
    public void Test_EncryptWithAES128_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithAES128(_testData, null!));
    }

    [Fact]
    public void Test_EncryptWithAES256_ValidInputs()
    {
        var result = CryptFilter.EncryptWithAES256(_testData, _testKey256);

        Assert.NotNull(result);
        Assert.True(result.Length >= 16);
        Assert.NotEqual(_testData, result);
    }

    [Fact]
    public void Test_EncryptWithAES256_InvalidKeyLength_ThrowsEncryptionException()
    {
        var invalidKey = new byte[] { 0x01, 0x02, 0x03 };

        Assert.Throws<EncryptionException>(() => CryptFilter.EncryptWithAES256(_testData, invalidKey));
    }

    [Fact]
    public void Test_EncryptWithAES256_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithAES256(null!, _testKey256));
    }

    [Fact]
    public void Test_EncryptWithAES256_NullKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CryptFilter.EncryptWithAES256(_testData, null!));
    }

    [Fact]
    public void Test_RC4_RoundTrip()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted = CryptFilter.EncryptWithRC4(_testData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(_testData, decrypted);
    }

    [Fact]
    public void Test_AES128_RoundTrip()
    {
        var encrypted = CryptFilter.EncryptWithAES128(_testData, _testKey128);
        var decrypted = CryptFilter.DecryptWithAES128(encrypted, _testKey128);

        Assert.Equal(_testData, decrypted);
    }

    [Fact]
    public void Test_AES256_RoundTrip()
    {
        var encrypted = CryptFilter.EncryptWithAES256(_testData, _testKey256);
        var decrypted = CryptFilter.DecryptWithAES256(encrypted, _testKey256);

        Assert.Equal(_testData, decrypted);
    }

    [Fact]
    public void Test_AES_DataTooShort_ThrowsEncryptionException()
    {
        var shortData = new byte[8];

        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES128(shortData, _testKey128));
    }

    [Fact]
    public void Test_AES_EmptyData_Works()
    {
        var emptyData = Array.Empty<byte>();

        var encrypted = CryptFilter.EncryptWithAES128(emptyData, _testKey128);
        var decrypted = CryptFilter.DecryptWithAES128(encrypted, _testKey128);

        Assert.Equal(emptyData, decrypted);
    }

    [Fact]
    public void Test_RC4_EmptyData_Works()
    {
        var emptyData = Array.Empty<byte>();
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted = CryptFilter.EncryptWithRC4(emptyData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(emptyData, decrypted);
    }

    [Fact]
    public void Test_AES_LargeData_Works()
    {
        var largeData = new byte[10000];
        new Random(42).NextBytes(largeData);

        var encrypted = CryptFilter.EncryptWithAES128(largeData, _testKey128);
        var decrypted = CryptFilter.DecryptWithAES128(encrypted, _testKey128);

        Assert.Equal(largeData, decrypted);
    }

    [Fact]
    public void Test_RC4_LargeData_Works()
    {
        var largeData = new byte[10000];
        new Random(42).NextBytes(largeData);
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted = CryptFilter.EncryptWithRC4(largeData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(largeData, decrypted);
    }
}
