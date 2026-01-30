namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Represents a single entry in the cross-reference table
/// </summary>
internal struct XRefEntry
{
    public XRefEntry(long byteOffset, int generation, bool inUse)
    {
        ByteOffset = byteOffset;
        Generation = generation;
        InUse = inUse;
    }

    /// <summary>
    /// The byte offset in the file
    /// </summary>
    public long ByteOffset { get; }

    /// <summary>
    /// The generation number
    /// </summary>
    public int Generation { get; }

    /// <summary>
    /// Whether this entry is in use
    /// </summary>
    public bool InUse { get; }
}
