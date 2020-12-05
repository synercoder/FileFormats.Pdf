using SixLabors.ImageSharp.Processing;
using Synercoding.FileFormats.Pdf.PdfInternals.Objects;
using Synercoding.FileFormats.Pdf.PdfInternals.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
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
        private int _imageCounter = 0;
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
        public Rectangle MediaBox { get; set; } = Sizes.A4.AsRectangle();

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
        public PdfPage AddImage(ImageSharp.Image image, Rectangle rectangle)
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
        public PdfPage AddImage(ImageSharp.Image image, Matrix matrix)
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

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(Image image, Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);
            var matrix = new Matrix(
                rectangle.URX.Raw - rectangle.LLX.Raw, 0,
                0, rectangle.URY.Raw - rectangle.LLY.Raw,
                rectangle.LLX.Raw, rectangle.LLY.Raw);
            return AddImage(image, matrix);
        }

        /// <summary>
        /// Add image to the <see cref="PdfPage"/>
        /// </summary>
        /// <param name="image">The image to be added</param>
        /// <param name="matrix">The <see cref="Matrix"/> that represents the placement on the page</param>
        /// <returns>This <see cref="PdfPage"/> so calls can be chained.</returns>
        public PdfPage AddImage(Image image, Matrix matrix)
        {
            var key = _addImageToResources(image);
            ContentStream.AddImage(key, matrix);

            return this;
        }

        private PdfPage _addImage(ImageSharp.Image image, Rectangle rectangle, bool clone)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);
            var matrix = new Matrix(
                rectangle.URX.Raw - rectangle.LLX.Raw, 0,
                0, rectangle.URY.Raw - rectangle.LLY.Raw,
                rectangle.LLX.Raw, rectangle.LLY.Raw);

            return _addImage(image, matrix, clone);
        }

        private PdfPage _addImage(ImageSharp.Image image, Matrix matrix, bool clone)
        {
            var key = _addImageToResources(image, clone);
            ContentStream.AddImage(key, matrix);

            return this;
        }

        private string _addImageToResources(ImageSharp.Image image, bool clone)
        {
            if (clone)
            {
                image = image.Clone(ctx => { });
            }

            var id = _tableBuilder.ReserveId();

            return _addImageToResources(new Image(id, image));
        }

        private string _addImageToResources(Image image)
        {
            var key = "Im" + System.Threading.Interlocked.Increment(ref _imageCounter).ToString().PadLeft(6, '0');

            _images.Add(key, image);

            return key;
        }
    }
}