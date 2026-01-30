using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal sealed class IdGenerator
{
    private int _value;

    public IdGenerator()
        : this(1)
    { }

    public IdGenerator(int start)
    {
        _value = start - 1;
    }

    public PdfObjectId GetId()
    {
        var id = Interlocked.Increment(ref _value);
        return new PdfObjectId(id);
    }
}
