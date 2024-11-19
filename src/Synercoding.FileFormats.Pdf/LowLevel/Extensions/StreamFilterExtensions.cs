namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions;

internal static class StreamFilterExtensions
{
    public static PdfName ToPdfName(this StreamFilter streamFilter)
    {
        return streamFilter switch
        {
            StreamFilter.DCTDecode => PdfName.Get("DCTDecode"),
            StreamFilter.FlateDecode => PdfName.Get("FlateDecode"),
            _ => throw new NotImplementedException(),
        };
    }
}
