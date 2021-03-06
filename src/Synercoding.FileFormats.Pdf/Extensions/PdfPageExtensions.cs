using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.Primitives;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    /// <summary>
    /// Extension class for <see cref="PdfPage"/>
    /// </summary>
    public static class PdfPageExtensions
    {
        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="image">The image to add</param>
        /// <param name="matrix">The placement matrix</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Image image, Matrix matrix)
        {
            page.ContentStream
                .SaveState()
                .CTM(matrix)
                .Paint(page.AddImageToResources(image))
                .RestoreState();

            return page;
        }

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="image">The image to add</param>
        /// <param name="rectangle">The placement rectangle</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Image image, Rectangle rectangle)
            => page.AddImage(image, rectangle.AsPlacementMatrix());

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="image">The image to add</param>
        /// <param name="matrix">The placement matrix</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, SixLabors.ImageSharp.Image image, Matrix matrix)
        {
            page.ContentStream
                .SaveState()
                .CTM(matrix)
                .Paint(page.AddImageToResources(image))
                .RestoreState();

            return page;
        }

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="image">The image to add</param>
        /// <param name="rectangle">The placement rectangle</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, SixLabors.ImageSharp.Image image, Rectangle rectangle)
            => page.AddImage(image, rectangle.AsPlacementMatrix());

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="imageStream">The image to add</param>
        /// <param name="matrix">The placement matrix</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Stream imageStream, Matrix matrix)
        {
            page.ContentStream
                .SaveState()
                .CTM(matrix)
                .Paint(page.AddImageToResources(imageStream))
                .RestoreState();

            return page;
        }

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="imageStream">The image to add</param>
        /// <param name="rectangle">The placement rectangle</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Stream imageStream, Rectangle rectangle)
            => page.AddImage(imageStream, rectangle.AsPlacementMatrix());

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="originalWidth">The original width of the image</param>
        /// <param name="originalHeight">The original height of the image</param>
        /// <param name="matrix">The placement matrix</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Stream jpgStream, int originalWidth, int originalHeight, Matrix matrix)
        {
            page.ContentStream
                .SaveState()
                .CTM(matrix)
                .Paint(page.AddImageToResources(jpgStream, originalWidth, originalHeight))
                .RestoreState();

            return page;
        }

        /// <summary>
        /// Add an image to the pdf page
        /// </summary>
        /// <param name="page">The page to add the image to</param>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="originalWidth">The original width of the image</param>
        /// <param name="originalHeight">The original height of the image</param>
        /// <param name="rectangle">The placement rectangle</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddImage(this PdfPage page, Stream jpgStream, int originalWidth, int originalHeight, Rectangle rectangle)
            => page.AddImage(jpgStream, originalWidth, originalHeight, rectangle.AsPlacementMatrix());

        /// <summary>
        /// Add shapes to the pdf page
        /// </summary>
        /// <param name="page">The page to add the shapes to</param>
        /// <param name="paintAction">The action painting the shapes</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddShapes(this PdfPage page, Action<IShapeContext> paintAction)
            => page.AddShapes(paintAction, static (action, context) => action(context));

        /// <summary>
        /// Add shapes to the pdf page
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="data"/></typeparam>
        /// <param name="page">The page to add the shapes to</param>
        /// <param name="data">Data that can be passed to the <paramref name="paintAction"/></param>
        /// <param name="paintAction">The action painting the shapes</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddShapes<T>(this PdfPage page, T data, Action<T, IShapeContext> paintAction)
        {
            using (var context = new ShapeContext(page.ContentStream, page.Resources))
                paintAction(data, context);

            return page;
        }
    }
}
