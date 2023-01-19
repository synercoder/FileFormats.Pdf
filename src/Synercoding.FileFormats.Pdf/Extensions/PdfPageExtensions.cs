using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
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
        /// Add text to the page.
        /// </summary>
        /// <param name="page">The page to add the text to.</param>
        /// <param name="text">The text to add.</param>
        /// <param name="point">The location of the text.</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddText(this PdfPage page, string text, Point point)
            => page.AddText(text, point, new TextState());

        /// <summary>
        /// Add text to the page.
        /// </summary>
        /// <param name="page">The page to add the text to.</param>
        /// <param name="text">The text to add.</param>
        /// <param name="point">The location of the text.</param>
        /// <param name="configureState">Configure the state of the text to place.</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddText(this PdfPage page, string text, Point point, Action<TextState> configureState)
        {
            var state = new TextState();
            configureState(state);

            return page.AddText(text, point, state);
        }

        /// <summary>
        /// Add text to the page.
        /// </summary>
        /// <param name="page">The page to add the text to.</param>
        /// <param name="text">The text to add.</param>
        /// <param name="point">The location of the text.</param>
        /// <param name="state">The state of the text to place.</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddText(this PdfPage page, string text, Point point, TextState state)
        {
            page.MarkStdFontAsUsed(state.Font);

            page.ContentStream
                .SaveState()
                .BeginText()
                .SetTextPosition(point)
                .SetFontAndSize(state.Font.LookupName, state.FontSize)
                .SetTextLeading(state.Leading ?? state.FontSize);

            if (state.Fill is Color fill)
                page.ContentStream.SetColorFill(fill);
            if (state.Stroke is Color stroke)
                page.ContentStream.SetColorStroke(stroke);
            if (state.LineWidth is double lineWidth)
                page.ContentStream.Write(new LineWidthOperator(lineWidth));
            if (state.LineCap is LineCapStyle lineCapStyle)
                page.ContentStream.Write(new LineCapOperator(lineCapStyle));
            if (state.LineJoin is LineJoinStyle lineJoinStyle)
                page.ContentStream.Write(new LineJoinOperator(lineJoinStyle));
            if (state.MiterLimit is double miterLimit)
                page.ContentStream.Write(new MiterLimitOperator(miterLimit));
            if (state.Dash is Dash dash)
                page.ContentStream.Write(new DashOperator(dash.Array, dash.Phase));
            if (state.CharacterSpacing is float charSpace)
                page.ContentStream.SetCharacterSpacing(charSpace);
            if (state.WordSpacing is float wordSpace)
                page.ContentStream.SetWordSpacing(wordSpace);
            if (state.HorizontalScaling is float horizontalScaling)
                page.ContentStream.SetHorizontalScaling(horizontalScaling);
            if (state.TextRise is float textRise)
                page.ContentStream.SetTextRise(textRise);
            if (state.RenderingMode is TextRenderingMode textRenderingMode)
                page.ContentStream.SetTextRenderMode(textRenderingMode);

            page.ContentStream
                .ShowText(text)
                .EndText()
                .RestoreState();

            return page;
        }

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
        /// <remarks>
        /// The <paramref name="jpgStream"/> is not checked, and is used as is. Make sure only streams that represent a JPG are used.
        /// </remarks>
        /// <param name="page">The page to add the image to</param>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="originalWidth">The original width of the image</param>
        /// <param name="originalHeight">The original height of the image</param>
        /// <param name="matrix">The placement matrix</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddJpgImageUnsafe(this PdfPage page, Stream jpgStream, int originalWidth, int originalHeight, Matrix matrix)
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
        /// <remarks>
        /// The <paramref name="jpgStream"/> is not checked, and is used as is. Make sure only streams that represent a JPG are used.
        /// </remarks>
        /// <param name="page">The page to add the image to</param>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="originalWidth">The original width of the image</param>
        /// <param name="originalHeight">The original height of the image</param>
        /// <param name="rectangle">The placement rectangle</param>
        /// <returns>The same <see cref="PdfPage"/> to chain other calls.</returns>
        public static PdfPage AddJpgImageUnsafe(this PdfPage page, Stream jpgStream, int originalWidth, int originalHeight, Rectangle rectangle)
            => page.AddJpgImageUnsafe(jpgStream, originalWidth, originalHeight, rectangle.AsPlacementMatrix());

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
