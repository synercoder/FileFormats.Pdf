using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements JBIG2 decoding filter for PDF streams.
/// JBIG2 is a lossless and lossy compression standard for bi-level images. Currently implemented as a pass-through filter.
/// </summary>
public class JBIG2Decode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.JBIG2Decode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always true for JBIG2).
    /// </summary>
    public bool PassThrough => true;

    /// <summary>
    /// Encodes data using JBIG2 encoding. Currently returns input unchanged.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters for JBIG2 encoding.</param>
    /// <returns>The input data unchanged (pass-through implementation).</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        return input;
    }
}
