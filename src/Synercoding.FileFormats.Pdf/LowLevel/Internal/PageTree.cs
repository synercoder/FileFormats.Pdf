using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf.LowLevel.Internal
{
    internal class PageTree : IPdfObject
    {
        private readonly List<PdfPage> _pages = new List<PdfPage>();

        public PageTree(PdfReference id)
        {
            Reference = id;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        public void AddPage(PdfPage pdfObject)
            => _pages.Add(pdfObject);

        public int PageCount
            => _pages.Count;

        internal IReadOnlyCollection<PdfPage> Pages
            => _pages.AsReadOnly();
    }
}
