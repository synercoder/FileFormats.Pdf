using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal class TableBuilder
{
    private readonly IdGenerator _idGen = new IdGenerator();
    private readonly Dictionary<PdfObjectId, long> _positions = new Dictionary<PdfObjectId, long>();

    public PdfObjectId ReserveId()
    {
        var id = _idGen.GetId();
        _positions.Add(id, -1);
        return id;
    }

    public bool IsWritten(PdfObjectId id)
        => _positions[id] != -1;

    public bool TrySetPosition(PdfObjectId id, long position)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position));

        if (_positions[id] != -1)
            return false;

        _positions[id] = position;

        return true;
    }

    public bool Validate()
    {
        foreach (var value in _positions.Values)
        {
            if (value == -1)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets the number of objects in the table (excluding the free object at position 0)
    /// </summary>
    public int Count => _positions.Count;

    /// <summary>
    /// Gets the cross-reference subsections for writing the xref table
    /// </summary>
    public IEnumerable<XRefSubsection> GetSubsections()
    {
        // First, we need to include the free object at position 0
        var allEntries = new List<(int objectNumber, XRefEntry entry)>
        {
            (0, new XRefEntry(0, 65535, false)) // Free object at position 0
        };

        // Add all tracked objects
        foreach (var kvp in _positions.OrderBy(x => x.Key.ObjectNumber))
        {
            if (kvp.Value == -1)
                throw new InvalidOperationException($"Object {kvp.Key} has not been written to the stream.");

            allEntries.Add((kvp.Key.ObjectNumber, new XRefEntry(kvp.Value, kvp.Key.Generation, true)));
        }

        // Group consecutive object numbers into subsections
        var subsections = new List<XRefSubsection>();
        var currentSubsection = new List<XRefEntry>();
        int? firstObjectNumber = null;
        int? lastObjectNumber = null;

        foreach (var (objectNumber, entry) in allEntries.OrderBy(x => x.objectNumber))
        {
            if (firstObjectNumber == null)
            {
                // Start of first subsection
                firstObjectNumber = objectNumber;
                lastObjectNumber = objectNumber;
                currentSubsection.Add(entry);
            }
            else if (objectNumber == lastObjectNumber + 1)
            {
                // Consecutive object number
                lastObjectNumber = objectNumber;
                currentSubsection.Add(entry);
            }
            else
            {
                // Gap in object numbers, start new subsection
                subsections.Add(new XRefSubsection(firstObjectNumber.Value, currentSubsection.ToArray()));

                currentSubsection = new List<XRefEntry> { entry };
                firstObjectNumber = objectNumber;
                lastObjectNumber = objectNumber;
            }
        }

        // Add the last subsection
        if (currentSubsection.Count > 0 && firstObjectNumber.HasValue)
            subsections.Add(new XRefSubsection(firstObjectNumber.Value, currentSubsection.ToArray()));

        return subsections;
    }
}
