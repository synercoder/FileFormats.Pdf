using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class RunLengthDecode : IStreamFilter
{
    public PdfName Name
        => PdfNames.RunLengthDecode;

    public byte[] Decode(byte[] input, IPdfDictionary? parameters, ObjectReader objectReader)
    {
        using var memoryStream = new MemoryStream();

        for(int index = 0; index < input.Length; index++)
        {
            var length = input[index];
            if (length == 128)
                break; // EOD indicator

            if(length <= 127)
            {
                var copyCount = length + 1;
                memoryStream.Write(input.AsSpan(index + 1, copyCount));
                index += copyCount;
            }
            else
            {
                var sameByteCount = 257 - length;
                index++;
                for (int copyIndex = 0; copyIndex < sameByteCount; copyIndex++)
                    memoryStream.WriteByte(input[index]);
            }
        }

        return memoryStream.ToArray();
    }

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        if (input.Length == 0)
            return new byte[] { 128 }; // Just EOD marker

        using var outputStream = new MemoryStream();
        int index = 0;

        while (index < input.Length)
        {
            var currentByte = input[index];
            var runLength = 1;

            // Look for repeated bytes
            while (index + runLength < input.Length && 
                   input[index + runLength] == currentByte && 
                   runLength < 128)
            {
                runLength++;
            }

            if (runLength >= 2)
            {
                // Encode as repeat run
                outputStream.WriteByte((byte)(257 - runLength));
                outputStream.WriteByte(currentByte);
                index += runLength;
            }
            else
            {
                // Look for literal sequence
                var literalStart = index;
                var literalLength = 1;

                while (index + literalLength < input.Length && literalLength < 128)
                {
                    var nextByte = input[index + literalLength];
                    
                    // Check if we're starting a run of repeated bytes
                    if (index + literalLength + 1 < input.Length && 
                        input[index + literalLength + 1] == nextByte)
                    {
                        break; // Start of a repeat run, end literal sequence
                    }
                    
                    literalLength++;
                }

                // Encode as literal run
                outputStream.WriteByte((byte)(literalLength - 1));
                outputStream.Write(input.AsSpan(literalStart, literalLength));
                index += literalLength;
            }
        }

        // Add EOD marker
        outputStream.WriteByte(128);
        return outputStream.ToArray();
    }
}
