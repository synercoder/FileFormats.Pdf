using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Exceptions;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfDictionaryTests
{
    private static PdfString CreatePdfString(string value, PdfStringEncoding encoding, bool isHex)
    {
        byte[] bytes = encoding switch
        {
            PdfStringEncoding.PdfDocEncoding => Encoding.Latin1.GetBytes(value),
            PdfStringEncoding.Utf16BE => [0xFE, 0xFF, .. Encoding.BigEndianUnicode.GetBytes(value)],
            PdfStringEncoding.Utf16LE => [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(value)],
            PdfStringEncoding.Utf8 => [0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes(value)],
            PdfStringEncoding.ByteString => Convert.FromHexString(value.Replace("<", "").Replace(">", "")),
            _ => throw new ArgumentException($"Unsupported encoding: {encoding}")
        };
        
        return new PdfString(bytes, isHex);
    }
    [Fact]
    public void Test_Constructor_Empty_CreatesEmptyDictionary()
    {
        var dictionary = new PdfDictionary();
        
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Test_Constructor_WithDictionary_CopiesAllEntries()
    {
        var source = new PdfDictionary();
        var key1 = PdfName.Get("Key1");
        var key2 = PdfName.Get("Key2");
        source.Add(key1, new PdfNumber(1));
        source.Add(key2, new PdfNumber(2));
        
        var dictionary = new PdfDictionary(source);
        
        Assert.Equal(2, dictionary.Count);
        Assert.True(dictionary.ContainsKey(key1));
        Assert.True(dictionary.ContainsKey(key2));
        Assert.Equal(1, ((PdfNumber)dictionary[key1]!).Value);
        Assert.Equal(2, ((PdfNumber)dictionary[key2]!).Value);
    }

    [Fact]
    public void Test_Indexer_Get_ReturnsCorrectValue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        var value = new PdfNumber(42);
        dictionary.Add(key, value);
        
        var result = dictionary[key];
        
        Assert.Equal(42, ((PdfNumber)result!).Value);
    }

    [Fact]
    public void Test_Indexer_Set_UpdatesValue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        var value = new PdfNumber(42);
        
        dictionary[key] = value;
        
        Assert.Equal(1, dictionary.Count);
        Assert.Equal(42, ((PdfNumber)dictionary[key]!).Value);
    }

    [Fact]
    public void Test_Indexer_Set_Null_RemovesEntry()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        dictionary.Add(key, new PdfNumber(42));
        
        dictionary[key] = null;
        
        Assert.Equal(0, dictionary.Count);
        Assert.False(dictionary.ContainsKey(key));
    }

    [Fact]
    public void Test_Indexer_Set_NullKey_ThrowsArgumentNullException()
    {
        var dictionary = new PdfDictionary();
        
        Assert.Throws<ArgumentNullException>(() => dictionary[null!] = new PdfNumber(42));
    }

    [Fact]
    public void Test_Add_ValidKeyValue_AddsEntry()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        var value = new PdfNumber(42);
        
        dictionary.Add(key, value);
        
        Assert.Equal(1, dictionary.Count);
        Assert.True(dictionary.ContainsKey(key));
        Assert.Equal(42, ((PdfNumber)dictionary[key]!).Value);
    }

    [Fact]
    public void Test_Add_NullKey_ThrowsArgumentNullException()
    {
        var dictionary = new PdfDictionary();
        
        Assert.Throws<ArgumentNullException>(() => dictionary.Add(null!, new PdfNumber(42)));
    }

    [Fact]
    public void Test_Add_NullValue_ThrowsArgumentNullException()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        
        Assert.Throws<ArgumentNullException>(() => dictionary.Add(key, null!));
    }

    [Fact]
    public void Test_Add_DuplicateKey_ThrowsPdfException()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        dictionary.Add(key, new PdfNumber(1));
        
        var exception = Assert.Throws<PdfException>(() => dictionary.Add(key, new PdfNumber(2)));
        Assert.Contains("already contains an entry with key", exception.Message);
    }

    [Fact]
    public void Test_ContainsKey_ExistingKey_ReturnsTrue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        dictionary.Add(key, new PdfNumber(42));
        
        Assert.True(dictionary.ContainsKey(key));
    }

    [Fact]
    public void Test_ContainsKey_NonExistingKey_ReturnsFalse()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        
        Assert.False(dictionary.ContainsKey(key));
    }

    [Fact]
    public void Test_Remove_ExistingKey_RemovesAndReturnsTrue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        dictionary.Add(key, new PdfNumber(42));
        
        bool removed = dictionary.Remove(key);
        
        Assert.True(removed);
        Assert.Equal(0, dictionary.Count);
        Assert.False(dictionary.ContainsKey(key));
    }

    [Fact]
    public void Test_Remove_NonExistingKey_ReturnsFalse()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        
        bool removed = dictionary.Remove(key);
        
        Assert.False(removed);
    }

    [Fact]
    public void Test_Clear_RemovesAllEntries()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfName.Get("Key1"), new PdfNumber(1));
        dictionary.Add(PdfName.Get("Key2"), new PdfNumber(2));
        
        dictionary.Clear();
        
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Test_TryGetValue_ExistingKey_ReturnsTrueAndValue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        var expectedValue = new PdfNumber(42);
        dictionary.Add(key, expectedValue);
        
        bool found = dictionary.TryGetValue(key, out var value);
        
        Assert.True(found);
        Assert.Equal(42, ((PdfNumber)value!).Value);
    }

    [Fact]
    public void Test_TryGetValue_NonExistingKey_ReturnsFalseAndNull()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        
        bool found = dictionary.TryGetValue(key, out var value);
        
        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void Test_TryGetValue_Generic_CorrectType_ReturnsTrueAndValue()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        dictionary.Add(key, new PdfNumber(42));
        
        bool found = dictionary.TryGetValue<PdfNumber>(key, out var value);
        
        Assert.True(found);
        Assert.Equal(42, value!.Value);
    }

    [Fact]
    public void Test_TryGetValue_Generic_NonExistingKey_ReturnsFalseAndDefault()
    {
        var dictionary = new PdfDictionary();
        var key = PdfName.Get("TestKey");
        
        bool found = dictionary.TryGetValue<PdfNumber>(key, out var value);
        
        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void Test_Keys_ReturnsAllKeys()
    {
        var dictionary = new PdfDictionary();
        var key1 = PdfName.Get("Key1");
        var key2 = PdfName.Get("Key2");
        dictionary.Add(key1, new PdfNumber(1));
        dictionary.Add(key2, new PdfNumber(2));
        
        var keys = dictionary.Keys;
        
        Assert.Equal(2, keys.Count);
        Assert.Contains(key1, keys);
        Assert.Contains(key2, keys);
    }

    [Fact]
    public void Test_GetEnumerator_IteratesAllEntries()
    {
        var dictionary = new PdfDictionary();
        var key1 = PdfName.Get("Key1");
        var key2 = PdfName.Get("Key2");
        dictionary.Add(key1, new PdfNumber(1));
        dictionary.Add(key2, new PdfNumber(2));
        
        var results = new Dictionary<PdfName, long>();
        foreach (var kvp in dictionary)
        {
            results[kvp.Key] = (PdfNumber)kvp.Value;
        }
        
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[key1]);
        Assert.Equal(2, results[key2]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(50)]
    public void Test_Count_ReturnsCorrectCount(int entryCount)
    {
        var dictionary = new PdfDictionary();
        for (int i = 0; i < entryCount; i++)
        {
            dictionary.Add(PdfName.Get($"Key{i}"), new PdfNumber(i));
        }
        
        Assert.Equal(entryCount, dictionary.Count);
    }

    [Fact]
    public void Test_Dictionary_WithMixedTypes_HandlesCorrectly()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfName.Get("Integer"), new PdfNumber(42));
        dictionary.Add(PdfName.Get("Real"), new PdfNumber(3.14));
        dictionary.Add(PdfName.Get("Boolean"), PdfBoolean.True);
        dictionary.Add(PdfName.Get("String"), CreatePdfString("test", PdfStringEncoding.PdfDocEncoding, false));
        
        Assert.Equal(4, dictionary.Count);
        Assert.IsType<PdfNumber>(dictionary[PdfName.Get("Integer")]);
        Assert.IsType<PdfNumber>(dictionary[PdfName.Get("Real")]);
        Assert.IsType<PdfBoolean>(dictionary[PdfName.Get("Boolean")]);
        Assert.IsType<PdfString>(dictionary[PdfName.Get("String")]);
    }

    [Fact]
    public void Test_Dictionary_LargeData_HandlesCorrectly()
    {
        var dictionary = new PdfDictionary();
        for (int i = 0; i < 1000; i++)
        {
            dictionary.Add(PdfName.Get($"Key{i}"), new PdfNumber(i));
        }
        
        Assert.Equal(1000, dictionary.Count);
        Assert.Equal(0, ((PdfNumber)dictionary[PdfName.Get("Key0")]!).Value);
        Assert.Equal(999, ((PdfNumber)dictionary[PdfName.Get("Key999")]!).Value);
    }
}
