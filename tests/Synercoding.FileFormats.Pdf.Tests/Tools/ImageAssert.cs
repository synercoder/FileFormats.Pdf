using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Synercoding.FileFormats.Pdf.Tests.Tools;

public static partial class ImageAssert
{
    public static void VisualEquals(Image expected, Image actual, double errorTolerance)
    {
        using (var expectedClone = expected.CloneAs<Bgra32>())
        using (var actualClone = actual.CloneAs<Bgra32>())
        {
            VisualEquals(expectedClone, actualClone, errorTolerance);
        }
    }

    public static void VisualEquals(Image<Bgra32> expected, Image<Bgra32> actual, double errorTolerance)
    {
        _ = expected ?? throw new ArgumentNullException(nameof(expected));
        _ = actual ?? throw new ArgumentNullException(nameof(actual));

        if (errorTolerance < 0 || errorTolerance >= 100)
            throw new ArgumentOutOfRangeException(nameof(errorTolerance), Messages.ERROR_TOLERANCE_MESSAGE);

        if (expected.Width != actual.Width || expected.Height != actual.Height)
            throw new SizeException(expected, actual, $"{nameof(ImageAssert)}.{nameof(VisualEquals)}");

        double diff = 0;

        for (int y = 0; y < expected.Height; y++)
        {
            var expectedRowSpan = expected.DangerousGetPixelRowMemory(y);
            var actualRowSpan = actual.DangerousGetPixelRowMemory(y);
            for (int x = 0; x < expected.Width; x++)
            {
                var pixel1 = expectedRowSpan.Span[x];
                var pixel2 = actualRowSpan.Span[x];

                diff += Math.Abs(pixel1.R - pixel2.R);
                diff += Math.Abs(pixel1.G - pixel2.G);
                diff += Math.Abs(pixel1.B - pixel2.B);
                diff += Math.Abs(pixel1.A - pixel2.A);
            }
        }

        diff = diff / ( expected.Width * expected.Height * 4 ) / 255;

        if (diff > errorTolerance)
            throw new VisualException(expected, actual, diff, $"{nameof(ImageAssert)}.{nameof(VisualEquals)}");
    }
}
