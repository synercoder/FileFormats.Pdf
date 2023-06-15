using Synercoding.FileFormats.Pdf.LowLevel.Internal;

namespace Synercoding.FileFormats.Pdf.LowLevel.XRef;

internal class TableBuilder
{
    private readonly IdGenerator _idGen = new IdGenerator();
    private readonly Dictionary<PdfReference, long> _positions = new Dictionary<PdfReference, long>();

    public PdfReference ReserveId()
    {
        var id = _idGen.GetId();
        var reference = new PdfReference(id);
        _positions.Add(reference, -1);
        return reference;
    }

    public PdfReference GetId(uint position)
    {
        var id = _idGen.GetId();
        var reference = new PdfReference(id);
        _positions.Add(reference, position);
        return reference;
    }

    public bool TrySetPosition(PdfReference id, uint position)
    {
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

    public Table GetXRefTable()
    {
        var entries = _positions
            .OrderBy(static x => x.Key.ObjectId)
            .Select(static x => new Entry((uint)x.Value))
            .ToArray();
        return new Table(entries);
    }
}
