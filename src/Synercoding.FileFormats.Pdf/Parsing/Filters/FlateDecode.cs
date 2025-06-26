using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
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

    public byte[] Decode(byte[] input, IPdfDictionary? parameters, ObjectReader objectReader)
    {
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
                var decodedData = outputStream.ToArray();
                
                // Apply predictor function if specified
                if (parameters != null && parameters.TryGetValue<PdfNumber>(PdfNames.Predictor, objectReader, out var predictorNumber) && !predictorNumber.IsFractional)
                {
                    var predictor = (int)predictorNumber.LongValue;
                    if (predictor > 1)
                    {
                        decodedData = _applyPredictor(decodedData, predictor, parameters, objectReader);
                    }
                }
                
                return decodedData;
            }
        }
    }

    private byte[] _applyPredictor(byte[] data, int predictor, IPdfDictionary parameters, ObjectReader objectReader)
    {
        var predictors = new Predictors();

        if (predictor == 2)
        {
            // TIFF predictor 2
            var columns = parameters.TryGetValue<PdfNumber>(PdfNames.Columns, objectReader, out var columnsNumber) && !columnsNumber.IsFractional
                ? (int)columnsNumber.LongValue
                : 1;

            var bitsPerComponent = parameters.TryGetValue<PdfNumber>(PdfNames.BitsPerComponent, objectReader, out var bitsNumber) && !bitsNumber.IsFractional
                ? (int)bitsNumber.LongValue
                : 8;

            var colors = parameters.TryGetValue<PdfNumber>(PdfNames.Colors, objectReader, out var colorsNumber) && !colorsNumber.IsFractional
                ? (int)colorsNumber.LongValue
                : 1;

            return predictors.DecodeTiff(data, columns, bitsPerComponent, colors);
        }
        else if (predictor >= 10 && predictor <= 15)
        {
            // PNG predictors
            var columns = parameters.TryGetValue<PdfNumber>(PdfNames.Columns, objectReader, out var columnsNumber) && !columnsNumber.IsFractional
                ? (int)columnsNumber.LongValue
                : 1;

            var bitsPerComponent = parameters.TryGetValue<PdfNumber>(PdfNames.BitsPerComponent, objectReader, out var bitsNumber) && !bitsNumber.IsFractional
                ? (int)bitsNumber.LongValue
                : 8;

            var colors = parameters.TryGetValue<PdfNumber>(PdfNames.Colors, objectReader, out var colorsNumber) && !colorsNumber.IsFractional
                ? (int)colorsNumber.LongValue
                : 1;

            // PNG predictors need the number of bytes per sample (columns * colors * bitsPerComponent / 8)
            var bytesPerRow = columns * colors * ((bitsPerComponent + 7) / 8);
            return predictors.DecodePng(data, bytesPerRow);
        }
        else if (predictor == 1)
        {
            // No prediction
            return data;
        }
        else
        {
            throw new NotSupportedException($"Predictor {predictor} is not supported");
        }
    }
}
