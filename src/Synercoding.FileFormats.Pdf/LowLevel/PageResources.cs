using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    internal sealed class PageResources : IDisposable
    {
        private readonly TableBuilder _tableBuilder;
        private readonly Map<PdfName, Image> _images;
        private readonly Dictionary<Type1StandardFont, PdfReference> _standardFonts;

        private int _imageCounter = 0;

        internal PageResources(TableBuilder tableBuilder)
        {
            _tableBuilder = tableBuilder;
            _images = new Map<PdfName, Image>();
            _standardFonts = new Dictionary<Type1StandardFont, PdfReference>();
        }

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

        internal void MarkStdFontAsUsed(Type1StandardFont font)
        {
            if (!_standardFonts.ContainsKey(font))
                _standardFonts[font] = _tableBuilder.ReserveId();
        }

        internal IReadOnlyCollection<(Type1StandardFont Font, PdfReference Reference)> FontReferences
        {
            get
            {
                return _standardFonts
                    .Select(kv => (kv.Key, kv.Value))
                    .ToArray();
            }
        }
    }
}
