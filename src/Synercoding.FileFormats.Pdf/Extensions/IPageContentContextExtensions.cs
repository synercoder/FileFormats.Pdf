using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Extensions;
public static class IPageContentContextExtensions
{
    public static IPageContentContext AddJpgUnsafe(this IPageContentContext context, System.IO.Stream jpgStream, int originalWidth, int originalHeight, ColorSpace colorSpace)
    {
        var name = context.RawContentStream.Resources.AddJpgUnsafe(jpgStream, originalWidth, originalHeight, colorSpace);

        context.RawContentStream.Paint(name);

        return context;
    }

    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream, Matrix matrix)
    {
        return context.WrapInState((stream, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.stream);
        });
    }

    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream, Rectangle rectangle)
        => context.AddImage(stream, rectangle.AsPlacementMatrix());

    public static IPageContentContext AddImage(this IPageContentContext context, System.IO.Stream stream)
    {
        using var image = SixLabors.ImageSharp.Image.Load(stream);

        return context.AddImage(image);
    }

    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image image, Matrix matrix)
    {
        return context.WrapInState((image, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.image);
        });
    }

    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image image, Rectangle rectangle)
        => context.AddImage(image, rectangle.AsPlacementMatrix());

    public static IPageContentContext AddImage(this IPageContentContext context, SixLabors.ImageSharp.Image image)
    {
        var name = context.RawContentStream.Resources.AddImage(image);

        context.RawContentStream.Paint(name);

        return context;
    }

    public static IPageContentContext AddImage(this IPageContentContext context, Image image, Rectangle rectangle)
        => context.AddImage(image, rectangle.AsPlacementMatrix());

    public static IPageContentContext AddImage(this IPageContentContext context, Image image, Matrix matrix)
    {
        return context.WrapInState((image, matrix), static (tuple, context) =>
        {
            context.ConcatenateMatrix(tuple.matrix);
            context.AddImage(tuple.image);
        });
    }

    public static IPageContentContext AddShapes(this IPageContentContext context, Action<IShapeContext> shapeOperations)
        => context.AddShapes(shapeOperations, static (operations, context) => operations(context));

    public static Task<IPageContentContext> AddShapesAsync(this IPageContentContext context, Func<IShapeContext, Task> shapeOperations)
        => context.AddShapesAsync(shapeOperations, static (operations, context) => operations(context));

    public static IPageContentContext AddText(this IPageContentContext context, Action<ITextContentContext> textOperations)
        => context.AddText(textOperations, static (operations, context) => operations(context));

    public static Task<IPageContentContext> AddTextAsync(this IPageContentContext context, Func<ITextContentContext, Task> textOperations)
        => context.AddTextAsync(textOperations, static (operations, context) => operations(context));

    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size)
        => context.AddText(text, font, size, ops => { });

    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size, Point location)
        => context.AddText(text, font, size, location, static (location, ops) =>
        {
            ops.MoveToStartNextLine(location.X.AsRaw(Unit.Points), location.Y.AsRaw(Unit.Points));
        });

    public static IPageContentContext AddText(this IPageContentContext context, string text, Font font, double size, Action<ITextContentContext> extraOperations)
        => context.AddText(text, font, size, extraOperations, static (extraOperations, context) => extraOperations(context));

    public static IPageContentContext AddText<T>(this IPageContentContext context, string text, Font font, double size, T data, Action<T, ITextContentContext> extraOperations)
    {
        return context.AddText((text, font, size, data, extraOperations), static (quintuple, context) =>
        {
            var (text, font, size, data, extraOperations) = quintuple;
            context.SetFontAndSize(font, size);

            extraOperations(data, context);

            var lines = StringHelper.SplitOnNewLines(text).ToArray();

            // if no leading parameter is set, and the text spans multiple lines, set textleading to the font size.
            if (lines.Length > 1 && context.TextLeading == default)
                context.SetTextLeading(size);

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    context.ShowText(lines[i]);
                }
                else
                {
                    context.ShowTextOnNextLine(lines[i]);
                }
            }
        });
    }
}
