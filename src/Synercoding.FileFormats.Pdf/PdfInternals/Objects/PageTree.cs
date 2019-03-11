using Synercoding.FileFormats.Pdf.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    internal class PageTree : IPdfObject
    {
        private readonly IList<IPdfObject> _pages;

        public PageTree(PdfReference id, IList<IPdfObject> pages)
        {
            Reference = id;
            _pages = pages;
        }

        public PdfReference Reference { get; private set; }

        public bool IsWritten { get; private set; }

        public uint WriteToStream(Stream stream)
        {
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            stream.IndirectDictionary(Reference, dictionary =>
            {
                dictionary
                    .Type(ObjectType.Pages)
                    .Write("/Kids", _pages.Select(p => p.Reference))
                    .Write("/Count", _pages.Count());
            });
            IsWritten = true;

            return position;
        }

        public void Dispose()
        {

        }
    }

}
