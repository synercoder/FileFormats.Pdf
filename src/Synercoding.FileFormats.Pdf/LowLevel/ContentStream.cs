using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    public class ContentStream : IPdfObject, IDisposable
    {
        private readonly MemoryStream _innerStream;
        private readonly PdfStream _streamWrapper;

        private bool _isWritten;

        public ContentStream(PdfReference id)
        {
            _innerStream = new MemoryStream();
            _streamWrapper = new PdfStream(_innerStream);
            Reference = id;
        }

        public PdfReference Reference { get; }

        internal bool IsWritten => _isWritten;

        internal uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            _innerStream.Position = 0;
            stream.IndirectStream(this, _innerStream);
            _isWritten = true;

            return position;
        }

        public void Dispose()
        {
            _streamWrapper.Dispose();
        }

        public ContentStream SaveState()
        {
            _streamWrapper.Write('q').NewLine();

            return this;
        }

        public ContentStream RestoreState()
        {
            _streamWrapper.Write('Q').NewLine();

            return this;
        }

        public ContentStream CTM(Matrix matrix)
        {
            _streamWrapper
                .Write(matrix.A)
                .Space()
                .Write(matrix.B)
                .Space()
                .Write(matrix.C)
                .Space()
                .Write(matrix.D)
                .Space()
                .Write(matrix.E)
                .Space()
                .Write(matrix.F)
                .Space()
                .Write("cm")
                .NewLine();

            return this;
        }

        public ContentStream Paint(PdfName resource)
        {
            _streamWrapper.Write(resource).Space().Write("Do").NewLine();

            return this;
        }
    }
}
