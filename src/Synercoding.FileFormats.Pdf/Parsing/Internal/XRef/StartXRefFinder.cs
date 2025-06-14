using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal class StartXRefFinder
{
    public long FindStartXRef(IPdfBytesProvider pdfBytesProvider, long pdfStart)
    {
        if (pdfBytesProvider.Length < 9 + pdfStart)
            throw new ParseException("Begin of pdf data reached before startxref could be found.");
            
        long startOffset = Math.Max(pdfStart, pdfBytesProvider.Length - 17);
        if (startOffset >= pdfBytesProvider.Length)
            startOffset = pdfBytesProvider.Length - 9; // Must have at least 9 bytes for "startxref"
            
        long offset = startOffset;
        while (offset >= pdfStart && offset <= pdfBytesProvider.Length - 9)
        {
            pdfBytesProvider.Seek(offset, SeekOrigin.Begin);

            if (!pdfBytesProvider.TryRead(9, out byte[] startXRef))
            {
                offset--;
                continue;
            }

            if (startXRef is [0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66]) // startxref
            {
                long xrefPosition = 0;
                pdfBytesProvider.SkipWhiteSpace();

                while (pdfBytesProvider.TryRead(out byte b) && b >= 0x30 && b <= 0x39)
                {
                    xrefPosition *= 10;
                    xrefPosition += b - 0x30;
                }

                if (xrefPosition == 0 && !pdfBytesProvider.TryPeek(out byte peek))
                    // Edge case: only "startxref" keyword without number
                    throw new ParseException("Begin of pdf data reached before startxref could be found.");

                return xrefPosition;
            }

            offset--;
        }

        throw new ParseException("Begin of pdf data reached before startxref could be found.");
    }
}