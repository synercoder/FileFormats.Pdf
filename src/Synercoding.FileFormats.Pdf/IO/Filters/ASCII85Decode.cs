using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements ASCII85 (base-85) decoding and encoding filter for PDF streams.
/// ASCII85 is an efficient binary-to-text encoding that represents 4 bytes as 5 ASCII characters.
/// </summary>
public class ASCII85Decode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.ASCII85Decode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always false for ASCII85).
    /// </summary>
    public bool PassThrough => false;

    /// <summary>
    /// Encodes binary data using ASCII85 encoding.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters (not used for ASCII85).</param>
    /// <returns>The ASCII85-encoded data terminated with '~>' marker.</returns>
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
                value = ( value << 8 ) | input[index++];
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
                value <<= ( 4 - bytesRead ) * 8;

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
                outputStream.WriteByte((byte)( encoded[i] + 33 ));
        }

        // Add end marker
        outputStream.WriteByte((byte)'~');
        outputStream.WriteByte((byte)'>');

        return outputStream.ToArray();
    }
}
