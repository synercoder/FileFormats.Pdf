using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal static class PdfFooter
{
    public static (Trailer Trailer, XRefTable XRefTable) Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, ObjectReader reader)
    {
        var startXRef = _findStartXRef(pdfBytesProvider, pdfStart);

        var (trailer, table) = _read(pdfStart, startXRef, pdfBytesProvider, reader);
        var lastTrailer = trailer;

        while (trailer.Prev.HasValue)
        {
            var (oldTrailer, previousTable) = _read(pdfStart, trailer.Prev.Value, pdfBytesProvider, reader);

            table = previousTable.Merge(table);
            trailer = oldTrailer;
        }

        return (lastTrailer, table);
    }

    private static long _findStartXRef(IPdfBytesProvider pdfBytesProvider, long pdfStart)
    {
        long offset = pdfBytesProvider.Length - 17;
        while (offset > pdfStart)
        {
            pdfBytesProvider.Seek(offset, SeekOrigin.Begin);

            if (!pdfBytesProvider.TryRead(9, out byte[] startXRef))
                throw ParseException.UnexpectedEOF();

            if (startXRef is [0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66]) // startxref
            {
                long xrefPosition = 0;
                pdfBytesProvider.SkipWhiteSpace();

                while (pdfBytesProvider.TryRead(out byte b) && b >= 0x30 && b <= 0x39)
                {
                    xrefPosition *= 10;
                    xrefPosition += b - 0x30;
                }

                return xrefPosition;
            }

            offset--;
        }

        throw new ParseException("Begin of pdf data reached before startxref could be found.");
    }

    private static (Trailer Trailer, XRefTable XRefTable) _read(long pdfStart, long xrefPosition, IPdfBytesProvider pdfBytesProvider, ObjectReader reader)
    {
        pdfBytesProvider.Seek(pdfStart + xrefPosition, SeekOrigin.Begin);

        var items = new List<XRefItem>();

        var parser = new Parser(new Tokenizer(pdfBytesProvider, reader.Settings.Logger), reader.Settings.Logger);

        if (pdfBytesProvider.TryPeek(4, out var xrefBytes) && xrefBytes is [0x78, 0x72, 0x65, 0x66])
        {
            // simple xref
            pdfBytesProvider
                .Skip(4)
                .ReadEndOfLineMarker();

            while (_tryReadXRefSectionStart(pdfBytesProvider, out var startAndCount))
            {
                var (startNumber, count) = startAndCount;
                pdfBytesProvider.ReadEndOfLineMarker();
                items.AddRange(_getItems(pdfBytesProvider, startNumber, count));
            }

            var table = new XRefTable(items);

            pdfBytesProvider.SkipWhiteSpace();

            if (!pdfBytesProvider.IsTrailerNext(true))
                throw new ParseException("Expected trailer keyword, but it was not found.");

            var trailerDictionary = parser.ReadDictionary();
            var trailer = new Trailer(trailerDictionary);

            if (trailer.XRefStm.HasValue)
            {
                parser.Tokenizer.Position = trailer.XRefStm.Value;
                var objStreamWrapper = parser.ReadObject<IPdfStreamObject>();

                var objStream = new ObjectStream(objStreamWrapper.Value, reader);
                table = table.Merge(new XRefTable(objStream.AsXRefItems(objStreamWrapper.Id)));
            }

            return (trailer, table);
        }
        else
        {
            var streamObjectWrap = parser.ReadObject<IPdfStreamObject>();

            var streamObject = streamObjectWrap.Value;

            var xrefData = streamObject.DecodeData(reader);

            var xRefItems = XRefStream.ParseStream(streamObject, reader);

            var trailer = new Trailer(streamObject);
            var table = new XRefTable(xRefItems);

            // Sometimes the xrefstream object itself is not referenced inside an xref table. Ensure it is by always setting it
            table.SetItem(new ClassicXRefItem(streamObjectWrap.Id, xrefPosition));

            return (trailer, table);
        }
    }

    private static bool _tryReadXRefSectionStart(IPdfBytesProvider pdfBytesProvider, out (int StartNumber, int Count) startAndCount)
    {
        var position = pdfBytesProvider.Position;

        if (_tryReadInteger(pdfBytesProvider, out int startNumber) && _tryReadInteger(pdfBytesProvider.Skip(1), out int count))
        {
            startAndCount = (startNumber, count);
            return true;
        }

        startAndCount = default;

        pdfBytesProvider.Seek(position, SeekOrigin.Begin);
        return false;
    }

    private static IEnumerable<XRefItem> _getItems(IPdfBytesProvider pdfBytesProvider, int startNumber, int count)
    {
        var items = new XRefItem[count];
        for (int i = startNumber; i < ( startNumber + count ); i++)
        {
            var offset = _readLong(pdfBytesProvider);
            pdfBytesProvider.ReadByte();
            var generation = _readInteger(pdfBytesProvider);
            pdfBytesProvider.ReadByte();
            var free = pdfBytesProvider.ReadByte() == (byte)'f';
            pdfBytesProvider.SkipWhiteSpace();

            items[i - startNumber] = free
                ? new FreeXRefItem(new PdfObjectId(i, generation))
                : new ClassicXRefItem(new PdfObjectId(i, generation), offset);
        }

        return items;
    }

    private static long _readLong(IPdfBytesProvider pdfBytesProvider)
    {
        if (_tryReadLong(pdfBytesProvider, out long number))
            return number;

        throw new ParseException("Invalid byte sequence while reading an long.");
    }

    private static bool _tryReadLong(IPdfBytesProvider pdfBytesProvider, out long number)
    {
        var position = pdfBytesProvider.Position;
        bool found = false;
        number = 0;
        while (true)
        {
            if (!pdfBytesProvider.TryPeek(out byte peek) || ByteUtils.IsWhiteSpace(peek))
            {
                if (!found)
                    pdfBytesProvider.Seek(position, SeekOrigin.Begin);

                return found;
            }

            var b = pdfBytesProvider.ReadByte();

            if (b < 0x30 || b > 0x39)
                return false;

            found = true;
            number *= 10;
            number += ( b - 0x30 );
        }
    }

    private static bool _tryReadInteger(IPdfBytesProvider pdfBytesProvider, out int number)
    {
        var position = pdfBytesProvider.Position;
        bool found = false;
        number = 0;
        while (true)
        {
            if (!pdfBytesProvider.TryPeek(out byte peek) || ByteUtils.IsWhiteSpace(peek))
            {
                if (!found)
                    pdfBytesProvider.Seek(position, SeekOrigin.Begin);

                return found;
            }

            var b = pdfBytesProvider.ReadByte();

            if (b < 0x30 || b > 0x39)
                return false;

            found = true;
            number *= 10;
            number += ( b - 0x30 );
        }
    }

    private static int _readInteger(IPdfBytesProvider pdfBytesProvider)
    {
        if (_tryReadInteger(pdfBytesProvider, out int number))
            return number;

        throw new ParseException("Invalid byte sequence while reading an integer.");
    }
}
