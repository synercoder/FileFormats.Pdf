using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class ASCIIHexDecode : IStreamFilter
{
    public PdfName Name
        => PdfNames.ASCIIHexDecode;

    public byte[] Decode(byte[] input, IPdfDictionary? parameters)
    {
        using var memoryStream = new MemoryStream(input.Length * 2);

        int index = 0;
        while (true)
        {
            if (index >= input.Length)
                break;

            var b = input[index++];
            while (ByteUtils.IsWhiteSpace(b) && index < input.Length)
            {
                b = input[index++];
            }

            if (b == ByteUtils.GREATER_THAN_SIGN)
                break;

            byte outputByte = b switch
            {
                >= (byte)'0' and <= (byte)'9' => (byte)( ( b - '0' ) << 4 ),
                >= (byte)'a' and <= (byte)'f' => (byte)( ( b - 'a' + 10 ) << 4 ),
                >= (byte)'A' and <= (byte)'F' => (byte)( ( b - 'A' + 10 ) << 4 ),
                var outOfRange => throw new ParseException($"Invalid byte while parsing with {nameof(ASCIIHexDecode)} filter. Found {outOfRange:X2} which is outside of hex range.")
            };

            if (index >= input.Length)
            {
                memoryStream.WriteByte(outputByte);
                break;
            }

            b = input[index++];
            while (ByteUtils.IsWhiteSpace(b) && index < input.Length)
            {
                b = input[index++];
            }

            if (b == ByteUtils.GREATER_THAN_SIGN || ByteUtils.IsWhiteSpace(b))
            {
                memoryStream.WriteByte(outputByte);
                break;
            }

            outputByte = b switch
            {
                >= (byte)'0' and <= (byte)'9' => (byte)( ( ( b - '0' ) & 0x0F ) | outputByte ),
                >= (byte)'a' and <= (byte)'f' => (byte)( ( ( b - 'a' + 10 ) & 0x0F ) | outputByte ),
                >= (byte)'A' and <= (byte)'F' => (byte)( ( ( b - 'A' + 10 ) & 0x0F ) | outputByte ),
                var outOfRange => throw new ParseException($"Invalid byte while parsing with {nameof(ASCIIHexDecode)} filter. Found byte 0x{outOfRange:X2} which is outside of hex range.")
            };

            memoryStream.WriteByte(outputByte);
        }

        return memoryStream.ToArray();
    }

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        var output = new byte[(input.Length * 2) + 1];
        for (int index = 0; index < input.Length; index++)
        {
            var b = input[index];
            output[index * 2] = ( b >> 4 ) < 10
                ? (byte)( ( b >> 4 ) + '0' )
                : (byte)( ( b >> 4 ) - 10 + 'A' );
            output[(index * 2) + 1] = ( b & 0x0F ) < 10
                ? (byte)( ( b & 0x0F ) + '0' )
                : (byte)( ( b & 0x0F ) - 10 + 'A' );
        }

        output[output.Length - 1] = ByteUtils.GREATER_THAN_SIGN;

        return output;
    }
}
