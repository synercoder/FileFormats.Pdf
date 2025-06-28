using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class PasswordDecryptorTests
{
    private static readonly byte[] _testEncryptionKey = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10];

    [Fact]
    public void Test_Constructor_ValidInputs()
    {
        var dictionary = _createValidDictionary();
        
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        
        Assert.NotNull(decryptor);
    }

    [Fact]
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PasswordDecryptor(null!, _testEncryptionKey));
    }

    [Fact]
    public void Test_Constructor_NullEncryptionKey_ThrowsArgumentNullException()
    {
        var dictionary = _createValidDictionary();
        
        Assert.Throws<ArgumentNullException>(() => new PasswordDecryptor(dictionary, null!));
    }

    [Fact]
    public void Test_Decrypt_PdfString_ValidInputs()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString("Hello, World!"u8.ToArray(), false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.IsType<PdfString>(result);
        Assert.Equal(originalString.IsHex, result.IsHex);
    }

    [Fact]
    public void Test_Decrypt_PdfString_NullString_ThrowsArgumentNullException()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var objectId = new PdfObjectId(1, 0);
        
        Assert.Throws<ArgumentNullException>(() => decryptor.Decrypt((PdfString)null!, objectId));
    }

    [Fact]
    public void Test_Decrypt_StreamObject_ValidInputs()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var streamDictionary = new PdfDictionary();
        var streamData = "Hello, World!"u8.ToArray();
        streamDictionary[PdfNames.Length] = new PdfNumber(streamData.Length);
        var stream = new ReadOnlyPdfStreamObject(streamDictionary, streamData);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(stream, objectId);
        
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IPdfStreamObject>(result);
        Assert.Equal(streamData.Length, result.RawData.Length);
    }

    [Fact]
    public void Test_Decrypt_StreamObject_NullStream_ThrowsArgumentNullException()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var objectId = new PdfObjectId(1, 0);
        
        Assert.Throws<ArgumentNullException>(() => decryptor.Decrypt((IPdfStreamObject)null!, objectId));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(100, 5)]
    [InlineData(999, 10)]
    public void Test_Decrypt_PdfString_DifferentObjectIds(int objectNumber, int generation)
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString("Test data"u8.ToArray(), false);
        var objectId = new PdfObjectId(objectNumber, generation);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.IsType<PdfString>(result);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(100, 5)]
    [InlineData(999, 10)]
    public void Test_Decrypt_StreamObject_DifferentObjectIds(int objectNumber, int generation)
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var streamDictionary = new PdfDictionary();
        var streamData = "Test data"u8.ToArray();
        streamDictionary[PdfNames.Length] = new PdfNumber(streamData.Length);
        var stream = new ReadOnlyPdfStreamObject(streamDictionary, streamData);
        var objectId = new PdfObjectId(objectNumber, generation);
        
        var result = decryptor.Decrypt(stream, objectId);
        
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IPdfStreamObject>(result);
    }

    [Fact]
    public void Test_Decrypt_PdfString_HexString()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString("Hello, World!"u8.ToArray(), true);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.True(result.IsHex);
    }

    [Fact]
    public void Test_Decrypt_PdfString_LiteralString()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString("Hello, World!"u8.ToArray(), false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.False(result.IsHex);
    }

    [Fact]
    public void Test_Decrypt_EmptyPdfString()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString(Array.Empty<byte>(), false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.Empty(result.Raw);
    }

    [Fact]
    public void Test_Decrypt_EmptyStreamData()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var streamDictionary = new PdfDictionary();
        var emptyData = Array.Empty<byte>();
        streamDictionary[PdfNames.Length] = new PdfNumber(emptyData.Length);
        var stream = new ReadOnlyPdfStreamObject(streamDictionary, emptyData);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(stream, objectId);
        
        Assert.NotNull(result);
        Assert.Empty(result.RawData);
    }

    [Theory]
    [InlineData(1, "V2")]
    [InlineData(2, "V2")]
    [InlineData(3, "V2")]
    [InlineData(4, "V4")]
    public void Test_Decrypt_DifferentVersions(int version, string _)
    {
        var dictionary = _createDictionaryWithVersion(version);
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var originalString = new PdfString("Test"u8.ToArray(), false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_Decrypt_Version5_AES256()
    {
        var dictionary = _createDictionaryWithVersion(5);
        var key256 = new byte[32] { 
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
        };
        var decryptor = new PasswordDecryptor(dictionary, key256);
        
        // Create properly encrypted AES data (with IV)
        var originalData = "Test"u8.ToArray();
        var encryptedData = CryptFilter.EncryptWithAES256(originalData, key256);
        var encryptedString = new PdfString(encryptedData, false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(encryptedString, objectId);
        
        Assert.NotNull(result);
        Assert.Equal("Test", result.Value);
    }

    [Fact]
    public void Test_Decrypt_StreamWithCustomFilter()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var streamDictionary = new PdfDictionary();
        var streamData = "Test data"u8.ToArray();
        streamDictionary[PdfNames.Length] = new PdfNumber(streamData.Length);
        streamDictionary[PdfNames.Filter] = PdfName.Get("CustomFilter");
        var stream = new ReadOnlyPdfStreamObject(streamDictionary, streamData);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(stream, objectId);
        
        Assert.NotNull(result);
    }

    [Fact]
    public void Test_Decrypt_LargeData()
    {
        var dictionary = _createValidDictionary();
        var decryptor = new PasswordDecryptor(dictionary, _testEncryptionKey);
        var largeData = new byte[10000];
        new Random(42).NextBytes(largeData);
        var originalString = new PdfString(largeData, false);
        var objectId = new PdfObjectId(1, 0);
        
        var result = decryptor.Decrypt(originalString, objectId);
        
        Assert.NotNull(result);
        Assert.Equal(largeData.Length, result.Raw.Length);
    }

    private static StandardEncryptionDictionary _createValidDictionary()
    {

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(2),
            [PdfNames.R] = new PdfNumber(3),
            [PdfNames.Length] = new PdfNumber(16),
            [PdfNames.O] = new PdfString(new byte[32], true),
            [PdfNames.U] = new PdfString(new byte[32], true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };

        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }

    private static StandardEncryptionDictionary _createDictionaryWithVersion(int version)
    {
        var revision = version switch
        {
            1 or 2 or 3 => 3,
            4 => version == 4 ? 4 : 5,
            5 => 6,
            _ => 3
        };

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(version),
            [PdfNames.R] = new PdfNumber(revision),
            [PdfNames.Length] = new PdfNumber(version ==1 ? 5 : 16),
            [PdfNames.O] = new PdfString(new byte[32], true),
            [PdfNames.U] = new PdfString(new byte[32], true),
            [PdfNames.P] = new PdfNumber(-44),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true)
        };

        return new StandardEncryptionDictionary(pdfDictionary, null!);
    }
}
