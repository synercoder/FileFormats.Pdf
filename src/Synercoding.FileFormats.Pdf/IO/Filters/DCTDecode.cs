using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements DCT (Discrete Cosine Transform) decoding filter for PDF streams.
/// DCT is commonly used for JPEG image compression. Currently implemented as a pass-through filter.
/// </summary>
public class DCTDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.DCTDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always true for DCT).
    /// </summary>
    public bool PassThrough => true;

    /// <summary>
    /// Encodes data using DCT encoding. Currently returns input unchanged.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters for DCT encoding.</param>
    /// <returns>The input data unchanged (pass-through implementation).</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        return input;
    }
}
