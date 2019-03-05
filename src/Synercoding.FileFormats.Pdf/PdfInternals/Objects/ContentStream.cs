using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Matrices;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    internal class ContentStream : IPdfObject
    {
        private readonly Stream _stream = new MemoryStream();

        public ContentStream(PdfReference id)
        {
            Reference = id;
        }

        public PdfReference Reference { get; }

        public bool IsWritten { get; private set; }

        public ContentStream AddImage(string resourceKey, Matrix matrix)
        {
            _stream
                .Write("q") // Save graphics state
                .NewLine()
                .Write(matrix) // Apply matrix
                .NewLine()
                .Write("/" + resourceKey + " Do") // Paint image
                .NewLine()
                .Write("Q") // Restore graphics state
                .NewLine();

            return this;
        }

        public uint WriteToStream(Stream stream)
        {
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            _stream.Position = 0;
            stream.IndirectStream(Reference, _stream, _ => { });
            IsWritten = true;

            return position;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}