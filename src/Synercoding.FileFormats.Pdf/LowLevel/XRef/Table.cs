namespace Synercoding.FileFormats.Pdf.LowLevel.XRef;

internal class Table
{
    public Table(params Entry[] entries)
    {
        var combined = new Entry[entries.Length + 1];
        combined[0] = new Entry(0, 65535, true);
        Array.Copy(entries, 0, combined, 1, entries.Length);
        Section = new Section(0, combined);
    }

    public Section Section { get; }

    internal uint WriteToStream(PdfStream stream)
    {
        var position = stream.Position;

        var bytes = new byte[_byteSize()];
        _fillSpan(bytes);
        stream.Write(bytes);

        return position;
    }

    private void _fillSpan(Span<byte> bytes)
    {
        bytes[0] = 0x78; // x
        bytes[1] = 0x72; // r
        bytes[2] = 0x65; // e 
        bytes[3] = 0x66; // f
        bytes[4] = 0x0D; // CR
        bytes[5] = 0x0A; // LF

        var freeSize = Section.ByteSize();
        Section.FillSpan(bytes.Slice(6, freeSize));
    }

    private int _byteSize()
    {
        int size = 6; // xref + CR LF
        size += Section.ByteSize();

        return size;
    }
}
