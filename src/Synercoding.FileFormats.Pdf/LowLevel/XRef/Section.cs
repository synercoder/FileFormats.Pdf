using Synercoding.FileFormats.Pdf.Internals;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.XRef;

internal class Section
{
    public Section(int firstObjectNumber, params Entry[] entries)
    {
        FirstObjectNumber = firstObjectNumber;
        Entries = entries;
    }

    public int FirstObjectNumber { get; }
    public int ObjectCount => Entries.Length;
    public Entry[] Entries { get; } = new Entry[0];

    public void FillSpan(Span<byte> bytes)
    {
        int bytesFilled = SpanHelper.FillSpan(bytes[..], FirstObjectNumber);
        bytes[bytesFilled] = 0x20;

        bytesFilled = bytesFilled + 1 + SpanHelper.FillSpan(bytes[( bytesFilled + 1 )..], ObjectCount);
        bytes[bytesFilled] = 0x0D;
        bytes[bytesFilled + 1] = 0x0A;

        for (int i = 0; i < Entries.Length; i++)
        {
            var position = bytesFilled + 2 + ( i * 20 );
            Entries[i].FillSpan(bytes.Slice(position, 20));
        }
    }

    public int ByteSize()
    {
        var size = ByteSizes.Size(FirstObjectNumber);
        size += ByteSizes.Size(ObjectCount);
        size += 3; // 1 space + CR LF
        size += ObjectCount * 20;

        return size;
    }
}
