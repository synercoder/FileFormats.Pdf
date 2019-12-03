using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.Extensions;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    public class Image : IPdfObject, IDisposable
    {
        private readonly SixLabors.ImageSharp.Image _image;
        private bool _disposed;

        internal Image(PdfReference id, SixLabors.ImageSharp.Image image)
        {
            Reference = id;
            _image = image;
        }

        public PdfReference Reference { get; private set; }

        public bool IsWritten { get; private set; }

        public void Dispose()
        {
            if(!_disposed)
            {
                _image.Dispose();
                _disposed = true;
            }
        }

        public uint WriteToStream(Stream stream)
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(nameof(_image), "Internal image is already disposed");
            }
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            IsWritten = true;

            var position = (uint)stream.Position;

            using (var ms = new MemoryStream())
            {
                _image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                {
                    Quality = 100,
                    Subsample = SixLabors.ImageSharp.Formats.Jpeg.JpegSubsample.Ratio420
                });
                ms.Position = 0;

                stream.IndirectStream(Reference, ms, dictionary =>
                {
                    dictionary
                        .Type(ObjectType.XObject)
                        .SubType(XObjectSubType.Image)
                        .Write("/Width", _image.Width)
                        .Write("/Height", _image.Height)
                        .Write("/ColorSpace", "/DeviceRGB")
                        .Write("/BitsPerComponent", 8)
                        .Write("/Decode", "[0.0 1.0 0.0 1.0 0.0 1.0]");
                },
                StreamFilter.DCTDecode);
            }

            return position;
        }
    }
}
