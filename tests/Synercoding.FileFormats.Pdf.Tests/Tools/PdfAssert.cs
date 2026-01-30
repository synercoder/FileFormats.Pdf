using Pdfium.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Synercoding.FileFormats.Pdf.Tests.Tools;

public static partial class PdfAssert
{
    public static void VisualEquals(string expected, string actual, double errorTolerance, int dpi = 300)
    {
        using (var expectedStream = File.OpenRead(expected))
        using (var actualStream = File.OpenRead(actual))
        {
            VisualEquals(expectedStream, actualStream, errorTolerance, dpi);
        }
    }

    public static void VisualEquals(Stream expected, Stream actual, double errorTolerance, int dpi = 300)
    {
        using (var expectedDocument = new PdfDocument(expected))
        using (var actualDocument = new PdfDocument(actual))
        {
            Assert.Equal(expectedDocument.PageCount, actualDocument.PageCount);

            for (int i = 1; i <= expectedDocument.PageCount; i++)
            {
                using (var expectedPage = expectedDocument.GetPage(i))
                using (var actualPage = actualDocument.GetPage(i))
                {
                    _visualEquals(expectedPage, actualPage, errorTolerance, dpi);
                }
            }
        }
    }

    private static void _visualEquals(PdfPage expected, PdfPage actual, double errorTolerance, int dpi)
    {
        var expectedRender = expected.Render(dpi, dpi);
        var actualRender = actual.Render(dpi, dpi);

        using (var expectedImage = Image.LoadPixelData<Bgra32>(expectedRender.Data, expectedRender.Width, expectedRender.Height))
        using (var actualImage = Image.LoadPixelData<Bgra32>(actualRender.Data, actualRender.Width, actualRender.Height))
        {
            ImageAssert.VisualEquals(expectedImage, actualImage, errorTolerance);
        }
    }
}
