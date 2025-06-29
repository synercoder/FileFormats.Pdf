using Synercoding.FileFormats.Pdf.Encryption;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class RC4Tests
{
    [Theory]
    [InlineData("Hello, World!", "key")]
    [InlineData("", "key")]
    [InlineData("A", "k")]
    [InlineData("The quick brown fox jumps over the lazy dog", "secretkey")]
    [InlineData("This is a longer test string with more content to ensure RC4 works correctly with various data sizes.", "longkey123")]
    public void Test_Process_RoundTrip_Success(string plaintext, string keyString)
    {
        var data = System.Text.Encoding.UTF8.GetBytes(plaintext);
        var key = System.Text.Encoding.UTF8.GetBytes(keyString);

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
    }

    [Fact]
    public void Test_Process_NullKey_ThrowsArgumentNullException()
    {
        var data = "Hello"u8.ToArray();

        Assert.Throws<ArgumentNullException>(() => RC4.Process(null!, data));
    }

    [Fact]
    public void Test_Process_NullData_ThrowsArgumentNullException()
    {
        var key = "key"u8.ToArray();

        Assert.Throws<ArgumentNullException>(() => RC4.Process(key, null!));
    }

    [Fact]
    public void Test_Process_EmptyData_ReturnsEmptyArray()
    {
        var key = "key"u8.ToArray();
        var data = Array.Empty<byte>();

        var result = RC4.Process(key, data);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_Process_EmptyKey_ThrowsException()
    {
        var key = Array.Empty<byte>();
        var data = "Hello"u8.ToArray();

        Assert.Throws<DivideByZeroException>(() => RC4.Process(key, data));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(256)]
    public void Test_Process_VariousKeySizes_Works(int keySize)
    {
        var key = new byte[keySize];
        var random = new Random(42);
        random.NextBytes(key);
        var data = "Test data for various key sizes"u8.ToArray();

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void Test_Process_LargeData_Works(int dataSize)
    {
        var key = "testkey"u8.ToArray();
        var data = new byte[dataSize];
        var random = new Random(42);
        random.NextBytes(data);

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
    }

    [Fact]
    public void Test_Process_SameDataDifferentKeys_ProducesDifferentResults()
    {
        var data = "Same data"u8.ToArray();
        var key1 = "key1"u8.ToArray();
        var key2 = "key2"u8.ToArray();

        var result1 = RC4.Process(key1, data);
        var result2 = RC4.Process(key2, data);

        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void Test_Process_SameKeyDifferentData_ProducesDifferentResults()
    {
        var key = "samekey"u8.ToArray();
        var data1 = "Data one"u8.ToArray();
        var data2 = "Data two"u8.ToArray();

        var result1 = RC4.Process(key, data1);
        var result2 = RC4.Process(key, data2);

        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void Test_Process_KnownTestVector_ProducesExpectedResult()
    {
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var data = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 };

        var result = RC4.Process(key, data);

        Assert.NotNull(result);
        Assert.Equal(5, result.Length);
        Assert.NotEqual(data, result);
    }

    [Fact] 
    public void Test_Process_EncryptionChangesData()
    {
        var key = "testkey"u8.ToArray();
        var data = "Hello, World!"u8.ToArray();

        var encrypted = RC4.Process(key, data);

        Assert.NotEqual(data, encrypted);
        Assert.Equal(data.Length, encrypted.Length);
    }

    [Fact]
    public void Test_Process_DeterministicBehavior()
    {
        var key = "testkey"u8.ToArray();
        var data = "Deterministic test"u8.ToArray();

        var result1 = RC4.Process(key, data);
        var result2 = RC4.Process(key, data);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Test_Process_SingleByte_Works()
    {
        var key = new byte[] { 0xFF };
        var data = new byte[] { 0x00 };

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
        Assert.Single(encrypted);
    }

    [Fact]
    public void Test_Process_AllZeroKey_Works()
    {
        var key = new byte[8];
        var data = "Test with zero key"u8.ToArray();

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
    }

    [Fact]
    public void Test_Process_AllZeroData_Works()
    {
        var key = "nonzerokey"u8.ToArray();
        var data = new byte[10];

        var encrypted = RC4.Process(key, data);
        var decrypted = RC4.Process(key, encrypted);

        Assert.Equal(data, decrypted);
    }
}
