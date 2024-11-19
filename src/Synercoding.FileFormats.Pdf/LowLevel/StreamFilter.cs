namespace Synercoding.FileFormats.Pdf.LowLevel;

/// <summary>
/// Enum representing different data encoding methods
/// </summary>
internal enum StreamFilter
{
    /// <summary>
    /// Decompress data encoded using a DCT (discrete cosine transform) technique based on the JPEG standard,
    /// reproducing image sample data that approximates the original data.
    /// </summary>
    DCTDecode,

    /// <summary>
    /// The Flate method is based on the public-domain zlib/deflate compression method,
    /// which is a variable-length Lempel-Ziv adaptive compression method cascaded
    /// with adaptive Huffman coding. It is fully defined in Internet RFC 1950, and Internet RFC 1951.
    /// </summary>
    FlateDecode,
}
