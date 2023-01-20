using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Internal;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// This class represents a page in a pdf
    /// </summary>
    public sealed class PdfPage : IPdfObject, IDisposable
    {
        private readonly TableBuilder _tableBuilder;
        private readonly PageTree _parent;

        private int? _rotation;

        internal PdfPage(TableBuilder tableBuilder, PageTree parent)
        {
            _tableBuilder = tableBuilder;
            _parent = parent;
            _parent.AddPage(this);

            PageNumber = _parent.PageCount;
            Reference = tableBuilder.ReserveId();
            Resources = new PageResources(_tableBuilder);
            ContentStream = new ContentStream(tableBuilder.ReserveId(), Resources);
        }

        internal PdfReference Parent
            => _parent.Reference;

        internal PageResources Resources { get; }

        /// <summary>
        /// The number of the page
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// The content stream of this page
        /// </summary>
        public ContentStream ContentStream { get; }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        /// <summary>
        /// The rotation of how the page is displayed, must be in increments of 90
        /// </summary>
        public int? Rotation
        {
            get => _rotation;
            set
            {
                if (value is not null && value % 90 != 0)
                    throw new ArgumentOutOfRangeException(nameof(Rotation), value, "The provided value can only be increments of 90.");

                _rotation = value;
            }
        }

        /// <summary>
        /// The media box of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle MediaBox { get; set; } = Sizes.A4.AsRectangle();

        /// <summary>
        /// The cropbox of the <see cref="PdfPage"/>, defaults to <see cref="MediaBox"/>
        /// </summary>
        public Rectangle? CropBox { get; set; }

        /// <summary>
        /// The bleedbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? BleedBox { get; set; }

        /// <summary>
        /// The trimbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? TrimBox { get; set; }

        /// <summary>
        /// The artbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? Art { get; set; }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="image">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(SixLabors.ImageSharp.Image image)
        {
            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, image);

            return Resources.AddImage(pdfImage);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="width">The width of the image in the <paramref name="jpgStream"/></param>
        /// <param name="height">The height of the image in the <paramref name="jpgStream"/></param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(System.IO.Stream jpgStream, int width, int height)
        {
            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, jpgStream, width, height);

            return Resources.AddImage(pdfImage);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="imageStream">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(System.IO.Stream imageStream)
        {
            using var image = SixLabors.ImageSharp.Image.Load(imageStream);
            return AddImageToResources(image);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="image">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(Image image)
        {
            return Resources.AddImage(image);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Resources.Dispose();

            ContentStream.Dispose();
        }

        internal void MarkStdFontAsUsed(Type1StandardFont font)
        {
            Resources.AddStandardFont(font);
        }
    }
}
