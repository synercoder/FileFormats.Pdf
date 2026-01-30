using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Primitives;
using static Synercoding.FileFormats.Pdf.Primitives.PdfNames;

namespace Synercoding.FileFormats.Pdf.Tests.Generation;

public class DirectObjectSerializerTests
{
    [Fact]
    public void Test_Constructor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DirectObjectSerializer(null!));
    }

    [Fact]
    public void Test_WriteDirect_NullPrimitive_ThrowsArgumentNullException()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        Assert.Throws<ArgumentNullException>(() => serializer.WriteDirect((IPdfPrimitive)null!));
    }

    [Fact]
    public void Test_WriteDirect_StreamObject_ThrowsArgumentException()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var streamObject = new Synercoding.FileFormats.Pdf.Primitives.Internal.ReadOnlyPdfStreamObject(
            new PdfDictionary { [Length] = new PdfNumber(2) },
            new byte[] { 0x01, 0x02 }
        );

        // Ensure the stream object implements IPdfStreamObject
        Assert.IsAssignableFrom<IPdfStreamObject>(streamObject);

        var ex = Assert.Throws<ArgumentException>(() => serializer.WriteDirect((IPdfPrimitive)streamObject));
        Assert.Contains("streams can not be written as direct objects", ex.Message);
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Test_WriteDirect_Boolean(bool value, string expected)
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        serializer.WriteDirect(new PdfBoolean(value));

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_WriteDirect_Null()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        serializer.WriteDirect(PdfNull.INSTANCE);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("null", result);
    }

    [Theory]
    [InlineData(42, "42")]
    [InlineData(-42, "-42")]
    [InlineData(0, "0")]
    [InlineData(2147483647, "2147483647")]
    public void Test_WriteDirect_IntegerNumber(long value, string expected)
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        serializer.WriteDirect(new PdfNumber(value));

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(3.14, "3.14")]
    [InlineData(-3.14, "-3.14")]
    [InlineData(0.0, "0.0")]
    [InlineData(1.23456789, "1.23456789")]
    public void Test_WriteDirect_FractionalNumber(double value, string expected)
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        serializer.WriteDirect(new PdfNumber(value));

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_WriteDirect_Name()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        serializer.WriteDirect(PdfName.Get("TestName"));

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("/TestName", result);
    }

    [Fact]
    public void Test_WriteDirect_Name_WithSpecialCharacters()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var name = PdfName.Get("Name With Spaces");
        serializer.WriteDirect(name);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        // PdfName escapes spaces as #20
        Assert.Equal("/Name#20With#20Spaces", result);
    }

    [Fact]
    public void Test_WriteDirect_LiteralString()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var pdfString = new PdfString(System.Text.Encoding.ASCII.GetBytes("Hello World"), false);
        serializer.WriteDirect(pdfString);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("(Hello World)", result);
    }

    [Fact]
    public void Test_WriteDirect_HexString()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var hexBytes = Convert.FromHexString("48656C6C6F");
        var pdfString = new PdfString(hexBytes, true);
        serializer.WriteDirect(pdfString);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("<48656C6C6F>", result);
    }

    [Fact]
    public void Test_WriteDirect_Reference()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var reference = new PdfReference(new PdfObjectId(12, 0));
        serializer.WriteDirect(reference);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("12 0 R", result);
    }

    [Fact]
    public void Test_WriteDirect_Reference_WithGeneration()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var reference = new PdfReference(new PdfObjectId(5, 2));
        serializer.WriteDirect(reference);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("5 2 R", result);
    }

    [Fact]
    public void Test_WriteDirect_EmptyArray()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray();
        serializer.WriteDirect(array);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("[]", result);
    }

    [Fact]
    public void Test_WriteDirect_Array_WithNumbers()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray
        {
            new PdfNumber(1),
            new PdfNumber(2),
            new PdfNumber(3)
        };
        serializer.WriteDirect(array);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("[1 2 3]", result);
    }

    [Fact]
    public void Test_WriteDirect_Array_WithMixedTypes()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray
        {
            new PdfNumber(42),
            PdfName.Get("Name"),
            new PdfBoolean(true),
            PdfNull.INSTANCE,
            new PdfString(System.Text.Encoding.ASCII.GetBytes("test"), false)
        };
        serializer.WriteDirect(array);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("[42/Name true null(test)]", result);
    }

    [Fact]
    public void Test_WriteDirect_Array_WithReferences()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray
        {
            new PdfReference(new PdfObjectId(1, 0)),
            new PdfReference(new PdfObjectId(2, 0)),
            new PdfReference(new PdfObjectId(3, 0))
        };
        serializer.WriteDirect(array);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("[1 0 R 2 0 R 3 0 R]", result);
    }

    [Fact]
    public void Test_WriteDirect_NestedArray()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var innerArray = new PdfArray { new PdfNumber(1), new PdfNumber(2) };
        var outerArray = new PdfArray { innerArray, new PdfNumber(3) };
        serializer.WriteDirect(outerArray);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("[[1 2] 3]", result);
    }

    [Fact]
    public void Test_WriteDirect_EmptyDictionary()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var dict = new PdfDictionary();
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("<<>>", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_WithSingleEntry()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var dict = new PdfDictionary
        {
            [PdfName.Get("Type")] = PdfName.Get("Catalog")
        };
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("<</Type/Catalog>>", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_WithMultipleEntries()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var dict = new PdfDictionary
        {
            [PdfName.Get("Type")] = PdfName.Get("Page"),
            [PdfName.Get("Count")] = new PdfNumber(10),
            [PdfName.Get("Parent")] = new PdfReference(new PdfObjectId(1, 0))
        };
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Type/Page", result);
        Assert.Contains("/Count 10", result);
        Assert.Contains("/Parent 1 0 R", result);
        Assert.StartsWith("<<", result);
        Assert.EndsWith(">>", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_WithArrayValue()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray { new PdfNumber(0), new PdfNumber(0), new PdfNumber(612), new PdfNumber(792) };
        var dict = new PdfDictionary
        {
            [PdfName.Get("MediaBox")] = array
        };
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("<</MediaBox[0 0 612 792]>>", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_NestedDictionary()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var innerDict = new PdfDictionary
        {
            [PdfName.Get("SubType")] = PdfName.Get("Widget")
        };
        var outerDict = new PdfDictionary
        {
            [PdfName.Get("Type")] = PdfName.Get("Annot"),
            [PdfName.Get("AP")] = innerDict
        };
        serializer.WriteDirect(outerDict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Type/Annot", result);
        Assert.Contains("/AP<</SubType/Widget>>", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_WithBooleanValue()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var dict = new PdfDictionary
        {
            [PdfName.Get("Visible")] = new PdfBoolean(true),
            [PdfName.Get("Hidden")] = new PdfBoolean(false)
        };
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Visible true", result);
        Assert.Contains("/Hidden false", result);
    }

    [Fact]
    public void Test_WriteDirect_Dictionary_WithNullValue()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var dict = new PdfDictionary
        {
            [PdfName.Get("Optional")] = PdfNull.INSTANCE
        };
        serializer.WriteDirect(dict);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("<</Optional null>>", result);
    }

    [Fact]
    public void Test_WriteDirect_ComplexObject()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var resources = new PdfDictionary
        {
            [PdfName.Get("Font")] = new PdfDictionary
            {
                [PdfName.Get("F1")] = new PdfReference(new PdfObjectId(5, 0))
            },
            [PdfName.Get("ProcSet")] = new PdfArray
            {
                PdfName.Get("PDF"),
                PdfName.Get("Text")
            }
        };

        var page = new PdfDictionary
        {
            [PdfName.Get("Type")] = PdfName.Get("Page"),
            [PdfName.Get("Parent")] = new PdfReference(new PdfObjectId(3, 0)),
            [PdfName.Get("Resources")] = resources,
            [PdfName.Get("MediaBox")] = new PdfArray
            {
                new PdfNumber(0),
                new PdfNumber(0),
                new PdfNumber(612),
                new PdfNumber(792)
            },
            [PdfName.Get("Contents")] = new PdfReference(new PdfObjectId(4, 0))
        };

        serializer.WriteDirect(page);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        Assert.Contains("/Type/Page", result);
        Assert.Contains("/Parent 3 0 R", result);
        Assert.Contains("/Resources", result);
        Assert.Contains("/Font", result);
        Assert.Contains("/F1 5 0 R", result);
        Assert.Contains("/ProcSet[/PDF/Text]", result);
        Assert.Contains("/MediaBox[0 0 612 792]", result);
        Assert.Contains("/Contents 4 0 R", result);
    }

    [Fact]
    public void Test_WriteDirect_LargeArray()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var array = new PdfArray();
        for (int i = 0; i < 100; i++)
        {
            array.Add(new PdfNumber(i));
        }

        serializer.WriteDirect(array);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("[", result);
        Assert.EndsWith("]", result);
        Assert.Contains("0 1 2 3 4 5", result);
        Assert.Contains("95 96 97 98 99", result);
    }

    [Fact]
    public void Test_WriteDirect_SpecialCharactersInString()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var pdfString = new PdfString(new byte[] {
            (byte)'(', (byte)')', (byte)'\\'
        }, false);
        serializer.WriteDirect(pdfString);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.Equal("(()\u005c)", result);
    }

    [Fact]
    public void Test_WriteDirect_VeryLongHexString()
    {
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var serializer = new DirectObjectSerializer(pdfStream);

        var hexData = string.Concat(Enumerable.Repeat("FF", 256));
        var hexBytes = Convert.FromHexString(hexData);
        var pdfString = new PdfString(hexBytes, true);
        serializer.WriteDirect(pdfString);

        var result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("<", result);
        Assert.EndsWith(">", result);
        Assert.Contains(hexData, result);
    }
}
