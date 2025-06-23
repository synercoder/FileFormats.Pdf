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

        pdfBytesProvider.Seek(0, SeekOrigin.Begin);

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
                    throw new UnexpectedEndOfFileException();
                if (!pdfBytesProvider.TryRead(out byte dot))
                    throw new UnexpectedEndOfFileException();
                if (dot != 0x2E)
                    throw new UnexpectedByteException(0x2E, dot);
                if (!pdfBytesProvider.TryRead(out byte minor))
                    throw new UnexpectedEndOfFileException();

                return new PdfHeader()
                {
                    PdfStart = offset,
                    Version = new PdfVersion((byte)(major - (byte)'0'), (byte)(minor - (byte)'0'))
                };
            }
            else if (b == 0x25) // %
            {
                offset = pdfBytesProvider.Position - 1;
                index = 1;
            }
            else if (index != 0)
            {
                index = 0;
            }
        }

        throw new ParseException("End of pdf data reached before pdf header could be found.");
    }
}
