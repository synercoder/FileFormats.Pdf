using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class ASCII85Decode : IStreamFilter
{
    public PdfName Name => PdfNames.ASCII85Decode;

    public byte[] Decode(byte[] input, IPdfDictionary? parameters)
    {
        using var outputStream = new MemoryStream();
        
        int index = 0;
        while (index < input.Length)
        {
            // Skip whitespace
            while (index < input.Length && ByteUtils.IsWhiteSpace(input[index]))
                index++;
            
            if (index >= input.Length)
                break;
                
            // Check for end marker '~>'
            if (input[index] == (byte)'~')
            {
                if (index + 1 < input.Length && input[index + 1] == (byte)'>')
                    break;
                // Invalid character '~' not followed by '>'
                throw new ParseException($"Invalid byte while parsing with {nameof(ASCII85Decode)} filter. Found {input[index]:X2} which is outside of ASCII85 range.");
            }
            
            // Handle 'z' shortcut for four null bytes
            if (input[index] == (byte)'z')
            {
                outputStream.Write([0, 0, 0, 0]);
                index++;
                continue;
            }
            
            // Read up to 5 characters for a group
            var group = new uint[5];
            int groupSize = 0;
            
            for (int i = 0; i < 5 && index < input.Length; i++)
            {
                // Skip whitespace
                while (index < input.Length && ByteUtils.IsWhiteSpace(input[index]))
                    index++;
                
                if (index >= input.Length)
                    break;
                    
                // Check for end marker
                if (input[index] == (byte)'~')
                    break;
                    
                var b = input[index++];
                
                // Validate character is in ASCII85 range
                if (b < 33 || b > 117) // '!' to 'u'
                    throw new ParseException($"Invalid byte while parsing with {nameof(ASCII85Decode)} filter. Found {b:X2} which is outside of ASCII85 range.");
                
                group[i] = (uint)(b - 33);
                groupSize++;
            }
            
            if (groupSize == 0)
                break;
            
            // For incomplete groups, pad with 'u' characters (84)
            for (int i = groupSize; i < 5; i++)
            {
                group[i] = 84;
            }
            
            // Decode the group using Horner's method
            uint decoded = group[0] * 85 * 85 * 85 * 85 +
                          group[1] * 85 * 85 * 85 +
                          group[2] * 85 * 85 +
                          group[3] * 85 +
                          group[4];
            
            // Output the bytes (big-endian)
            var bytesToOutput = groupSize - 1;
            for (int i = 0; i < bytesToOutput; i++)
            {
                outputStream.WriteByte((byte)(decoded >> (24 - (i * 8))));
            }
        }
        
        return outputStream.ToArray();
    }

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        using var outputStream = new MemoryStream();
        
        int index = 0;
        while (index < input.Length)
        {
            // Read up to 4 bytes
            uint value = 0;
            int bytesRead = 0;
            
            for (int i = 0; i < 4 && index < input.Length; i++)
            {
                value = (value << 8) | input[index++];
                bytesRead++;
            }
            
            // Check for special case of four null bytes
            if (bytesRead == 4 && value == 0)
            {
                outputStream.WriteByte((byte)'z');
                continue;
            }
            
            // Shift value for partial groups (pad with zeros on right)
            if (bytesRead < 4)
                value <<= (4 - bytesRead) * 8;
            
            // Convert to base 85 using repeated division
            var encoded = new uint[5];
            for (int i = 4; i >= 0; i--)
            {
                encoded[i] = value % 85;
                value /= 85;
            }
            
            // Output the encoded bytes (plus one extra for partial groups)
            var bytesToOutput = bytesRead + 1;
            for (int i = 0; i < bytesToOutput; i++)
            {
                outputStream.WriteByte((byte)(encoded[i] + 33));
            }
        }
        
        // Add end marker
        outputStream.WriteByte((byte)'~');
        outputStream.WriteByte((byte)'>');
        
        return outputStream.ToArray();
    }
}