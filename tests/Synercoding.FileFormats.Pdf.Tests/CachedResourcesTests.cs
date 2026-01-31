using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests;

public class CachedResourcesTests
{
    [Fact]
    public void Test_Constructor_InitializesCollections()
    {
        // Arrange
        var tableBuilder = _createTableBuilder();

        // Act
        var cachedResources = new CachedResources(tableBuilder);

        // Assert
        Assert.NotNull(cachedResources.Separations);
        Assert.NotNull(cachedResources.Fonts);
        Assert.Empty(cachedResources.Separations);
        Assert.Empty(cachedResources.Fonts);
    }

    [Fact]
    public void Test_GetOrAdd_Separation_FirstCall_ReturnsNewReference()
    {
        // Arrange
        var tableBuilder = _createTableBuilder();
        var cachedResources = new CachedResources(tableBuilder);
        var separation = new Separation(PdfName.Get("TestSeparation"), new GrayColor(0.5));

        // Act
        var reference = cachedResources.GetOrAdd(separation);

        // Assert
        // PdfReference is a struct, no need to check for null
        Assert.Single(cachedResources.Separations);
        Assert.True(cachedResources.Separations.ContainsKey(separation));
        Assert.Equal(reference, cachedResources.Separations[separation]);
    }

    [Fact]
    public void Test_GetOrAdd_Separation_SubsequentCalls_ReturnsSameReference()
    {
        // Arrange
        var tableBuilder = _createTableBuilder();
        var cachedResources = new CachedResources(tableBuilder);
        var separation = new Separation(PdfName.Get("TestSeparation"), new GrayColor(0.5));

        // Act
        var reference1 = cachedResources.GetOrAdd(separation);
        var reference2 = cachedResources.GetOrAdd(separation);
        var reference3 = cachedResources.GetOrAdd(separation);

        // Assert
        Assert.Equal(reference1, reference2);
        Assert.Equal(reference2, reference3);
        Assert.Single(cachedResources.Separations);
    }

    [Fact]
    public void Test_GetOrAdd_MultipleDifferentSeparations()
    {
        // Arrange
        var tableBuilder = _createTableBuilder();
        var cachedResources = new CachedResources(tableBuilder);
        var separation1 = new Separation(PdfName.Get("Separation1"), new GrayColor(0.5));
        var separation2 = new Separation(PdfName.Get("Separation2"), new RgbColor(1.0, 0.0, 0.0));
        var separation3 = new Separation(PdfName.Get("Separation3"), new CmykColor(0.0, 1.0, 1.0, 0.0));

        // Act
        var reference1 = cachedResources.GetOrAdd(separation1);
        var reference2 = cachedResources.GetOrAdd(separation2);
        var reference3 = cachedResources.GetOrAdd(separation3);

        // Assert
        Assert.NotEqual(reference1, reference2);
        Assert.NotEqual(reference2, reference3);
        Assert.NotEqual(reference1, reference3);
        Assert.Equal(3, cachedResources.Separations.Count);
        Assert.Equal(reference1, cachedResources.Separations[separation1]);
        Assert.Equal(reference2, cachedResources.Separations[separation2]);
        Assert.Equal(reference3, cachedResources.Separations[separation3]);
    }

    private TableBuilder _createTableBuilder()
    {
        return new TableBuilder();
    }
}
