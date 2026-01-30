using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;

namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Class representing an image inside a pdf
/// </summary>
public class PdfImage : IDisposable
{
    private protected bool _disposed;

    internal PdfImage(PdfObjectId id, Stream imageStream, int width, int height, ColorSpace colorSpace, PdfImage? softMask, double[]? decodeArray, params (PdfName Filter, IPdfDictionary? DecodeParms)[] appliedFilters)
    {
        Id = id;

        Width = width;
        Height = height;
        Stream = imageStream;
        ColorSpace = colorSpace;

        if (softMask != null)
        {
            if (softMask.SoftMask != null)
                throw new ArgumentException("Soft mask image can not also contain a soft mask.", nameof(softMask));
            if (softMask.ColorSpace != DeviceGray.Instance)
                throw new ArgumentException("Soft mask should be in colorspace DeviceGray.", nameof(softMask));
        }

        SoftMask = softMask;
        if (decodeArray != null)
        {
            if (decodeArray.Length != ColorSpace.Components * 2)
                throw new ArgumentException("Decode array must contains twice as many elements as there are components in the provided colorspace.", nameof(decodeArray));

            DecodeArray = decodeArray;
        }
        AppliedFilters = appliedFilters ?? Array.Empty<(PdfName, IPdfDictionary?)>();
    }

    internal PdfImage? SoftMask { get; }

    internal Stream Stream { get; }

    internal double[]? DecodeArray { get; }

    internal (PdfName Filter, IPdfDictionary? DecodeParms)[] AppliedFilters { get; }

    /// <summary>
    /// A pdf reference object that can be used to reference to this object
    /// </summary>
    public PdfObjectId Id { get; }

    /// <summary>
    /// The width of this <see cref="PdfImage"/>
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of this <see cref="PdfImage"/>
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The name of the colorspace used in this <see cref="PdfImage"/>
    /// </summary>
    public ColorSpace ColorSpace { get; }

    internal PdfObject<IPdfStreamObject> ToStreamObject(CachedResources cachedResources)
    {
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Type] = PdfNames.XObject,
            [PdfNames.Subtype] = PdfNames.Image,
            [PdfNames.Width] = new PdfNumber(Width),
            [PdfNames.Height] = new PdfNumber(Height),
            [PdfNames.BitsPerComponent] = new PdfNumber(8),
            [PdfNames.Length] = new PdfNumber(Stream.Length)
        };

        if (ColorSpace is Separation separation)
            dictionary[PdfNames.ColorSpace] = cachedResources.GetOrAdd(separation);
        else
            dictionary[PdfNames.ColorSpace] = ColorSpace.Name;

        if (DecodeArray != null)
            dictionary[PdfNames.Decode] = new PdfArray(DecodeArray);

        if (SoftMask != null)
            dictionary[PdfNames.SMask] = SoftMask.Id.GetReference();

        if (AppliedFilters is [(var singleFilter, var singleDecodeParms)])
        {
            dictionary[PdfNames.Filter] = singleFilter;
            if (singleDecodeParms != null)
                dictionary[PdfNames.DecodeParms] = singleDecodeParms;
        }
        else if (AppliedFilters.Length > 1)
        {
            var filterNames = AppliedFilters.Select(t => t.Filter).ToArray();
            var decodeParms = AppliedFilters.Select(t => t.DecodeParms).ToArray();
            dictionary[PdfNames.Filter] = new PdfArray(filterNames);
            if (decodeParms.Any(p => p != null))
            {
                var parms = new PdfArray();
                foreach (var parm in decodeParms)
                    parms.Add(parm is null ? PdfNull.INSTANCE : parm);
                dictionary[PdfNames.DecodeParms] = parms;
            }
        }

        return new PdfObject<IPdfStreamObject>()
        {
            Id = Id,
            Value = new ReadOnlyPdfStreamObject(dictionary, ByteUtils.ToByteArray(Stream))
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            Stream.Dispose();
            _disposed = true;
        }
    }

    private static Stream _encodeToJpg(Image image)
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

    internal static PdfImage Get(TableBuilder tableBuilder, Image<Rgba32> image)
        => new PdfImage(tableBuilder.ReserveId(), _encodeToJpg(image), image.Width, image.Height, DeviceRGB.Instance, GetMask(tableBuilder, image), null, (PdfNames.DCTDecode, null));

    internal static PdfImage Get(TableBuilder tableBuilder, Image<Rgb24> image)
        => new PdfImage(tableBuilder.ReserveId(), _encodeToJpg(image), image.Width, image.Height, DeviceRGB.Instance, null, null, (PdfNames.DCTDecode, null));

    internal static PdfImage GetSeparation(TableBuilder tableBuilder, Image<Rgb24> image, Separation separation, GrayScaleMethod grayScaleMethod, double[]? decodeArray = null)
    {
        using var grayScaleStream = AsGrayScaleByteStream(image, grayScaleMethod);

        return new PdfImage(tableBuilder.ReserveId(), _flateEncode(grayScaleStream), image.Width, image.Height, separation, null, decodeArray, (PdfNames.FlateDecode, null));
    }

    internal static PdfImage GetSeparation(TableBuilder tableBuilder, Image<Rgba32> image, Separation separation, GrayScaleMethod grayScaleMethod, double[]? decodeArray = null)
    {
        using var grayScaleStream = AsGrayScaleByteStream(image, grayScaleMethod);

        return new PdfImage(tableBuilder.ReserveId(), _flateEncode(grayScaleStream), image.Width, image.Height, separation, GetMask(tableBuilder, image), decodeArray, (PdfNames.FlateDecode, null));
    }

    internal static PdfImage? GetMask(TableBuilder tableBuilder, Image<Rgba32> image)
    {
        if (!_hasTransparancy(image))
            return null;

        using var grayScaleStream = AsGrayScaleByteStream(image, GrayScaleMethod.AlphaChannel);

        return new PdfImage(tableBuilder.ReserveId(), _flateEncode(grayScaleStream), image.Width, image.Height, DeviceGray.Instance, null, null, (PdfNames.FlateDecode, null));
    }

    private static bool _hasTransparancy(Image<Rgba32> image)
    {
        if (image.Metadata.TryGetPngMetadata(out var pngMeta))
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

    private static Stream _flateEncode(MemoryStream stream)
    {
        var bytes = stream.ToArray();

        bytes = new FlateDecode().Encode(bytes, null);

        return new MemoryStream(bytes);
    }

    internal static MemoryStream AsGrayScaleByteStream(Image<Rgba32> image, GrayScaleMethod grayScaleMethod)
    {
        var byteStream = new MemoryStream();

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

        return byteStream;
    }

    internal static MemoryStream AsGrayScaleByteStream(Image<Rgb24> image, GrayScaleMethod grayScaleMethod)
    {
        if (grayScaleMethod == GrayScaleMethod.AlphaChannel)
            throw new ArgumentException($"Can not use alpha channel for images of pixel format {nameof(Rgb24)}.", nameof(grayScaleMethod));

        var byteStream = new MemoryStream();

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
                    ref Rgb24 pixel = ref pixelRow[x];

                    var pixelValue = grayScaleMethod switch
                    {
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

        return byteStream;
    }
}
