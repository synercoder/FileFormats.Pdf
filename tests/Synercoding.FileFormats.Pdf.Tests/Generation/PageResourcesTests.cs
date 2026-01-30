using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Generation;

public class PageResourcesTests : IDisposable
{
    private readonly TableBuilder _tableBuilder;
    private readonly CachedResources _cachedResources;
    private readonly PageResources _pageResources;
    private readonly List<IDisposable> _disposables = new();

    public PageResourcesTests()
    {
        _tableBuilder = new TableBuilder();
        _cachedResources = new CachedResources(_tableBuilder);
        _pageResources = new PageResources(_tableBuilder, _cachedResources);
    }

    public void Dispose()
    {
        _pageResources?.Dispose();
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
    }

    [Fact]
    public void Test_Constructor_InitializesProperties()
    {
        // Assert
        // PdfObjectId is a struct, no need to check for null
        Assert.NotNull(_pageResources.Images);
        Assert.NotNull(_pageResources.ExtendedGraphicsStates);
        Assert.Empty(_pageResources.Images);
        Assert.Empty(_pageResources.ExtendedGraphicsStates);
    }

    [Fact]
    public void Test_Add_Separation_ReturnsUniqueName()
    {
        // Arrange
        var separation = new Separation(PdfName.Get("TestSeparation"), new GrayColor(0.5));

        // Act
        var name = _pageResources.Add(separation);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("Sep", name.Display);
        Assert.Single(_pageResources.SeparationReferences);
        Assert.True(_pageResources.SeparationReferences.ContainsKey(separation));
    }

    [Fact]
    public void Test_Add_Separation_SameSeparationMultipleTimes_ReturnsSameName()
    {
        // Arrange
        var separation = new Separation(PdfName.Get("TestSeparation"), new RgbColor(1.0, 0.0, 0.0));

        // Act
        var name1 = _pageResources.Add(separation);
        var name2 = _pageResources.Add(separation);
        var name3 = _pageResources.Add(separation);

        // Assert
        Assert.Equal(name1, name2);
        Assert.Equal(name2, name3);
        Assert.Single(_pageResources.SeparationReferences);
    }

    [Fact]
    public void Test_Add_MultipleDifferentSeparations()
    {
        // Arrange
        var separation1 = new Separation(PdfName.Get("Sep1"), new GrayColor(0.5));
        var separation2 = new Separation(PdfName.Get("Sep2"), new RgbColor(1.0, 0.0, 0.0));
        var separation3 = new Separation(PdfName.Get("Sep3"), new CmykColor(0.0, 1.0, 1.0, 0.0));

        // Act
        var name1 = _pageResources.Add(separation1);
        var name2 = _pageResources.Add(separation2);
        var name3 = _pageResources.Add(separation3);

        // Assert
        Assert.NotEqual(name1, name2);
        Assert.NotEqual(name2, name3);
        Assert.NotEqual(name1, name3);
        Assert.Equal(3, _pageResources.SeparationReferences.Count);
        Assert.StartsWith("Sep", name1.Display);
        Assert.StartsWith("Sep", name2.Display);
        Assert.StartsWith("Sep", name3.Display);
    }

    [Fact]
    public void Test_Add_ExtendedGraphicsState_ReturnsUniqueName()
    {
        // Arrange
        var state = new ExtendedGraphicsState { Overprint = true };

        // Act
        var name = _pageResources.Add(state);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("ExGs", name.Display);
        Assert.Single(_pageResources.ExtendedGraphicsStates);
        Assert.True(_pageResources.ExtendedGraphicsStates.ContainsKey(state));
    }

    [Fact]
    public void Test_Add_ExtendedGraphicsState_SameStateMultipleTimes_ReturnsSameName()
    {
        // Arrange
        var state = new ExtendedGraphicsState { Overprint = false, OverprintNonStroking = true };

        // Act
        var name1 = _pageResources.Add(state);
        var name2 = _pageResources.Add(state);
        var name3 = _pageResources.Add(state);

        // Assert
        Assert.Equal(name1, name2);
        Assert.Equal(name2, name3);
        Assert.Single(_pageResources.ExtendedGraphicsStates);
    }

    [Fact]
    public void Test_Add_MultipleDifferentExtendedGraphicsStates()
    {
        // Arrange
        var state1 = new ExtendedGraphicsState { Overprint = true };
        var state2 = new ExtendedGraphicsState { OverprintNonStroking = true };
        var state3 = new ExtendedGraphicsState { Overprint = false, OverprintNonStroking = false };

        // Act
        var name1 = _pageResources.Add(state1);
        var name2 = _pageResources.Add(state2);
        var name3 = _pageResources.Add(state3);

        // Assert
        Assert.NotEqual(name1, name2);
        Assert.NotEqual(name2, name3);
        Assert.NotEqual(name1, name3);
        Assert.Equal(3, _pageResources.ExtendedGraphicsStates.Count);
        Assert.StartsWith("ExGs", name1.Display);
        Assert.StartsWith("ExGs", name2.Display);
        Assert.StartsWith("ExGs", name3.Display);
    }

    [Fact]
    public void Test_Add_PdfImage_ReturnsUniqueName()
    {
        // Arrange
        var id = _tableBuilder.ReserveId();
        var stream = new MemoryStream();
        _disposables.Add(stream);
        var pdfImage = new PdfImage(id, stream, 100, 100, DeviceRGB.Instance, null, null);

        // Act
        var name = _pageResources.Add(pdfImage);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("Im", name.Display);
        Assert.Single(_pageResources.Images);
        Assert.True(_pageResources.Images.ContainsKey(name));
        Assert.Equal(pdfImage, _pageResources.Images[name]);
    }

    [Fact]
    public void Test_Add_PdfImage_SameImageMultipleTimes_ReturnsSameName()
    {
        // Arrange
        var id = _tableBuilder.ReserveId();
        var stream = new MemoryStream();
        _disposables.Add(stream);
        var pdfImage = new PdfImage(id, stream, 100, 100, DeviceGray.Instance, null, null);

        // Act
        var name1 = _pageResources.Add(pdfImage);
        var name2 = _pageResources.Add(pdfImage);
        var name3 = _pageResources.Add(pdfImage);

        // Assert
        Assert.Equal(name1, name2);
        Assert.Equal(name2, name3);
        Assert.Single(_pageResources.Images);
    }

    [Fact]
    public void Test_Add_MultipleDifferentPdfImages()
    {
        // Arrange
        var id1 = _tableBuilder.ReserveId();
        var id2 = _tableBuilder.ReserveId();
        var id3 = _tableBuilder.ReserveId();
        var stream1 = new MemoryStream();
        var stream2 = new MemoryStream();
        var stream3 = new MemoryStream();
        _disposables.Add(stream1);
        _disposables.Add(stream2);
        _disposables.Add(stream3);
        var pdfImage1 = new PdfImage(id1, stream1, 100, 100, DeviceRGB.Instance, null, null);
        var pdfImage2 = new PdfImage(id2, stream2, 200, 200, DeviceGray.Instance, null, null);
        var pdfImage3 = new PdfImage(id3, stream3, 300, 300, DeviceCMYK.Instance, null, null);

        // Act
        var name1 = _pageResources.Add(pdfImage1);
        var name2 = _pageResources.Add(pdfImage2);
        var name3 = _pageResources.Add(pdfImage3);

        // Assert
        Assert.NotEqual(name1, name2);
        Assert.NotEqual(name2, name3);
        Assert.NotEqual(name1, name3);
        Assert.Equal(3, _pageResources.Images.Count);
        Assert.StartsWith("Im", name1.Display);
        Assert.StartsWith("Im", name2.Display);
        Assert.StartsWith("Im", name3.Display);
    }

    [Fact]
    public void Test_AddJpgUnsafe_CreatesAndAddsImage()
    {
        // Arrange
        var jpgStream = new MemoryStream();
        _disposables.Add(jpgStream);
        var width = 640;
        var height = 480;
        var colorSpace = DeviceRGB.Instance;

        // Act
        var name = _pageResources.AddJpgUnsafe(jpgStream, width, height, colorSpace);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("Im", name.Display);
        Assert.Single(_pageResources.Images);
        var addedImage = _pageResources.Images[name];
        Assert.Equal(width, addedImage.Width);
        Assert.Equal(height, addedImage.Height);
        Assert.Equal(colorSpace, addedImage.ColorSpace);
    }

    [Fact]
    public void Test_Add_ImageSharp_CreatesAndAddsImage()
    {
        // Arrange
        using var image = new Image<Rgba32>(100, 100);
        for (int y = 0; y < 100; y++)
        {
            for (int x = 0; x < 100; x++)
            {
                image[x, y] = new Rgba32(255, 0, 0); // Red
            }
        }

        // Act
        var name = _pageResources.Add(image);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("Im", name.Display);
        Assert.Single(_pageResources.Images);
        var addedImage = _pageResources.Images[name];
        Assert.Equal(100, addedImage.Width);
        Assert.Equal(100, addedImage.Height);
    }

    [Fact]
    public void Test_Add_ImageSharp_WithSeparation()
    {
        // Arrange
        using var image = new Image<Rgba32>(50, 50);
        var separation = new Separation(PdfName.Get("SpotColor"), new RgbColor(0, 1, 0)); // Green

        // Act
        var name = _pageResources.Add(image, separation, GrayScaleMethod.AverageOfRGBChannels);

        // Assert
        Assert.NotNull(name);
        Assert.StartsWith("Im", name.Display);
        Assert.Single(_pageResources.Images);
        var addedImage = _pageResources.Images[name];
        Assert.Equal(50, addedImage.Width);
        Assert.Equal(50, addedImage.Height);
        Assert.Equal(separation, addedImage.ColorSpace);
    }

    [Fact]
    public void Test_Dispose_ClearsImagesAndDisposesTheirStreams()
    {
        // Arrange
        var id1 = _tableBuilder.ReserveId();
        var id2 = _tableBuilder.ReserveId();
        var stream1 = new MemoryStream();
        var stream2 = new MemoryStream();
        var pdfImage1 = new PdfImage(id1, stream1, 100, 100, DeviceRGB.Instance, null, null);
        var pdfImage2 = new PdfImage(id2, stream2, 200, 200, DeviceGray.Instance, null, null);

        var pageResources = new PageResources(_tableBuilder, _cachedResources);
        pageResources.Add(pdfImage1);
        pageResources.Add(pdfImage2);

        Assert.Equal(2, pageResources.Images.Count);

        // Act
        pageResources.Dispose();

        // Assert
        Assert.Empty(pageResources.Images);
    }

    [Fact]
    public void Test_Id_Property_ReturnsValidId()
    {
        // Assert
        // PdfObjectId is a struct, no need to check for null
        Assert.True(_pageResources.Id.ObjectNumber > 0);
    }

    [Fact]
    public void Test_ResourceNaming_CorrectPaddingAndIncrement()
    {
        // Arrange
        var state1 = new ExtendedGraphicsState { Overprint = true };
        var state2 = new ExtendedGraphicsState { OverprintNonStroking = true };
        var separation1 = new Separation(PdfName.Get("Sep1"), new GrayColor(0.5));
        var separation2 = new Separation(PdfName.Get("Sep2"), new RgbColor(1.0, 0.0, 0.0));

        // Act
        var stateName1 = _pageResources.Add(state1);
        var stateName2 = _pageResources.Add(state2);
        var sepName1 = _pageResources.Add(separation1);
        var sepName2 = _pageResources.Add(separation2);

        // Assert
        Assert.Equal("ExGs000001", stateName1.Display);
        Assert.Equal("ExGs000002", stateName2.Display);
        Assert.Equal("Sep000001", sepName1.Display);
        Assert.Equal("Sep000002", sepName2.Display);
    }

    [Fact]
    public void Test_ImagesProperty_IsReadOnly()
    {
        // Arrange
        var id = _tableBuilder.ReserveId();
        var stream = new MemoryStream();
        _disposables.Add(stream);
        var pdfImage = new PdfImage(id, stream, 100, 100, DeviceRGB.Instance, null, null);
        var name = _pageResources.Add(pdfImage);

        // Act
        var images = _pageResources.Images;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyDictionary<PdfName, PdfImage>>(images);
        Assert.Single(images);
        Assert.Equal(pdfImage, images[name]);
    }

    [Fact]
    public void Test_ExtendedGraphicsStatesProperty_IsReadOnly()
    {
        // Arrange
        var state = new ExtendedGraphicsState { Overprint = true };
        var name = _pageResources.Add(state);

        // Act
        var states = _pageResources.ExtendedGraphicsStates;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyDictionary<ExtendedGraphicsState, (PdfName Name, PdfReference Id)>>(states);
        Assert.Single(states);
        Assert.True(states.ContainsKey(state));
        Assert.Equal(name, states[state].Name);
    }
}
