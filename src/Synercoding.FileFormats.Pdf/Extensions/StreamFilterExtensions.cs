using Synercoding.FileFormats.Pdf.PdfInternals;
using System;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    internal static class StreamFilterExtensions
    {
        public static string ToPdfName(this StreamFilter streamFilter)
        {
            switch(streamFilter)
            {
                case StreamFilter.DCTDecode:
                    return "/DCTDecode";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
