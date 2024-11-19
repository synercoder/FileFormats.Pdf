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
}
