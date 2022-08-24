using Synercoding.FileFormats.Pdf.Internals;
using System;
using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    internal sealed class PageResources : IDisposable
    {
        private readonly Map<PdfName, Image> _images = new Map<PdfName, Image>();

        private int _imageCounter = 0;

        public IReadOnlyDictionary<PdfName, Image> Images
            => _images.Forward;

        public void Dispose()
        {
            foreach (var kv in _images)
                kv.Value.Dispose();

            _images.Clear();
        }

        public PdfName AddImageToResources(Image image)
        {
            if (_images.Reverse.Contains(image))
                return _images.Reverse[image];

            var key = "Im" + System.Threading.Interlocked.Increment(ref _imageCounter).ToString().PadLeft(6, '0');

            var pdfName = PdfName.Get(key);

            _images.Add(pdfName, image);

            return pdfName;
        }
    }
}
