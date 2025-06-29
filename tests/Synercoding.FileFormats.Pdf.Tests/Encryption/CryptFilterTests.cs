using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Exceptions;

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

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    public void Test_RC4_VariousKeySizes_RoundTrip(int keySize)
    {
        var key = new byte[keySize];
        new Random(42).NextBytes(key);

        var encrypted = CryptFilter.EncryptWithRC4(_testData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(_testData, decrypted);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(63)]
    [InlineData(64)]
    public void Test_AES128_VariousDataSizes_RoundTrip(int dataSize)
    {
        var data = new byte[dataSize];
        new Random(42).NextBytes(data);

        var encrypted = CryptFilter.EncryptWithAES128(data, _testKey128);
        var decrypted = CryptFilter.DecryptWithAES128(encrypted, _testKey128);

        Assert.Equal(data, decrypted);
    }

    [Fact]
    public void Test_RC4_SingleByteKey_Works()
    {
        var key = new byte[] { 0xFF };

        var encrypted = CryptFilter.EncryptWithRC4(_testData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(_testData, decrypted);
    }

    [Fact]
    public void Test_RC4_AllZeroKey_Works()
    {
        var key = new byte[16];

        var encrypted = CryptFilter.EncryptWithRC4(_testData, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(_testData, decrypted);
    }

    [Fact]
    public void Test_RC4_AllZeroData_Works()
    {
        var data = new byte[32];
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted = CryptFilter.EncryptWithRC4(data, key);
        var decrypted = CryptFilter.DecryptWithRC4(encrypted, key);

        Assert.Equal(data, decrypted);
    }

    [Fact]
    public void Test_AES128_SameDataDifferentInvocations_ProducesDifferentResults()
    {
        var encrypted1 = CryptFilter.EncryptWithAES128(_testData, _testKey128);
        var encrypted2 = CryptFilter.EncryptWithAES128(_testData, _testKey128);

        Assert.NotEqual(encrypted1, encrypted2);
    }

    [Fact]
    public void Test_RC4_SameInputs_ProducesSameResults()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted1 = CryptFilter.EncryptWithRC4(_testData, key);
        var encrypted2 = CryptFilter.EncryptWithRC4(_testData, key);

        Assert.Equal(encrypted1, encrypted2);
    }

    [Fact]
    public void Test_AES128_EncryptionContainsIV()
    {
        var encrypted = CryptFilter.EncryptWithAES128(_testData, _testKey128);

        Assert.True(encrypted.Length >= 16 + _testData.Length);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(14)]
    [InlineData(8)]
    [InlineData(1)]
    [InlineData(0)]
    public void Test_AES128_DecryptTooShortData_ThrowsEncryptionException(int dataLength)
    {
        var shortData = new byte[dataLength];

        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES128(shortData, _testKey128));
    }

    [Fact]
    public void Test_AES128_ExactMinimumSize_Works()
    {
        var minData = new byte[16];
        new Random(42).NextBytes(minData);

        var encrypted = CryptFilter.EncryptWithAES128(minData, _testKey128);
        var decrypted = CryptFilter.DecryptWithAES128(encrypted, _testKey128);

        Assert.Equal(minData, decrypted);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(33)]
    public void Test_AES128_InvalidKeyLength_ThrowsEncryptionException(int keyLength)
    {
        var invalidKey = new byte[keyLength];

        Assert.Throws<EncryptionException>(() => CryptFilter.EncryptWithAES128(_testData, invalidKey));
        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES128(_testData, invalidKey));
    }

    [Theory]
    [InlineData(31)]
    [InlineData(33)]
    [InlineData(15)]
    [InlineData(17)]
    [InlineData(64)]
    public void Test_AES256_InvalidKeyLength_ThrowsEncryptionException(int keyLength)
    {
        var invalidKey = new byte[keyLength];

        Assert.Throws<EncryptionException>(() => CryptFilter.EncryptWithAES256(_testData, invalidKey));
        Assert.Throws<EncryptionException>(() => CryptFilter.DecryptWithAES256(_testData, invalidKey));
    }

    [Fact]
    public void Test_RC4_EncryptDecryptModifiesData()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        var encrypted = CryptFilter.EncryptWithRC4(_testData, key);

        Assert.NotEqual(_testData, encrypted);
        Assert.Equal(_testData.Length, encrypted.Length);
    }

    [Fact]
    public void Test_AES128_EncryptModifiesAndExpandsData()
    {
        var encrypted = CryptFilter.EncryptWithAES128(_testData, _testKey128);

        Assert.NotEqual(_testData, encrypted);
        Assert.True(encrypted.Length > _testData.Length);
    }

    [Fact]
    public void Test_RC4_EmptyKey_ThrowsException()
    {
        var emptyKey = Array.Empty<byte>();

        Assert.Throws<DivideByZeroException>(() => CryptFilter.EncryptWithRC4(_testData, emptyKey));
        Assert.Throws<DivideByZeroException>(() => CryptFilter.DecryptWithRC4(_testData, emptyKey));
    }

    [Fact]
    public void Test_AES128_RepeatedEncryption_ProducesDifferentResults()
    {
        var results = new List<byte[]>();
        
        for (int i = 0; i < 5; i++)
        {
            var encrypted = CryptFilter.EncryptWithAES128(_testData, _testKey128);
            results.Add(encrypted);
        }

        for (int i = 0; i < results.Count; i++)
        {
            for (int j = i + 1; j < results.Count; j++)
            {
                Assert.NotEqual(results[i], results[j]);
            }
        }
    }

    [Fact]
    public void Test_RC4_DifferentKeys_ProduceDifferentResults()
    {
        var key1 = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var key2 = new byte[] { 0x05, 0x04, 0x03, 0x02, 0x01 };

        var encrypted1 = CryptFilter.EncryptWithRC4(_testData, key1);
        var encrypted2 = CryptFilter.EncryptWithRC4(_testData, key2);

        Assert.NotEqual(encrypted1, encrypted2);
    }

    [Fact]
    public void Test_AES128_DifferentKeys_ProduceDifferentResults()
    {
        var key2 = new byte[16];
        Array.Fill(key2, (byte)0xFF);

        var encrypted1 = CryptFilter.EncryptWithAES128(_testData, _testKey128);
        var encrypted2 = CryptFilter.EncryptWithAES128(_testData, key2);

        var decrypted1 = CryptFilter.DecryptWithAES128(encrypted1, _testKey128);
        var decrypted2 = CryptFilter.DecryptWithAES128(encrypted2, key2);

        Assert.Equal(_testData, decrypted1);
        Assert.Equal(_testData, decrypted2);
        Assert.NotEqual(encrypted1, encrypted2);
    }
}
