using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class RunlengthDecode : IStreamFilter
{
    public PdfName Name
        => PdfNames.RunLengthDecode;

    public byte[] Decode(byte[] input, IPdfDictionary? parameters)
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
        throw new NotImplementedException();
    }
}
