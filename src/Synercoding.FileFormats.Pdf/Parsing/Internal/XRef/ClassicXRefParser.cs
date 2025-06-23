using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Reflection.PortableExecutable;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal class ClassicXRefParser : IXRefParser
{
    public bool CanParse(IPdfBytesProvider pdfBytesProvider, long xrefPosition)
    {
        var originalPosition = pdfBytesProvider.Position;
        try
        {
            pdfBytesProvider.Seek(xrefPosition, SeekOrigin.Begin);
            return pdfBytesProvider.TryPeek(4, out var xrefBytes) && xrefBytes is [0x78, 0x72, 0x65, 0x66]; // "xref"
        }
        catch
        {
            return false;
        }
        finally
        {
            pdfBytesProvider.Seek(originalPosition, SeekOrigin.Begin);
        }
    }

    public Trailer GetTrailer(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ReaderSettings readerSettings)
    {
        _ = _readTableAt(pdfBytesProvider, pdfStart, xrefPosition);

        var parser = new Parser(new Lexer(pdfBytesProvider, readerSettings.Logger), null, readerSettings.Logger);
        parser.Lexer.ReadOrThrow(TokenKind.Trailer);
        var trailerDictionary = parser.ReadDictionary(null);
        return new Trailer(trailerDictionary, readerSettings);
    }

    public (Trailer Trailer, XRefTable XRefTable) Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ObjectReader reader)
    {
        var table = _readTableAt(pdfBytesProvider, pdfStart, xrefPosition);

        pdfBytesProvider.SkipWhiteSpace();

        var trailer = GetTrailer(pdfBytesProvider, pdfStart, xrefPosition, reader.Settings);

        // Handle XRefStm if present (hybrid PDF)
        if (trailer.XRefStm.HasValue)
        {
            var parser = new Parser(new Lexer(pdfBytesProvider, reader.Settings.Logger), null, reader.Settings.Logger);
            parser.Lexer.Position = trailer.XRefStm.Value;
            var objStreamWrapper = parser.ReadObject<IPdfStreamObject>();
            var objStream = new ObjectStream(objStreamWrapper.Value, reader);
            table = table.Merge(new XRefTable(objStream.AsXRefItems(objStreamWrapper.Id)));
        }

        return (trailer, table);
    }

    private XRefTable _readTableAt(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition)
    {
        pdfBytesProvider.Seek(pdfStart + xrefPosition, SeekOrigin.Begin);

        var items = new List<XRefItem>();

        // Skip "xref" keyword
        pdfBytesProvider
            .Skip(4)
            .ReadEndOfLineMarker();

        // Read xref sections
        while (_tryReadXRefSectionStart(pdfBytesProvider, out var startAndCount))
        {
            var (startNumber, count) = startAndCount;
            pdfBytesProvider.ReadEndOfLineMarker();
            items.AddRange(_getXRefItems(pdfBytesProvider, startNumber, count));
        }

        return new XRefTable(items);
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

    private static IEnumerable<XRefItem> _getXRefItems(IPdfBytesProvider pdfBytesProvider, int startNumber, int count)
    {
        var items = new XRefItem[count];
        for (int i = startNumber; i < startNumber + count; i++)
        {
            var offset = _readLong(pdfBytesProvider);
            pdfBytesProvider.ReadByte();
            var generation = _readInteger(pdfBytesProvider);
            pdfBytesProvider.ReadByte();
            var free = pdfBytesProvider.ReadByte() == (byte)'f';
            pdfBytesProvider.SkipWhiteSpace();

            items[i - startNumber] = free
                ? new FreeXRefItem(new PdfObjectId(i, generation, true))
                : new ClassicXRefItem(new PdfObjectId(i, generation), offset);
        }

        return items;
    }

    private static long _readLong(IPdfBytesProvider pdfBytesProvider)
    {
        if (_tryReadLong(pdfBytesProvider, out long number))
            return number;

        throw new ParseException("Invalid byte sequence while reading a long.");
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
            number += b - 0x30;
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
            number += b - 0x30;
        }
    }

    private static int _readInteger(IPdfBytesProvider pdfBytesProvider)
    {
        if (_tryReadInteger(pdfBytesProvider, out int number))
            return number;

        throw new ParseException("Invalid byte sequence while reading an integer.");
    }
}
