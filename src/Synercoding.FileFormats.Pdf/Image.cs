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

    internal Image(PdfReference id, Stream jpgStream, int width, int height, ColorSpace colorSpace, Image? softMask, double[]? decodeArray, params StreamFilter[] filters)
    {
        Reference = id;

        Width = width;
        Height = height;
        RawStream = jpgStream;
        ColorSpace = colorSpace;
        SoftMask = softMask;
        DecodeArray = decodeArray;
        Filters = filters;
    }

    internal Image? SoftMask { get; private set; }

    internal Stream RawStream { get; private set; }

    internal double[]? DecodeArray { get; private set; }

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
    public ColorSpace ColorSpace { get; }

    internal StreamFilter[] Filters { get; } = Array.Empty<StreamFilter>();

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

    internal static Image Get(TableBuilder tableBuilder, Image<Rgba32> image)
    {
        return new Image(tableBuilder.ReserveId(), _encodeToJpg(image), image.Width, image.Height, DeviceRGB.Instance, GetMask(tableBuilder, image), null, StreamFilter.DCTDecode);
    }

    internal static Image? GetMask(TableBuilder tableBuilder, Image<Rgba32> image)
    {
        return _hasTransparancy(image)
            ? new Image(tableBuilder.ReserveId(), AsImageByteStream(image, GrayScaleMethod.AlphaChannel), image.Width, image.Height, DeviceGray.Instance, null, null, StreamFilter.FlateDecode)
            : null;
    }

    private static bool _hasTransparancy(Image<Rgba32> image)
    {
        if( image.Metadata.TryGetPngMetadata(out var pngMeta))
        {
            if (pngMeta.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.RgbWithAlpha)
                return true;
            if (pngMeta.ColorType == SixLabors.ImageSharp.Formats.Png.PngColorType.GrayscaleWithAlpha)
                return true;
        }

        bool hasTransparancy = false;
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                {
                    ref Rgba32 pixel = ref row[x];
                    if (pixel.A != 0xFF)
                    {
                        hasTransparancy = true;
                        return;
                    }
                }
            }
        });
        return hasTransparancy;
    }

    internal static Stream AsImageByteStream(Image<Rgba32> image, GrayScaleMethod grayScaleMethod)
    {
        using (var byteStream = new MemoryStream())
        {
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
                            GrayScaleMethod.BT601 => (byte)( ( pixel.R * 0.299 ) + ( pixel.G * 0.587 ) + ( pixel.B * 0.114 ) ),
                            GrayScaleMethod.BT709 => (byte)( ( pixel.R * 0.2126 ) + ( pixel.G * 0.7152 ) + ( pixel.B * 0.0722 ) ),
                            _ => throw new NotImplementedException()
                        };

                        byteStream.WriteByte(pixelValue);
                    }
                }
            });

            byteStream.Position = 0;

            return PdfWriter.FlateEncode(byteStream);
        }
    }
}
