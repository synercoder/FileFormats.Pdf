using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    internal class PageTree : IPdfObject
    {
        private readonly IList<PdfPage> _pages = new List<PdfPage>();
        private readonly TableBuilder _tableBuilder;
        private bool _isWritten;

        public PageTree(PdfReference id, TableBuilder tableBuilder)
        {
            Reference = id;
            _tableBuilder = tableBuilder;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        public void AddPage(PdfPage pdfObject)
            => _pages.Add(pdfObject);

        public int PageCount
            => _pages.Count;

        internal uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }

            var position = stream.Position;
            stream.IndirectDictionary(Reference, this, static (pageTree, dictionary) =>
            {
                dictionary
                    .Type(ObjectType.Pages)
                    .Write(PdfName.Get("Kids"), pageTree._pages.Select(static p => p.Reference).ToArray())
                    .Write(PdfName.Get("Count"), pageTree._pages.Count);
            });
            _isWritten = true;

            return position;
        }
    }
}
