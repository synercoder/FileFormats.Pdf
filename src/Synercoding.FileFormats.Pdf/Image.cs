using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// Class representing an image inside a pdf
    /// </summary>
    public sealed class Image : IPdfObject, IDisposable
    {
        private readonly Stream _imageStream;
        private bool _disposed;
        private bool _isWritten;

        internal Image(PdfReference id, SixLabors.ImageSharp.Image image)
        {
            Reference = id;

            var ms = new MemoryStream();
            image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
            {
                Quality = 100,
                Subsample = SixLabors.ImageSharp.Formats.Jpeg.JpegSubsample.Ratio420
            });
            Width = image.Width;
            Height = image.Height;
            ms.Position = 0;
            _imageStream = ms;
        }

        internal Image(PdfReference id, Stream jpgStream, int width, int height)
        {
            Reference = id;

            Width = width;
            Height = height;
            _imageStream = jpgStream;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; private set; }

        /// <summary>
        /// The width of this <see cref="Image"/>
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of this <see cref="Image"/>
        /// </summary>
        public int Height { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _imageStream.Dispose();
                _disposed = true;
            }
        }

        internal bool TryWriteToStream(PdfStream stream, out uint position)
        {
            position = 0;

            if (_isWritten)
                return false;
            if (_disposed)
                throw new ObjectDisposedException(nameof(_imageStream), "Internal image is already disposed");

            position = (uint)stream.Position;

            stream.IndirectStream(this, _imageStream, this, static (image, dictionary) =>
            {
                dictionary
                    .Type(ObjectType.XObject)
                    .SubType(XObjectSubType.Image)
                    .Write(PdfName.Get("Width"), image.Width)
                    .Write(PdfName.Get("Height"), image.Height)
                    .Write(PdfName.Get("ColorSpace"), PdfName.Get("DeviceRGB"))
                    .Write(PdfName.Get("BitsPerComponent"), 8)
                    .Write(PdfName.Get("Decode"), 0f, 1f, 0f, 1f, 0f, 1f);
            },
            StreamFilter.DCTDecode);

            _isWritten = true;

            return true;
        }
    }
}
