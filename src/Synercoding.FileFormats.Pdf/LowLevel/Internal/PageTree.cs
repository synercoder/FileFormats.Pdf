using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf.LowLevel.Internal
{
    internal class PageTree
    {
        private readonly List<PdfPage> _pages = new List<PdfPage>();

        public PageTree(PdfReference id)
        {
            Reference = id;
        }

        /// <summary>
        /// A pdf reference object that can be used to reference to this object
        /// </summary>
        public PdfReference Reference { get; }

        public void AddPage(PdfPage pdfObject)
            => _pages.Add(pdfObject);

        public int PageCount
            => _pages.Count;

        internal IReadOnlyCollection<PdfPage> Pages
            => _pages.AsReadOnly();
    }
}
