using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements JPX (JPEG 2000) decoding filter for PDF streams.
/// JPX is a modern image compression standard based on wavelets. Currently implemented as a pass-through filter.
/// </summary>
public class JPXDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.JPXDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always true for JPX).
    /// </summary>
    public bool PassThrough => true;

    /// <summary>
    /// Encodes data using JPX encoding. Currently returns input unchanged.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters for JPX encoding.</param>
    /// <returns>The input data unchanged (pass-through implementation).</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        return input;
    }
}
