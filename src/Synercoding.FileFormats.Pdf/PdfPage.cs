using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Helpers;
using Synercoding.FileFormats.Pdf.PdfInternals.Objects;
using Synercoding.FileFormats.Pdf.PdfInternals.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Matrices;
using System.Collections.Generic;
using System.IO;
using ImageSharp = SixLabors.ImageSharp;

namespace Synercoding.FileFormats.Pdf
{
    public class PdfPage
    {
        private int _pageCounter = 0;
        private readonly Dictionary<string, Image> _images = new Dictionary<string, Image>();
        private readonly TableBuilder _tableBuilder;

        private Rectangle _trimBox = Rectangle.Zero;
        private Rectangle _bleedBox = Rectangle.Zero;
        private Rectangle _cropBox = Rectangle.Zero;

        internal PdfPage(TableBuilder tableBuilder)
        {
            _tableBuilder = tableBuilder;
            ContentStream = new ContentStream(tableBuilder.ReserveId());
        }

        internal IReadOnlyDictionary<string, Image> Images => _images;

        internal ContentStream ContentStream { get; }

        public Rectangle MediaBox { get; set; } = Sizes.A4Portrait;

        public Rectangle CropBox
        {
            get => _cropBox.Equals(Rectangle.Zero) ? MediaBox : _cropBox;
            set => _cropBox = value;
        }

        public Rectangle BleedBox
        {
            get => _bleedBox.Equals(Rectangle.Zero) ? CropBox : _bleedBox;
            set => _bleedBox = value;
        }

        public Rectangle TrimBox
        {
            get => _trimBox.Equals(Rectangle.Zero) ? CropBox : _trimBox;
            set => _trimBox = value;
        }

        public PdfPage AddImage(ImageSharp.Image<Rgba32> image, Rectangle rectangle)
        {
            return _addImage(image, rectangle, true);
        }

        public PdfPage AddImage(byte[] image, Rectangle rectangle)
        {
            return _addImage(ImageSharp.Image.Load(image), rectangle, false);
        }

        public PdfPage AddImage(Stream image, Rectangle rectangle)
        {
            return _addImage(ImageSharp.Image.Load(image), rectangle, false);
        }

        public PdfPage AddImage(ImageSharp.Image<Rgba32> image, Matrix matrix)
        {
            return _addImage(image, matrix, true);
        }

        public PdfPage AddImage(byte[] image, Matrix matrix)
        {
            return _addImage(ImageSharp.Image.Load(image), matrix, false);
        }

        public PdfPage AddImage(Stream image, Matrix matrix)
        {
            return _addImage(ImageSharp.Image.Load(image), matrix, false);
        }

        private PdfPage _addImage(ImageSharp.Image<Rgba32> image, Rectangle rectangle, bool clone)
        {
            var matrix = new Matrix(rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY);

            return _addImage(image, matrix, clone);
        }

        private PdfPage _addImage(ImageSharp.Image<Rgba32> image, Matrix matrix, bool clone)
        {
            var key = _addImageToResources(image, clone);
            ContentStream.AddImage(key, matrix);

            return this;
        }

        private string _addImageToResources(ImageSharp.Image<Rgba32> image, bool clone)
        {
            var key = "Im" + System.Threading.Interlocked.Increment(ref _pageCounter).ToString().PadLeft(6, '0');
            var id = _tableBuilder.ReserveId();

            if (clone)
            {
                image = image.Clone();
            }

            _images.Add(key, new Image(id, image));

            return key;
        }
    }
}