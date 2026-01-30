using Synercoding.FileFormats.Pdf.Primitives;
using System.IO.Compression;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements Flate (zlib/deflate) decoding and encoding filter for PDF streams.
/// This filter provides lossless compression using the deflate algorithm with optional predictor functions.
/// </summary>
public class FlateDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.FlateDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always false for Flate).
    /// </summary>
    public bool PassThrough => false;

    /// <summary>
    /// Encodes binary data using Flate (zlib/deflate) compression.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters including predictor settings.</param>
    /// <returns>The Flate-encoded data with zlib headers.</returns>
    /// <exception cref="NotImplementedException">Thrown when predictor functions other than 1 are specified.</exception>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        if (parameters?.TryGetValue<PdfNumber>(PdfNames.Predictor, out var predictorInteger) == true && predictorInteger != 1)
            throw new NotImplementedException($"{nameof(FlateDecode)} currently only supports flate encoding with no prediction function.");

        using (var outputStream = new MemoryStream())
        {
            const CompressionLevel LEVEL = CompressionLevel.SmallestSize;

            var (method, flags) = _getHeader(LEVEL);
            outputStream.WriteByte(method);
            outputStream.WriteByte(flags);

            using (var flateStream = new DeflateStream(outputStream, LEVEL, leaveOpen: true))
                flateStream.Write(input);
            return outputStream.ToArray();
        }
    }

    private (byte CompressionMethod, byte Flags) _getHeader(CompressionLevel compressionLevel)
        => compressionLevel switch
        {
            CompressionLevel.Optimal => (0x78, 0x9C),
            CompressionLevel.Fastest => (0x78, 0x5E),
            CompressionLevel.SmallestSize => (0x78, 0xDA),
            CompressionLevel.NoCompression => (0x78, 0x01),
            var level => throw new NotImplementedException("Unsupported compression level: {level}")
        };
}
