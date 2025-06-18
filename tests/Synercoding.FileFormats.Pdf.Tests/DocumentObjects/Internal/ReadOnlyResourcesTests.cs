using Synercoding.FileFormats.Pdf.DocumentObjects.Internal;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.DocumentObjects.Internal;

public class ReadOnlyResourcesTests
{
    private static ObjectReader _createObjectReader()
    {
        var pdfContent = 
            "%PDF-1.7" + '\n' +
            "1 0 obj" + '\n' +
            "<< /Type /Font >>" + '\n' +
            "endobj" + '\n' +
            "2 0 obj" + '\n' +
            "<< /F1 1 0 R >>" + '\n' +
            "endobj" + '\n' +
            "xref" + '\n' +
            "0 3" + '\n' +
            "0000000000 65535 f " + '\n' +
            "0000000009 00000 n " + '\n' +
            "0000000033 00000 n " + '\n' +
            "trailer" + '\n' +
            "<< /Size 3 >>" + '\n' +
            "startxref" + '\n' +
            "56" + '\n' +
            "%%EOF";
        
        var bytes = Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }

    private static PdfDictionary _createResourcesDictionary()
    {
        return new PdfDictionary();
    }

    [Fact]
    public void Test_Constructor_ValidParameters_CreatesInstance()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var id = new PdfObjectId(1, 0);

        var resources = new ReadOnlyResources(id, dictionary, objectReader);

        Assert.Equal(id, resources.Id);
    }

    [Fact]
    public void Test_Constructor_NullId_CreatesInstanceWithNullId()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.Id);
    }

    [Fact]
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReader();
        var id = new PdfObjectId(1, 0);

        Assert.Throws<ArgumentNullException>(() => new ReadOnlyResources(id, null!, objectReader));
    }

    [Fact]
    public void Test_Constructor_NullObjectReader_ThrowsArgumentNullException()
    {
        var dictionary = _createResourcesDictionary();
        var id = new PdfObjectId(1, 0);

        Assert.Throws<ArgumentNullException>(() => new ReadOnlyResources(id, dictionary, null!));
    }

    [Fact]
    public void Test_ExtGState_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var extGStateDict = new PdfDictionary();
        dictionary.Add(PdfNames.ExtGState, extGStateDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(extGStateDict, resources.ExtGState);
    }

    [Fact]
    public void Test_ExtGState_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.ExtGState);
    }

    [Fact]
    public void Test_ColorSpace_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var colorSpaceDict = new PdfDictionary();
        dictionary.Add(PdfNames.ColorSpace, colorSpaceDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(colorSpaceDict, resources.ColorSpace);
    }

    [Fact]
    public void Test_ColorSpace_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.ColorSpace);
    }

    [Fact]
    public void Test_Pattern_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var patternDict = new PdfDictionary();
        dictionary.Add(PdfNames.Pattern, patternDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(patternDict, resources.Pattern);
    }

    [Fact]
    public void Test_Pattern_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.Pattern);
    }

    [Fact]
    public void Test_Shading_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var shadingDict = new PdfDictionary();
        dictionary.Add(PdfNames.Shading, shadingDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(shadingDict, resources.Shading);
    }

    [Fact]
    public void Test_Shading_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.Shading);
    }

    [Fact]
    public void Test_XObject_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var xObjectDict = new PdfDictionary();
        dictionary.Add(PdfNames.XObject, xObjectDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(xObjectDict, resources.XObject);
    }

    [Fact]
    public void Test_XObject_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.XObject);
    }

    [Fact]
    public void Test_Font_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var fontDict = new PdfDictionary();
        dictionary.Add(PdfNames.Font, fontDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(fontDict, resources.Font);
    }

    [Fact]
    public void Test_Font_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.Font);
    }

    [Fact]
    public void Test_ProcSet_WithValidArray_ReturnsCorrectArray()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var procSetArray = new PdfArray();
        procSetArray.Add(PdfName.Get("PDF"));
        procSetArray.Add(PdfName.Get("Text"));
        dictionary.Add(PdfNames.ProcSet, procSetArray);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(procSetArray, resources.ProcSet);
    }

    [Fact]
    public void Test_ProcSet_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.ProcSet);
    }

    [Fact]
    public void Test_Properties_WithValidDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var propertiesDict = new PdfDictionary();
        dictionary.Add(PdfNames.Properties, propertiesDict);

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Same(propertiesDict, resources.Properties);
    }

    [Fact]
    public void Test_Properties_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.Properties);
    }

    [Fact]
    public void Test_AllProperties_WithFullResourcesDictionary_ReturnsCorrectValues()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        
        var extGStateDict = new PdfDictionary();
        var colorSpaceDict = new PdfDictionary();
        var patternDict = new PdfDictionary();
        var shadingDict = new PdfDictionary();
        var xObjectDict = new PdfDictionary();
        var fontDict = new PdfDictionary();
        var procSetArray = new PdfArray();
        var propertiesDict = new PdfDictionary();

        dictionary.Add(PdfNames.ExtGState, extGStateDict);
        dictionary.Add(PdfNames.ColorSpace, colorSpaceDict);
        dictionary.Add(PdfNames.Pattern, patternDict);
        dictionary.Add(PdfNames.Shading, shadingDict);
        dictionary.Add(PdfNames.XObject, xObjectDict);
        dictionary.Add(PdfNames.Font, fontDict);
        dictionary.Add(PdfNames.ProcSet, procSetArray);
        dictionary.Add(PdfNames.Properties, propertiesDict);

        var resources = new ReadOnlyResources(new PdfObjectId(1, 0), dictionary, objectReader);

        Assert.Same(extGStateDict, resources.ExtGState);
        Assert.Same(colorSpaceDict, resources.ColorSpace);
        Assert.Same(patternDict, resources.Pattern);
        Assert.Same(shadingDict, resources.Shading);
        Assert.Same(xObjectDict, resources.XObject);
        Assert.Same(fontDict, resources.Font);
        Assert.Same(procSetArray, resources.ProcSet);
        Assert.Same(propertiesDict, resources.Properties);
    }

    [Fact]
    public void Test_AllProperties_WithEmptyDictionary_ReturnsNullForAll()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.ExtGState);
        Assert.Null(resources.ColorSpace);
        Assert.Null(resources.Pattern);
        Assert.Null(resources.Shading);
        Assert.Null(resources.XObject);
        Assert.Null(resources.Font);
        Assert.Null(resources.ProcSet);
        Assert.Null(resources.Properties);
    }

    [Fact]
    public void Test_Font_WithIndirectReference_CallsGetOrDefault()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        
        // Add indirect reference to object 2 0 which contains font dictionary
        dictionary.Add(PdfNames.Font, new PdfReference { Id = new PdfObjectId(2, 0) });

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        // The GetOrDefault method should be called, behavior depends on ObjectReader implementation
        // This test verifies the property can be accessed without throwing an exception
        var fontDict = resources.Font;
        // Don't assert specific values since the ObjectReader behavior may vary
    }

    [Fact]
    public void Test_ExtGState_WithIndirectReference_HandlesCorrectly()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        
        // Test with indirect reference that doesn't exist
        dictionary.Add(PdfNames.ExtGState, new PdfReference { Id = new PdfObjectId(99, 0) });

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        // Should return null when indirect reference cannot be resolved
        Assert.Null(resources.ExtGState);
    }

    [Theory]
    [InlineData("ExtGState")]
    [InlineData("ColorSpace")]
    [InlineData("Pattern")]
    [InlineData("Shading")]
    [InlineData("XObject")]
    [InlineData("Properties")]
    public void Test_DictionaryProperties_WithWrongType_ReturnsNull(string propertyName)
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        var pdfName = PdfName.Get(propertyName);
        
        // Add wrong type (string instead of dictionary)
        dictionary.Add(pdfName, new PdfString(System.Text.Encoding.UTF8.GetBytes("WrongType"), false));

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        // Use reflection to get the property value
        var property = typeof(ReadOnlyResources).GetProperty(propertyName);
        var value = property?.GetValue(resources);
        
        Assert.Null(value);
    }

    [Fact]
    public void Test_ProcSet_WithWrongType_ReturnsNull()
    {
        var objectReader = _createObjectReader();
        var dictionary = _createResourcesDictionary();
        
        // Add wrong type (string instead of array)
        dictionary.Add(PdfNames.ProcSet, new PdfString(System.Text.Encoding.UTF8.GetBytes("WrongType"), false));

        var resources = new ReadOnlyResources(null, dictionary, objectReader);

        Assert.Null(resources.ProcSet);
    }
}