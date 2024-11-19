using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class PdfHeader
{
    public long PdfStart { get; init; }
    public required PdfVersion Version { get; init; }

    public static PdfHeader Parse(IPdfBytesProvider pdfBytesProvider)
    {
        long offset = 0;

        int index = 0;
        while (pdfBytesProvider.TryRead(out byte b))
        {
            if (index == 1 && b == 0x50) // p
            {
                index++;
            }
            else if (index == 2 && b == 0x44) // d
            {
                index++;
            }
            else if (index == 3 && b == 0x46) // f
            {
                index++;
            }
            else if (index == 4 && b == 0x2D) // -
            {
                if (!pdfBytesProvider.TryRead(out byte major))
                    throw ParseException.UnexpectedEOF();
                if (!pdfBytesProvider.TryRead(out byte dot))
                    throw ParseException.UnexpectedEOF();
                if (dot != 0x2E)
                    throw new ParseException(0x2E, dot);
                if (!pdfBytesProvider.TryRead(out byte minor))
                    throw ParseException.UnexpectedEOF();

                return new PdfHeader()
                {
                    PdfStart = offset,
                    Version = new PdfVersion(major, minor)
                };
            }
            else if (b == 0x25) // %
            {
                offset = pdfBytesProvider.Position - 1;
                index = 1;
            }
        }

        throw new ParseException("End of pdf data reached before pdf header could be found.");
    }
}
