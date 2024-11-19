using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf.Extensions;

/// <summary>
/// Extension methods for <see cref="IPageContentContext"/>
/// </summary>
public static class IPageContentContextExtensions
{
    /// <summary>
    /// Add a JPG stream directly to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="jpgStream">The stream containing a JPG image</param>
    /// <param name="originalWidth">The width of the JPG in the <paramref name="jpgStream"/>.</param>
    /// <param name="originalHeight">The height of the JPG in the <paramref name="jpgStream"/>.</param>
    /// <param name="colorSpace">The colorspace of the JPG in the <paramref name="jpgStream"/>.</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddJpgUnsafe(this IPageContentContext context, System.IO.Stream jpgStream, int originalWidth, int originalHeight, ColorSpace colorSpace)
    {
        var name = context.RawContentStream.Resources.AddJpgUnsafe(jpgStream, originalWidth, originalHeight, colorSpace);

        context.RawContentStream.Paint(name);

        return context;
    }

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="stream">The stream containing the image</param>
    /// <param name="matrix">The placement matrix to use</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream, Matrix matrix)
    {
        return context.WrapInState((stream, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.stream);
        });
    }

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="stream">The stream containing the image</param>
    /// <param name="rectangle">The rectangle of where to place the image.</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream, Rectangle rectangle)
        => context.AddImage(stream, rectangle.AsPlacementMatrix());

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="stream">The stream containing the image</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream)
    {
        using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);

        return context.AddImage(image);
    }

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="image">The image to place</param>
    /// <param name="matrix">The placement matrix to use</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image<Rgba32> image, Matrix matrix)
    {
        return context.WrapInState((image, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.image);
        });
    }

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="image">The image to place</param>
    /// <param name="rectangle">The rectangle of where to place the image.</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image<Rgba32> image, Rectangle rectangle)
        => context.AddImage(image, rectangle.AsPlacementMatrix());

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="image">The image to place</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image<Rgba32> image)
    {
        var name = context.RawContentStream.Resources.AddImage(image);

        context.RawContentStream.Paint(name);

        return context;
    }

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="image">The image to place</param>
    /// <param name="rectangle">The rectangle of where to place the image.</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, Image image, Rectangle rectangle)
        => context.AddImage(image, rectangle.AsPlacementMatrix());

    /// <summary>
    /// Add an image to the page
    /// </summary>
    /// <param name="context">The context to add the image to.</param>
    /// <param name="image">The image to place</param>
    /// <param name="matrix">The placement matrix to use</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddImage(this IPageContentContext context, Image image, Matrix matrix)
    {
        return context.WrapInState((image, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.image);
        });
    }

    /// <summary>
    /// Start a shape operation on the page
    /// </summary>
    /// <param name="context">The context to add the shape to.</param>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddShapes(this IPageContentContext context, Action<IShapeContentContext> shapeOperations)
        => context.AddShapes(shapeOperations, static (operations, context) => operations(context));

    /// <summary>
    /// Start a shape operation on the page
    /// </summary>
    /// <param name="context">The context to add the shape to.</param>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>A task that resolves to <paramref name="context"/> to enable chaining operations</returns>
    public static Task<IPageContentContext> AddShapesAsync(this IPageContentContext context, Func<IShapeContentContext, Task> shapeOperations)
        => context.AddShapesAsync(shapeOperations, static (operations, context) => operations(context));

    /// <summary>
    /// Start a text operation on the page
    /// </summary>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="textOperations">The text operations to execute</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddText(this IPageContentContext context, Action<ITextContentContext> textOperations)
        => context.AddText(textOperations, static (operations, context) => operations(context));

    /// <summary>
    /// Start a text operation on the page
    /// </summary>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="textOperations">The text operations to execute</param>
    /// <returns>A task that resolves to <paramref name="context"/> to enable chaining operations</returns>
    public static Task<IPageContentContext> AddTextAsync(this IPageContentContext context, Func<ITextContentContext, Task> textOperations)
        => context.AddTextAsync(textOperations, static (operations, context) => operations(context));

    /// <summary>
    /// Show text on the page using the parameters provided
    /// </summary>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="text">The text to show</param>
    /// <param name="font">The font to use</param>
    /// <param name="size">The font size to use</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size)
        => context.AddText(text, font, size, ops => { });

    /// <summary>
    /// Show text on the page using the parameters provided
    /// </summary>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="text">The text to show</param>
    /// <param name="font">The font to use</param>
    /// <param name="size">The font size to use</param>
    /// <param name="location">The location to place the text</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size, Point location)
        => context.AddText(text, font, size, location, static (location, ops) => ops.MoveToStartNextLine(location.X.AsRaw(Unit.Points), location.Y.AsRaw(Unit.Points)));

    /// <summary>
    /// Show text on the page using the parameters provided
    /// </summary>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="text">The text to show</param>
    /// <param name="font">The font to use</param>
    /// <param name="size">The font size to use</param>
    /// <param name="extraOperations">The extra text operations to execute</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size, Action<ITextContentContext> extraOperations)
        => context.AddText(text, font, size, extraOperations, static (extraOperations, context) => extraOperations(context));

    /// <summary>
    /// Show text on the page using the parameters provided
    /// </summary>
    /// <typeparam name="T">The type of the data to provide to the <paramref name="extraOperations"/></typeparam>
    /// <param name="context">The context to add the text to.</param>
    /// <param name="text">The text to show</param>
    /// <param name="font">The font to use</param>
    /// <param name="size">The font size to use</param>
    /// <param name="data">the data to provide to <paramref name="extraOperations"/></param>
    /// <param name="extraOperations">The extra text operations to execute</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IPageContentContext AddText<T>(this IPageContentContext context, string text, Font font, double size, T data, Action<T, ITextContentContext> extraOperations)
    {
        return context.AddText((text, font, size, data, extraOperations), static (quintuple, context) =>
        {
            var (text, font, size, data, extraOperations) = quintuple;
            context.SetFontAndSize(font, size);

            extraOperations(data, context);

            var lines = StringHelper.SplitOnNewLines(text).ToArray();

            // if no leading parameter is set, and the text spans multiple lines, set textleading to the font size.
            if (lines.Length > 1 && context.GraphicState.TextLeading == default)
                context.SetTextLeading(size);

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                    context.ShowText(lines[i]);
                else
                    context.ShowTextOnNextLine(lines[i]);
            }
        });
    }
}
