using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    public interface IPdfObject : IStreamWriteable, IDisposable
    {
        PdfReference Reference { get; }
        bool IsWritten { get; }
    }
}
