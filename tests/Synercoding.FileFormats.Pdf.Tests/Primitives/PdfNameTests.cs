using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfNameTests
{
    [Theory]
    [InlineData("simple")]
    [InlineData("Name")]
    [InlineData("123")]
    [InlineData("ABC")]
    public void Test_ExplicitCast_FromString_ReturnsCorrectName(string input)
    {
        var name = (PdfName)input;

        Assert.Equal(input, name.Display);
    }

    [Fact]
    public void Test_Equals_SameName_ReturnsTrue()
    {
        var name1 = PdfName.Get("TestName");
        var name2 = PdfName.Get("TestName");

        Assert.True(name1.Equals(name2));
        Assert.True(name1 == name2);
        Assert.False(name1 != name2);
    }

    [Fact]
    public void Test_Equals_DifferentName_ReturnsFalse()
    {
        var name1 = PdfName.Get("TestName1");
        var name2 = PdfName.Get("TestName2");

        Assert.False(name1.Equals(name2));
        Assert.False(name1 == name2);
        Assert.True(name1 != name2);
    }

    [Fact]
    public void Test_Equals_Null_ReturnsFalse()
    {
        var name = PdfName.Get("TestName");
        PdfName? nullName = null;

        Assert.False(name.Equals(nullName));
    }

    [Fact]
    public void Test_Equals_Object_SameName_ReturnsTrue()
    {
        var name1 = PdfName.Get("TestName");
        object name2 = PdfName.Get("TestName");

        Assert.True(name1.Equals(name2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var name = PdfName.Get("TestName");
        object obj = "TestName";

        Assert.False(name.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var name = PdfName.Get("TestName");
        object? obj = null;

        Assert.False(name.Equals(obj));
    }

    [Theory]
    [InlineData("TestName")]
    [InlineData("AnotherName")]
    public void Test_GetHashCode_SameName_ReturnsSameHash(string input)
    {
        var name1 = PdfName.Get(input);
        var name2 = PdfName.Get(input);

        Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentNames_ReturnsDifferentHash()
    {
        var name1 = PdfName.Get("TestName1");
        var name2 = PdfName.Get("TestName2");

        Assert.NotEqual(name1.GetHashCode(), name2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_Consistency()
    {
        var name = PdfName.Get("TestName");

        var hash1 = name.GetHashCode();
        var hash2 = name.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Test_EqualityOperators_Reflexive()
    {
        var name = PdfName.Get("TestName");
        var sameName = name;

        Assert.True(name == sameName);
        Assert.False(name != sameName);
        Assert.True(name.Equals(sameName));
    }

    [Fact]
    public void Test_EqualityOperators_Symmetric()
    {
        var name1 = PdfName.Get("TestName");
        var name2 = PdfName.Get("TestName");

        Assert.True(name1 == name2);
        Assert.True(name2 == name1);
        Assert.True(name1.Equals(name2));
        Assert.True(name2.Equals(name1));
    }

    [Fact]
    public void Test_EqualityOperators_Transitive()
    {
        var name1 = PdfName.Get("TestName");
        var name2 = PdfName.Get("TestName");
        var name3 = PdfName.Get("TestName");

        Assert.True(name1 == name2);
        Assert.True(name2 == name3);
        Assert.True(name1 == name3);
    }

    [Theory]
    [InlineData("simple")]
    [InlineData("Name With Spaces")]
    [InlineData("Name#With#Hash")]
    [InlineData("Name(With)Delimiters")]
    [InlineData("Name<With>Brackets")]
    [InlineData("Name[With]SquareBrackets")]
    [InlineData("Name/With/Slash")]
    [InlineData("Name%With%Percent")]
    public void Test_NameEscaping_SpecialCharacters(string input)
    {
        var name = PdfName.Get(input);

        Assert.Equal(input, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Fact]
    public void Test_NameEscaping_HashCharacter()
    {
        var name = PdfName.Get("Name#Hash");

        Assert.Equal("Name#Hash", name.Display);
        var rawString = Encoding.UTF8.GetString(name.Raw);
        Assert.Contains("#23", rawString);
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\f")]
    [InlineData(" ")]
    public void Test_NameEscaping_WhitespaceCharacters(string whitespace)
    {
        var input = $"Name{whitespace}Test";
        var name = PdfName.Get(input);

        Assert.Equal(input, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Theory]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("/")]
    [InlineData("%")]
    public void Test_NameEscaping_DelimiterCharacters(string delimiter)
    {
        var input = $"Name{delimiter}Test";
        var name = PdfName.Get(input);

        Assert.Equal(input, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Theory]
    [InlineData(31)]  // Below printable ASCII range
    [InlineData(127)] // Above printable ASCII range
    [InlineData(128)] // Extended ASCII
    [InlineData(255)] // High extended ASCII
    public void Test_NameEscaping_NonPrintableCharacters(int charCode)
    {
        var input = $"Name{(char)charCode}Test";
        var name = PdfName.Get(input);

        Assert.Equal(input, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Fact]
    public void Test_Constructor_NullByteThrows()
    {
        Assert.Throws<InvalidOperationException>(() => PdfName.Get("Name\0Test"));
    }

    [Fact]
    public void Test_InternalConstructor_WithBytes()
    {
        var bytes = Encoding.UTF8.GetBytes("TestName");
        var name = new PdfName(bytes);

        Assert.Equal("TestName", name.Display);
        Assert.Equal(bytes, name.Raw);
    }

    [Fact]
    public void Test_InternalConstructor_NullBytes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PdfName((byte[])null!));
    }

    [Theory]
    [InlineData("Name#23")]
    [InlineData("Name#41")]
    [InlineData("Name#5A")]
    public void Test_HexUnescaping_ValidHex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var name = new PdfName(bytes);

        Assert.NotNull(name.Display);
        Assert.Equal(bytes, name.Raw);
    }

    [Theory]
    [InlineData("NameABC")]
    [InlineData("Name123")]
    [InlineData("Name_Test")]
    [InlineData("Name-Test")]
    [InlineData("Name.Test")]
    public void Test_SimpleNames_NoEscaping(string input)
    {
        var name = PdfName.Get(input);

        Assert.Equal(input, name.Display);
        var rawString = Encoding.UTF8.GetString(name.Raw);
        Assert.Equal(input, rawString);
    }

    [Fact]
    public void Test_LongName_HandledCorrectly()
    {
        var longName = new string('A', 1000);
        var name = PdfName.Get(longName);

        Assert.Equal(longName, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Fact]
    public void Test_UnicodeCharacters_HandledCorrectly()
    {
        var unicodeName = "こんにちは";
        var name = PdfName.Get(unicodeName);

        Assert.Equal(unicodeName, name.Display);
        Assert.NotNull(name.Raw);
    }

    [Fact]
    public void Test_NamesInCollections()
    {
        var names = new HashSet<PdfName>
        {
            PdfName.Get("Name1"),
            PdfName.Get("Name2"),
            PdfName.Get("Name1"), // Duplicate
            PdfName.Get("Author"), // Reserved name
            PdfName.Get("Author")  // Reserved name duplicate
        };

        Assert.Equal(3, names.Count);
    }

    [Fact]
    public void Test_ReservedNamesCache_Performance()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 10000; i++)
        {
            var name = PdfName.Get("Author");
        }

        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 100, "Reserved name lookup should be fast");
    }

    [Theory]
    [InlineData("Author", true)]
    [InlineData("NotReserved", false)]
    [InlineData("Type", true)]
    [InlineData("CustomType", false)]
    public void Test_ReservedNameCaching(string input, bool shouldBeCached)
    {
        var name1 = PdfName.Get(input);
        var name2 = PdfName.Get(input);

        if (shouldBeCached)
        {
            Assert.Same(name1, name2);
        }
        else
        {
            Assert.NotSame(name1, name2);
        }
    }

    [Fact]
    public void Test_Display_Property()
    {
        var testName = "TestDisplayName";
        var name = PdfName.Get(testName);

        Assert.Equal(testName, name.Display);
    }

    [Fact]
    public void Test_Raw_Property()
    {
        var testName = "TestRawName";
        var name = PdfName.Get(testName);

        Assert.NotNull(name.Raw);
        Assert.True(name.Raw.Length > 0);
    }

    [Theory]
    [InlineData("Test Name")]
    [InlineData("Test\tName")]
    [InlineData("Test\nName")]
    public void Test_RoundTrip_EscapeUnescape(string input)
    {
        var name = PdfName.Get(input);
        var reconstructed = new PdfName(name.Raw);

        Assert.Equal(input, reconstructed.Display);
        Assert.True(name.Raw.SequenceEqual(reconstructed.Raw));
    }

    [Fact]
    public void Test_Get_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => PdfName.Get(""));
    }

    [Fact]
    public void Test_Get_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PdfName.Get(null!));
    }

    [Fact]
    public void Test_ExplicitCast_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => (PdfName)"");
    }

    [Fact]
    public void Test_ExplicitCast_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PdfName.Get(null!));
    }

    [Fact]
    public void Test_InternalConstructor_EmptyByteArray_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new PdfName(Array.Empty<byte>()));
    }
}
