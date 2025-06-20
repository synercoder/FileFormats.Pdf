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
}
