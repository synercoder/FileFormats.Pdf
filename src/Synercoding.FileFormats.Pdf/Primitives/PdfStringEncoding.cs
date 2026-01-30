namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Defines the various encoding types that can be used for PDF strings.
/// </summary>
public enum PdfStringEncoding
{
    /// <summary>
    /// PDFDocEncoding, a superset of ISO Latin 1 character set.
    /// </summary>
    PdfDocEncoding,
    /// <summary>
    /// UTF-16 Big Endian encoding with byte order mark (0xFE 0xFF).
    /// </summary>
    Utf16BE,
    /// <summary>
    /// UTF-16 Little Endian encoding with byte order mark (0xFF 0xFE).
    /// </summary>
    Utf16LE,
    /// <summary>
    /// UTF-8 encoding with byte order mark (0xEF 0xBB 0xBF).
    /// </summary>
    Utf8,
    /// <summary>
    /// Raw byte string that cannot be decoded as text.
    /// </summary>
    ByteString
}
