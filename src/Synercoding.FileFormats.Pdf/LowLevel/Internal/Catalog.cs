namespace Synercoding.FileFormats.Pdf.LowLevel.Internal
{
    internal class Catalog : IPdfObject
    {
        public Catalog(PdfReference id, PageTree pageTree)
        {
            Reference = id;
            PageTree = pageTree;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        public PageTree PageTree { get; }
    }
}
