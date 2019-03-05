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
    /// <summary>
    /// A class that represents a pdf page 
    /// </summary>
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

        /// <summary>
        /// The media box of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle MediaBox { get; set; } = Sizes.A4Portrait;

        /// <summary>
        /// The cropbox of the <see cref="PdfPage"/>, defaults to <see cref="MediaBox"/>
        /// </summary>
        public Rectangle CropBox
        {
            get => _cropBox.Equals(Rectangle.Zero) ? MediaBox : _cropBox;
            set => _cropBox = value;
        }

        /// <summary>
        /// The bleedbox of the <see cref="PdfPage"/>, defaults to <see cref="CropBox"/>
        /// </summary>
        public Rectangle BleedBox
        {
            get => _bleedBox.Equals(Rectangle.Zero) ? CropBox : _bleedBox;
            set => _bleedBox = value;
        }

        /// <summary>
        /// The trimbox of the <see cref="PdfPage"/>, defaults to <see cref="CropBox"/>
        /// </summary>
        public Rectangle TrimBox
        {
            get => _trimBox.Equals(Rectangle.Zero) ? CropBox : _trimBox;
            set => _trimBox = value;
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(ImageSharp.Image<Rgba32> image, Rectangle rectangle)
        {
            return _addImage(image, rectangle, true);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(byte[] image, Rectangle rectangle)
        {
            return _addImage(ImageSharp.Image.Load(image), rectangle, false);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(Stream image, Rectangle rectangle)
        {
            return _addImage(ImageSharp.Image.Load(image), rectangle, false);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="matrix">The <see cref="Matrix"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(ImageSharp.Image<Rgba32> image, Matrix matrix)
        {
            return _addImage(image, matrix, true);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="matrix">The <see cref="Matrix"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(byte[] image, Matrix matrix)
        {
            return _addImage(ImageSharp.Image.Load(image), matrix, false);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="matrix">The <see cref="Matrix"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
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