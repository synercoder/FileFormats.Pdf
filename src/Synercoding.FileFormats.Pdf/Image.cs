using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Class representing an image inside a pdf
/// </summary>
public class Image : IDisposable
{
    private protected bool _disposed;

    internal static Image Get(TableBuilder tableBuilder, Image<Rgba32> image)
    {
        return new Image(tableBuilder.ReserveId(), _encodeToJpg(image), image.Width, image.Height, DeviceRGB.Instance, GetMask(tableBuilder, image));
    }

    internal static SoftMask? GetMask(TableBuilder tableBuilder, Image<Rgba32> image)
    {
        var hasTrans = image.Metadata.TryGetPngMetadata(out var pngMeta)
            &&
            (
                pngMeta.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha
                || pngMeta.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.GrayscaleWithAlpha
            );

        return hasTrans
            ? new SoftMask(tableBuilder.ReserveId(), AsImageByteStream(image, GrayScaleMethod.AlphaChannel), image.Width, image.Height)
            : null;
    }

    internal static Stream AsImageByteStream(Image<Rgba32> image, GrayScaleMethod grayScaleMethod)
    {
        var ms = new MemoryStream();

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);

                // pixelRow.Length has the same value as accessor.Width,
                // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                for (int x = 0; x < pixelRow.Length; x++)
                {
                    // Get a reference to the pixel at position x
                    ref Rgba32 pixel = ref pixelRow[x];

                    var pixelValue = grayScaleMethod switch
                    {
                        GrayScaleMethod.AlphaChannel => pixel.A,
                        GrayScaleMethod.RedChannel => pixel.R,
                        GrayScaleMethod.GreenChannel => pixel.G,
                        GrayScaleMethod.BlueChannel => pixel.B,
                        GrayScaleMethod.AverageOfRGBChannels => (byte)( ( pixel.R + pixel.G + pixel.B ) / 3 ),
                        _ => throw new NotImplementedException()
                    };

                    ms.WriteByte(pixelValue);

                }
            }
        });

        ms.Position = 0;

        return ms;
    }

    //internal Image(PdfReference id, SixLabors.ImageSharp.Image image)
    //    : this(id, _encodeToJpg(image), image.Width, image.Height, DeviceRGB.Instance, null)
    //{ }

    internal Image(PdfReference id, Stream jpgStream, int width, int height, ColorSpace colorSpace, SoftMask? softMask)
        : this(id, jpgStream, width, height, colorSpace.Name, _decodeArray(colorSpace), softMask)
    { }

    internal Image(PdfReference id, Stream jpgStream, int width, int height, PdfName colorSpace, double[] decodeArray, SoftMask? softMask)
    {
        Reference = id;

        Width = width;
        Height = height;
        RawStream = jpgStream;
        ColorSpace = colorSpace;
        DecodeArray = decodeArray;
        SoftMask = softMask;
    }

    internal SoftMask? SoftMask { get; private set; }

    internal Stream RawStream { get; private set; }

    /// <summary>
    /// A pdf reference object that can be used to reference to this object
    /// </summary>
    public PdfReference Reference { get; private set; }

    /// <summary>
    /// The width of this <see cref="Image"/>
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of this <see cref="Image"/>
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The name of the colorspace used in this <see cref="Image"/>
    /// </summary>
    public PdfName ColorSpace { get; }

    /// <summary>
    /// The decode array used in this <see cref="Image"/>
    /// </summary>
    public double[] DecodeArray { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            RawStream.Dispose();
            _disposed = true;
        }
    }

    private static Stream _encodeToJpg(SixLabors.ImageSharp.Image image)
    {
        var ms = new MemoryStream();
        image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
        {
            Quality = 100,
            ColorType = SixLabors.ImageSharp.Formats.Jpeg.JpegEncodingColor.YCbCrRatio444
        });
        ms.Position = 0;

        return ms;
    }

    private static double[] _decodeArray(ColorSpace colorSpace)
        => Enumerable.Range(0, colorSpace.Components)
            .Select(_ => new double[] { 0, 1 })
            .SelectMany(x => x)
            .ToArray();
}
