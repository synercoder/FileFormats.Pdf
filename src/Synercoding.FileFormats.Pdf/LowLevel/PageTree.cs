using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    internal class PageTree : IPdfObject
    {
        private readonly IList<PdfPage> _pages;
        private bool _isWritten;

        public PageTree(PdfReference id)
            : this(id, new List<PdfPage>())
        { }

        public PageTree(PdfReference id, IList<PdfPage> pages)
        {
            Reference = id;
            _pages = pages;
        }

        public PdfReference Reference { get; }

        public void AddPage(PdfPage pdfObject)
            => _pages.Add(pdfObject);

        public uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            stream.IndirectDictionary(this, static (pageTree, dictionary) =>
            {
                dictionary
                    .Type(ObjectType.Pages)
                    .Write(PdfName.Get("Kids"), pageTree._pages.Select(static p => p.Reference))
                    .Write(PdfName.Get("Count"), pageTree._pages.Count());
            });
            _isWritten = true;

            return position;
        }
    }
}
