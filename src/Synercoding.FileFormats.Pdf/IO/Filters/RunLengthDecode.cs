using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements Run Length decoding and encoding filter for PDF streams.
/// Run Length Encoding compresses data by storing runs of identical bytes as a count followed by the byte value.
/// </summary>
public class RunLengthDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name
        => PdfNames.RunLengthDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always false for Run Length).
    /// </summary>
    public bool PassThrough => false;

    /// <summary>
    /// Encodes binary data using Run Length encoding.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters (not used for Run Length).</param>
    /// <returns>The Run Length-encoded data terminated with EOD marker (128).</returns>
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
                runLength++;

            if (runLength >= 2)
            {
                // Encode as repeat run
                outputStream.WriteByte((byte)( 257 - runLength ));
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
                        break; // Start of a repeat run, end literal sequence

                    literalLength++;
                }

                // Encode as literal run
                outputStream.WriteByte((byte)( literalLength - 1 ));
                outputStream.Write(input.AsSpan(literalStart, literalLength));
                index += literalLength;
            }
        }

        // Add EOD marker
        outputStream.WriteByte(128);
        return outputStream.ToArray();
    }
}
