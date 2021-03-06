using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    internal class Catalog : IPdfObject
    {
        private bool _isWritten;

        public Catalog(PdfReference id, PageTree pageTree)
        {
            Reference = id;
            PageTree = pageTree;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        public PageTree PageTree { get; }

        internal uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
                throw new InvalidOperationException("Object is already written to stream.");

            var position = (uint)stream.Position;

            stream.IndirectDictionary(this, static (catalog, dictionary) =>
            {
                dictionary
                    .Type(ObjectType.Catalog)
                    .Write(PdfName.Get("Pages"), catalog.PageTree.Reference);
            });
            _isWritten = true;

            return position;
        }
    }
}
