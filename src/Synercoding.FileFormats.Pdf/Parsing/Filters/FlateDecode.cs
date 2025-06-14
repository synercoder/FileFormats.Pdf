using Synercoding.FileFormats.Pdf.Primitives;
using System.IO.Compression;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;
public class FlateDecode : IStreamFilter
{
    public PdfName Name => PdfNames.FlateDecode;

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
            {
                flateStream.Write(input);
            }
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

    public byte[] Decode(byte[] input, IPdfDictionary? parameters)
    {
        if (parameters?.TryGetValue<PdfNumber>(PdfNames.Predictor, out var predictorInteger) == true && predictorInteger != 1)
            throw new NotImplementedException($"{nameof(FlateDecode)} currently only supports flate encoding with no prediction function.");

        using (var inputStream = new MemoryStream(input))
        {
            inputStream.ReadByte();
            inputStream.ReadByte();

            using (var outputStream = new MemoryStream())
            {
                using (var flateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    flateStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }
}
