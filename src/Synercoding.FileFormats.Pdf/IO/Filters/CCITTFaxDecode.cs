using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Implements CCITT fax decoding filter for PDF streams.
/// Currently implemented as a pass-through filter that returns data unchanged.
/// </summary>
public class CCITTFaxDecode : IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as used in PDF documents.
    /// </summary>
    public PdfName Name => PdfNames.CCITTFaxDecode;

    /// <summary>
    /// Gets a value indicating whether this filter is a pass-through filter (always true for CCITT fax).
    /// </summary>
    public bool PassThrough => true;

    /// <summary>
    /// Encodes data using CCITT fax encoding. Currently returns input unchanged.
    /// </summary>
    /// <param name="input">The binary data to encode.</param>
    /// <param name="parameters">Optional encode parameters for CCITT fax encoding.</param>
    /// <returns>The input data unchanged (pass-through implementation).</returns>
    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        return input;
    }
}
