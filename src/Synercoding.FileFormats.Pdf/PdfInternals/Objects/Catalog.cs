using Synercoding.FileFormats.Pdf.Extensions;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    internal class Catalog : IPdfObject
    {
        public Catalog(PdfReference id, PdfReference pageTree)
        {
            Reference = id;
            PageTree = pageTree;
        }

        public PdfReference Reference { get; }

        public PdfReference PageTree { get; }

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
                    .Type(ObjectType.Catalog)
                    .Write("/Pages", PageTree);
            });
            IsWritten = true;

            return position;
        }

        public void Dispose()
        { }
    }
}
