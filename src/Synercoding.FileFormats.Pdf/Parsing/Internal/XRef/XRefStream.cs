using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal static class XRefStream
{
    public static XRefItem[] ParseStream(IPdfStreamObject stream, ObjectReader reader)
    {
        _ = stream ?? throw new ArgumentNullException(nameof(stream));

        if (!stream.TryGetValue<PdfName>(PdfNames.Type, out var typeName) || typeName != PdfNames.XRef)
            throw new ParseException("XRef stream did not contain a /Type key of value /XRef.");

        var sections = _getSections(stream);

        var (w1, w2, w3) = _getWidths(stream);

        var decodedData = stream.DecodeData(reader);

        var items = new XRefItem[sections.Sum(s => s.NumberOfEntries)];
        var index = 0;
        using (var memoryStream = new MemoryStream(decodedData))
        {
            foreach (var section in sections)
            {
                for (int objNmr = section.FirstObjectNumber; objNmr < section.FirstObjectNumber + section.NumberOfEntries; objNmr++)
                {
                    var type = _readType(memoryStream, w1);

                    if (type == 0)
                    {
                        // field 2 refers to the next free object number
                        var nextFreeObjectNumber = _readField(memoryStream, w2, 0);
                        var generationForNextUse = _readField(memoryStream, w3, 0);
                        items[index++] = new FreeXRefItem(new PdfObjectId(objNmr, (int)generationForNextUse - 1, true));
                    }
                    else if (type == 1)
                    {
                        var byteOffset = _readField(memoryStream, w2, 0);
                        var generationNumber = _readField(memoryStream, w3, 0);
                        items[index++] = new ClassicXRefItem(new PdfObjectId(objNmr, (int)generationNumber), byteOffset);
                    }
                    else if (type == 2)
                    {
                        var objectNumberOfStream = _readField(memoryStream, w2, 0);
                        var indexWithinStream = _readField(memoryStream, w3, 0);
                        items[index++] = new CompressedXRefItem(
                            Id: new PdfObjectId(objNmr, 0),
                            ObjectStreamId: new PdfObjectId((int)objectNumberOfStream, 0),
                            ObjectIndex: (int)indexWithinStream
                        );
                    }
                    else
                        throw new ParseException("XRef stream field type other than 0, 1 or 2 is not supported.");
                }
            }
        }
        return items;
    }

    private static byte _readType(Stream stream, byte width)
    {
        if (width == 0)
            return 1;

        Span<byte> buffer = stackalloc byte[width];
        stream.Read(buffer);

        return buffer[width - 1];
    }

    private static long _readField(Stream stream, byte width, long defaultValue)
    {
        if (width == 0)
            return defaultValue;

        Span<byte> buffer = stackalloc byte[width];
        stream.Read(buffer);

        long number = 0;
        for (int i = 0; i < buffer.Length; i++)
            number |= (long)buffer[i] << ( ( width - i - 1 ) * 8 );
        return number;
    }

    private static (byte W1, byte W2, byte W3) _getWidths(IPdfStreamObject stream)
    {
        if (!stream.TryGetValue<PdfArray>(PdfNames.W, out var w) || w.Count != 3)
            throw new ParseException("XRef stream did not contain a /W key with an array of size 3.");

        var integers = w.OfType<PdfNumber>().ToArray();
        if (integers.Length != 3)
            throw new ParseException("The W array of the XRef stream contained other values than integers");

        return ((byte)integers[0], (byte)integers[1], (byte)integers[2]);
    }

    private static SectionIndex[] _getSections(IPdfStreamObject stream)
    {
        if (!stream.TryGetValue<PdfArray>(PdfNames.Index, out var indexesArray))
        {
            if (!stream.TryGetValue<PdfNumber>(PdfNames.Size, out var size) || size.Value < 0)
                throw new ParseException("XRef stream did not contain a /Size key with a zero or higher integer value.");
            return [new SectionIndex(0, (int)size.Value)];
        }

        if (indexesArray.Count % 2 != 0)
            throw new ParseException("The /Index array of the XRef stream dictionary contains an un-even amount of items.");

        var indexes = new SectionIndex[indexesArray.Count / 2];

        for (int i = 0; i < indexesArray.Count / 2; i++)
        {
            if (!indexesArray.TryGetValue<PdfNumber>(i * 2, out var firstObjectNumber))
                throw new ParseException("The /Index array contained something else than an integer.");
            if (!indexesArray.TryGetValue<PdfNumber>((i * 2) + 1, out var numberOfEntries))
                throw new ParseException("The /Index array contained something else than an integer.");

            indexes[i] = new SectionIndex((int)firstObjectNumber.Value, (int)numberOfEntries.Value);
        }

        return indexes;
    }

    private record struct SectionIndex(int FirstObjectNumber, int NumberOfEntries);
}
