using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Extensions;
public static class IShapeContextExtensions
{
    public static IShapeContentContext Rectangle(this IShapeContentContext context, Rectangle rectangle)
        => context.Rectangle(rectangle.LLX.AsRaw(Unit.Points), rectangle.LLY.AsRaw(Unit.Points), rectangle.Width.AsRaw(Unit.Points), rectangle.Height.AsRaw(Unit.Points));
}
