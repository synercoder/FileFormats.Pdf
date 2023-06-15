namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions;

internal static class StreamFilterExtensions
{
    public static PdfName ToPdfName(this StreamFilter streamFilter)
    {
        return streamFilter switch
        {
            StreamFilter.DCTDecode => PdfName.Get("DCTDecode"),
            _ => throw new NotImplementedException(),
        };
    }
}
