using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal static class PdfHeaderWriter
{
    public static long WriteTo(PdfStream stream, PdfVersion pdfVersion)
    {
        _ = stream ?? throw new ArgumentNullException(nameof(stream));
        _ = pdfVersion ?? throw new ArgumentNullException(nameof(pdfVersion));

        var position = stream.Position;

        stream
            .Write($"%PDF-{pdfVersion.Major}.{pdfVersion.Minor}")
            .NewLine()
            .WriteByte(ByteUtils.PERCENT_SIGN)
            .Write([0x81, 0x82, 0x83, 0x84]) // 4 bytes of 128 or higher value to indicate file contains binary data
            .NewLine();

        return position;
    }
}
