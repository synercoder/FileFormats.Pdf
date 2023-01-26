using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.XRef;

internal readonly struct Entry
{
    public Entry(uint data)
        : this(data, 0, false)
    { }

    public Entry(uint data, ushort generation, bool isFree)
    {
        Data = data;
        GenerationNumber = generation;
        IsFree = isFree;
    }

    public uint Data { get; }
    public ushort GenerationNumber { get; }

    public bool IsFree { get; }

    public void FillSpan(Span<byte> bytes)
    {
        _fillSpanLeadingZero(bytes[..10], Data);
        bytes[10] = 0x20;

        _fillSpanLeadingZero(bytes.Slice(11, 5), GenerationNumber);
        bytes[16] = 0x20;

        bytes[17] = (byte)( IsFree ? 0x66 : 0x6E );
        bytes[18] = 0x0D;
        bytes[19] = 0x0A;
    }

    public int ByteSize()
    {
        return 20;
    }

    private void _fillSpanLeadingZero(Span<byte> span, uint data)
    {
        uint val = data;
        for (int i = span.Length - 1; i >= 0; i--)
        {
            span[i] = (byte)( '0' + ( val % 10 ) );
            val /= 10;
        }
    }

    private void _fillSpanLeadingZero(Span<byte> span, ushort data)
    {
        int val = data;
        for (int i = span.Length - 1; i >= 0; i--)
        {
            span[i] = (byte)( '0' + ( val % 10 ) );
            val /= 10;
        }
    }
}
