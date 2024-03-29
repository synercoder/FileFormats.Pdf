namespace Synercoding.FileFormats.Pdf.Internals;

internal static class SpanHelper
{
    public static int FillSpan(Span<byte> span, int integer)
    {
        int val = integer;
        var intSize = ByteSizes.Size(integer);
        for (int i = intSize - 1; i >= 0; i--)
        {
            span[i] = (byte)( '0' + ( val % 10 ) );
            val /= 10;
        }
        return intSize;
    }

    public static int FillSpan(Span<byte> span, uint integer)
    {
        uint val = integer;
        var intSize = ByteSizes.Size(integer);
        for (int i = intSize - 1; i >= 0; i--)
        {
            span[i] = (byte)( '0' + ( val % 10 ) );
            val /= 10;
        }
        return intSize;
    }

    public static int WriteIndirectObjectRef(Span<byte> bytes, int objectRef, int generation)
    {
        int position = FillSpan(bytes, objectRef);

        bytes[position++] = 0x20; // space

        position += FillSpan(bytes[position..], generation);

        bytes[position++] = 0x20; // space

        bytes[position++] = 0x52; // R

        return position;
    }
}
