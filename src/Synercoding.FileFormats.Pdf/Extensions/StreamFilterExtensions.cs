using Synercoding.FileFormats.Pdf.PdfInternals;
using System;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    internal static class StreamFilterExtensions
    {
        public static string ToPdfName(this StreamFilter streamFilter)
        {
            return streamFilter switch
            {
                StreamFilter.DCTDecode => "/DCTDecode",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
