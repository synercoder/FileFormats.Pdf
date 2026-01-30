namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Represents a subsection in the cross-reference table
/// </summary>
internal class XRefSubsection
{
    public XRefSubsection(int firstObjectNumber, IReadOnlyList<XRefEntry> entries)
    {
        FirstObjectNumber = firstObjectNumber;
        Entries = entries ?? throw new ArgumentNullException(nameof(entries));
    }

    /// <summary>
    /// The first object number in this subsection
    /// </summary>
    public int FirstObjectNumber { get; }

    /// <summary>
    /// The number of entries in this subsection
    /// </summary>
    public int Count => Entries.Count;

    /// <summary>
    /// The entries in this subsection
    /// </summary>
    public IReadOnlyList<XRefEntry> Entries { get; }
}
