namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF stream object, which is a dictionary with associated binary data.
/// </summary>
public interface IPdfStreamObject : IPdfDictionary
{
    /// <summary>
    /// Gets the raw stream data before any filters are applied.
    /// </summary>
    byte[] RawData { get; }

    /// <summary>
    /// Gets the length of the raw stream data.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Returns this stream object as a pure dictionary without stream data.
    /// </summary>
    /// <returns>The dictionary portion of this stream object.</returns>
    IPdfDictionary AsPureDictionary();
}
