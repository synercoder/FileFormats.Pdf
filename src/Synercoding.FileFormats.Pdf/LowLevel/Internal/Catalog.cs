namespace Synercoding.FileFormats.Pdf.LowLevel.Internal
{
    internal class Catalog
    {
        public Catalog(PdfReference id, PageTree pageTree)
        {
            Reference = id;
            PageTree = pageTree;
        }

        /// <summary>
        /// A pdf reference object that can be used to reference to this object
        /// </summary>
        public PdfReference Reference { get; }

        public PageTree PageTree { get; }
    }
}
