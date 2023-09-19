namespace Synercoding.FileFormats.Pdf.Extensions;

/// <summary>
/// Extensions for <see cref="IShapeContentContext"/>
/// </summary>
public static class IShapeContextExtensions
{
    /// <summary>
    /// Add a rectangle to the current path
    /// </summary>
    /// <param name="context">The context to execute this path on.</param>
    /// <param name="rectangle">The <see cref="Synercoding.Primitives.Rectangle"/> to add.</param>
    /// <returns>The same <paramref name="context"/> to enable chaining operations.</returns>
    public static IShapeContentContext Rectangle(this IShapeContentContext context, Rectangle rectangle)
        => context.Rectangle(rectangle.LLX.AsRaw(Unit.Points), rectangle.LLY.AsRaw(Unit.Points), rectangle.Width.AsRaw(Unit.Points), rectangle.Height.AsRaw(Unit.Points));
}
