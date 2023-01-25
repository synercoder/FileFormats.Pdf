using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// Class representing an image inside a pdf
    /// </summary>
    public sealed class Image : IDisposable
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
            ColorSpace = DeviceRGB.Instance.Name;
            DecodeArray = new double[] { 0, 1, 0, 1, 0, 1 };
            ms.Position = 0;
            RawStream = ms;
        }

        internal Image(PdfReference id, Stream jpgStream, int width, int height, ColorSpace colorSpace)
        {
            Reference = id;

            Width = width;
            Height = height;
            RawStream = jpgStream;

            var (csName, decodeArray) = colorSpace switch
            {
                DeviceCMYK cmyk => (cmyk.Name, new double[] { 0, 1, 0, 1, 0, 1, 0, 1 }),
                DeviceRGB rgb => (rgb.Name, new double[] { 0, 1, 0, 1, 0, 1 }),
                _ => throw new ArgumentOutOfRangeException(nameof(colorSpace), $"The provided color space {colorSpace} is currently not supported.")
            };

            ColorSpace = csName;
            DecodeArray = decodeArray;
        }

        internal Image(PdfReference id, Stream jpgStream, int width, int height, PdfName colorSpace, double[] decodeArray)
        {
            Reference = id;

            Width = width;
            Height = height;
            RawStream = jpgStream;
            ColorSpace = colorSpace;
            DecodeArray = decodeArray;
        }

        internal Stream RawStream { get; private set; }

        /// <summary>
        /// A pdf reference object that can be used to reference to this object
        /// </summary>
        public PdfReference Reference { get; private set; }

        /// <summary>
        /// The width of this <see cref="Image"/>
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of this <see cref="Image"/>
        /// </summary>
        public int Height { get; }

        public PdfName ColorSpace { get; }

        public double[] DecodeArray { get; }

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
