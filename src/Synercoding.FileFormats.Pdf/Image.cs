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
        private bool _disposed;

        internal Image(PdfReference id, SixLabors.ImageSharp.Image image)
        {
            Reference = id;

            var ms = new MemoryStream();
            image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
            {
                Quality = 100,
                ColorType = SixLabors.ImageSharp.Formats.Jpeg.JpegColorType.YCbCrRatio444
            });
            Width = image.Width;
            Height = image.Height;
            ms.Position = 0;
            RawStream = ms;
        }

        internal Image(PdfReference id, Stream jpgStream, int width, int height)
        {
            Reference = id;

            Width = width;
            Height = height;
            RawStream = jpgStream;
        }

        internal Stream RawStream { get; private set; }

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
                RawStream.Dispose();
                _disposed = true;
            }
        }
    }
}
