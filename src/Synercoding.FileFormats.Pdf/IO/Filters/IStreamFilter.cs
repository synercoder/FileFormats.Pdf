using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Defines a contract for PDF stream filters that can encode and decode binary data.
/// </summary>
public interface IStreamFilter
{
    /// <summary>
    /// Gets the name of this filter as it appears in PDF documents.
    /// </summary>
    PdfName Name { get; }

    /// <summary>
    /// Indicates that the filter is meant for image data that this library does not encode or decode the data further.
    /// </summary>
    bool PassThrough { get; }

    /// <summary>
    /// Encodes binary data using this filter.
    /// </summary>
    /// <param name="input">The data to encode.</param>
    /// <param name="parameters">Optional parameters for the encoding process.</param>
    /// <returns>The encoded data.</returns>
    byte[] Encode(byte[] input, IPdfDictionary? parameters);
}
