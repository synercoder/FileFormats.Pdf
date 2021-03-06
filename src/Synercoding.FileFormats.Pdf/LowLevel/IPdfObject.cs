namespace Synercoding.FileFormats.Pdf.LowLevel
{
    /// <summary>
    /// Interface representing a pdf object
    /// </summary>
    public interface IPdfObject
    {
        /// <summary>
        /// A pdf reference object that can be used to reference to this object
        /// </summary>
        PdfReference Reference { get; }
    }
}
