using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    /// <summary>
    /// Interface representing a pdf object
    /// </summary>
    public interface IPdfObject : IStreamWriteable, IDisposable
    {
        /// <summary>
        /// A pdf reference object that can be used to reference to this object
        /// </summary>
        PdfReference Reference { get; }
        /// <summary>
        /// Indicator to check whether this object has been written to the PDF stream
        /// </summary>
        bool IsWritten { get; }
    }
}
