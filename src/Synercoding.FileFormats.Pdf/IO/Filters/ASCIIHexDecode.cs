using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements ASCII hexadecimal decoding and encoding filter for PDF streams.
/// This filter converts binary data to/from ASCII hexadecimal representation.
/// </summary>
public class ASCIIHexDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name
        => PdfNames.ASCIIHexDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always false for ASCII hex).
    /// </summary>
    public bool PassThrough => false;

    /// <summary>
    /// Encodes binary data using ASCII hexadecimal encoding.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters (not used for ASCII hex).</param>
    /// <returns>The ASCII hex-encoded data terminated with '>' marker.</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        var output = new byte[( input.Length * 2 ) + 1];
        for (int index = 0; index < input.Length; index++)
        {
            var b = input[index];
            output[index * 2] = b >> 4 < 10
                ? (byte)( ( b >> 4 ) + '0' )
                : (byte)( ( b >> 4 ) - 10 + 'A' );
            output[( index * 2 ) + 1] = ( b & 0x0F ) < 10
                ? (byte)( ( b & 0x0F ) + '0' )
                : (byte)( ( b & 0x0F ) - 10 + 'A' );
        }

        output[output.Length - 1] = ByteUtils.GREATER_THAN_SIGN;

        return output;
    }
}
