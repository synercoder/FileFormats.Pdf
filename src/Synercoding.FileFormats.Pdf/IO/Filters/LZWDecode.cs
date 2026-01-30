using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements LZW (Lempel-Ziv-Welch) decoding and encoding filter for PDF streams.
/// LZW is a lossless data compression algorithm that builds a dictionary of strings during compression/decompression.
/// </summary>
public class LZWDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.LZWDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always false for LZW).
    /// </summary>
    public bool PassThrough => false;

    /// <summary>
    /// Encodes binary data using LZW compression.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters including EarlyChange setting.</param>
    /// <returns>The LZW-encoded data with clear and EOD markers.</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        if (input.Length == 0)
            return Array.Empty<byte>();

        var earlyChange = parameters != null
            && parameters.TryGetValue<PdfNumber>(PdfNames.EarlyChange, out var earlyChangeNumber)
            && !earlyChangeNumber.IsFractional
            ? earlyChangeNumber.LongValue
            : 1;

        var dictionary = new Dictionary<string, int>();
        var codes = new List<int>();

        // Initialize dictionary with single bytes
        for (int i = 0; i < 256; i++)
            dictionary[( (char)i ).ToString()] = i;

        int nextCode = 258; // 256 = clear, 257 = EOD
        string w = string.Empty;

        foreach (byte b in input)
        {
            string wc = w + (char)b;
            if (dictionary.ContainsKey(wc))
                w = wc;
            else
            {
                codes.Add(dictionary[w]);

                if (nextCode < 4096)
                    dictionary[wc] = nextCode++;
                else
                {
                    // Clear table when full
                    codes.Add(256); // Clear marker
                    dictionary.Clear();
                    for (int i = 0; i < 256; i++)
                        dictionary[( (char)i ).ToString()] = i;
                    nextCode = 258;
                    dictionary[wc] = nextCode++;
                }

                w = ( (char)b ).ToString();
            }
        }

        if (!string.IsNullOrEmpty(w))
            codes.Add(dictionary[w]);

        codes.Add(257); // EOD marker

        return _packCodes(codes, earlyChange);
    }

    private byte[] _packCodes(List<int> codes, long earlyChange = 1)
    {
        var result = new List<byte>();
        int bitsPerCode = 9;
        int bitBuffer = 0;
        int bitCount = 0;
        int nextCodeToAssign = 258;

        foreach (int code in codes)
        {
            // Write code with current bit width
            bitBuffer |= code << bitCount;
            bitCount += bitsPerCode;

            // Output complete bytes
            while (bitCount >= 8)
            {
                result.Add((byte)( bitBuffer & 0xFF ));
                bitBuffer >>= 8;
                bitCount -= 8;
            }

            // Adjust bit width for next code based on EarlyChange parameter
            if (code == 256) // Clear table
            {
                bitsPerCode = 9;
                nextCodeToAssign = 258;
            }
            else if (code == 257) // EOD
            {
                // No change
            }
            else
            {
                // After processing this code, a new dictionary entry will be created
                if (nextCodeToAssign < 4096)
                    nextCodeToAssign++;

                // Check if we need to increase bit width based on EarlyChange
                // When EarlyChange=1 (default): increase one code early
                // When EarlyChange=0: postpone as long as possible
                int threshold = earlyChange == 1 ? 0 : 1;

                if (nextCodeToAssign >= 512 - threshold && bitsPerCode == 9)
                    bitsPerCode = 10;
                else if (nextCodeToAssign >= 1024 - threshold && bitsPerCode == 10)
                    bitsPerCode = 11;
                else if (nextCodeToAssign >= 2048 - threshold && bitsPerCode == 11)
                    bitsPerCode = 12;
            }
        }

        // Flush remaining bits
        if (bitCount > 0)
            result.Add((byte)( bitBuffer & 0xFF ));

        return result.ToArray();
    }
}
