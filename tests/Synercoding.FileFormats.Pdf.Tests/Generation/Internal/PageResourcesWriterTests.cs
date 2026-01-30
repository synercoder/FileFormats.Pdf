using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class PageResourcesWriterTests
{
    [Fact]
    public void Test_Write_EmptyPageResources_ReturnsEmptyDictionary()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PdfDictionary>(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Test_Write_WithImages_CreatesXObjectDictionary()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        // Create a test image
        var imageId = tableBuilder.ReserveId();
        var testImage = new PdfImage(
            imageId,
            new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }), // Mock JPEG data
            100,
            100,
            DeviceRGB.Instance,
            null,
            null,
            (PdfNames.DCTDecode, null)
        );

        // Add the image through reflection (since Add returns the name)
        var imageName = pageResources.Add(testImage);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(PdfNames.XObject));

        var xObjectDict = result[PdfNames.XObject] as IPdfDictionary;
        Assert.NotNull(xObjectDict);
        Assert.Single(xObjectDict);
        Assert.True(xObjectDict.ContainsKey(imageName));

        var imageRef = xObjectDict[imageName];
        Assert.IsType<PdfReference>(imageRef);
        Assert.Equal(imageId, ( (PdfReference)imageRef ).Id);
    }

    [Fact]
    public void Test_Write_WithImagesAndSoftMask_WritesImageAndMask()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        // Create a test image with soft mask
        var imageId = tableBuilder.ReserveId();
        var softMaskId = tableBuilder.ReserveId();
        var softMask = new PdfImage(
            softMaskId,
            new MemoryStream(new byte[] { 0x00, 0xFF }),
            100,
            100,
            DeviceGray.Instance,
            null,
            null,
            (PdfNames.FlateDecode, null)
        );

        var testImage = new PdfImage(
            imageId,
            new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }),
            100,
            100,
            DeviceRGB.Instance,
            softMask,
            null,
            (PdfNames.DCTDecode, null)
        );

        var imageName = pageResources.Add(testImage);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(PdfNames.XObject));

        // Verify the XObject dictionary contains the image
        var xObjectDict = result[PdfNames.XObject] as IPdfDictionary;
        Assert.NotNull(xObjectDict);
        Assert.Single(xObjectDict);
        Assert.True(xObjectDict.ContainsKey(imageName));
    }

    [Fact]
    public void Test_Write_WithColorSpaces_CreatesColorSpaceDictionary()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        var separation = new Separation(PdfName.Get("TestSpot"), new CmykColor(1f, 0f, 0f, 0f));

        var sepName = pageResources.Add(separation);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(PdfNames.ColorSpace));

        var colorSpaceDict = result[PdfNames.ColorSpace] as IPdfDictionary;
        Assert.NotNull(colorSpaceDict);
        Assert.Single(colorSpaceDict);
        Assert.True(colorSpaceDict.ContainsKey(sepName));

        var sepRef = colorSpaceDict[sepName];
        Assert.IsType<PdfReference>(sepRef);
    }

    [Fact]
    public void Test_Write_WithExtendedGraphicsStates_CreatesExtGStateDictionary()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        var extGState = new ExtendedGraphicsState();
        extGState = extGState with { OverprintNonStroking = true };

        var stateName = pageResources.Add(extGState);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(PdfNames.ExtGState));

        var extGStateDict = result[PdfNames.ExtGState] as IPdfDictionary;
        Assert.NotNull(extGStateDict);
        Assert.Single(extGStateDict);
        Assert.True(extGStateDict.ContainsKey(stateName));

        var stateRef = extGStateDict[stateName];
        Assert.IsType<PdfReference>(stateRef);
    }

    [Fact]
    public void Test_Write_WithAllResourceTypes_CreatesCompleteResourcesDictionary()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        // Add image
        var imageId = tableBuilder.ReserveId();
        var testImage = new PdfImage(
            imageId,
            new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }),
            100,
            100,
            DeviceRGB.Instance,
            null,
            null,
            (PdfNames.DCTDecode, null)
        );
        var imageName = pageResources.Add(testImage);

        var separation = new Separation(PdfName.Get("TestSpot"), new CmykColor(1f, 0f, 0f, 0f));
        var sepName = pageResources.Add(separation);

        var extGState = new ExtendedGraphicsState();
        extGState = extGState with { OverprintNonStroking = true };
        var stateName = pageResources.Add(extGState);

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey(PdfNames.XObject));
        Assert.True(result.ContainsKey(PdfNames.ColorSpace));
        Assert.True(result.ContainsKey(PdfNames.ExtGState));

        // Verify XObject dictionary
        var xObjectDict = result[PdfNames.XObject] as IPdfDictionary;
        Assert.NotNull(xObjectDict);
        Assert.Single(xObjectDict);
        Assert.True(xObjectDict.ContainsKey(imageName));

        // Verify ColorSpace dictionary
        var colorSpaceDict = result[PdfNames.ColorSpace] as IPdfDictionary;
        Assert.NotNull(colorSpaceDict);
        Assert.Single(colorSpaceDict);
        Assert.True(colorSpaceDict.ContainsKey(sepName));

        // Verify ExtGState dictionary
        var extGStateDict = result[PdfNames.ExtGState] as IPdfDictionary;
        Assert.NotNull(extGStateDict);
        Assert.Single(extGStateDict);
        Assert.True(extGStateDict.ContainsKey(stateName));

    }

    [Fact]
    public void Test_Write_MultipleImages_WritesAllImages()
    {
        // Arrange
        var writer = _createPageResourcesWriter(out var tableBuilder, out var pageResources);

        // Create multiple test images
        var imageNames = new List<PdfName>();
        for (int i = 0; i < 3; i++)
        {
            var imageId = tableBuilder.ReserveId();
            var testImage = new PdfImage(
                imageId,
                new MemoryStream([0xFF, 0xD8, (byte)( 0xFF + i )]),
                100 + ( i * 10 ),
                100 + ( i * 10 ),
                DeviceRGB.Instance,
                null,
                null,
                (PdfNames.DCTDecode, null)
            );
            imageNames.Add(pageResources.Add(testImage));
        }

        // Act
        var result = writer.Write(pageResources);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey(PdfNames.XObject));

        var xObjectDict = result[PdfNames.XObject] as IPdfDictionary;
        Assert.NotNull(xObjectDict);
        Assert.Equal(3, xObjectDict.Count);

        foreach (var imageName in imageNames)
        {
            Assert.True(xObjectDict.ContainsKey(imageName));
        }

    }

    private PageResourcesWriter _createPageResourcesWriter(out TableBuilder sharedTableBuilder, out PageResources pageResources)
    {
        sharedTableBuilder = new TableBuilder();
        var stream = new PdfStream(new MemoryStream());
        var objectWriter = new ObjectWriter(sharedTableBuilder, stream, 0);
        var cachedResources = new CachedResources(sharedTableBuilder);
        pageResources = new PageResources(sharedTableBuilder, cachedResources);

        return new PageResourcesWriter(objectWriter, cachedResources);
    }
}
