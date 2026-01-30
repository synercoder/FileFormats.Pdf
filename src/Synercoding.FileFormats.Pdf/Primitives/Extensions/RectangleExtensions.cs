using Synercoding.FileFormats.Pdf.Content;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;

internal static class RectangleExtensions
{
    public static PdfArray ToArray(this Rectangle rectangle)
        => new PdfArray()
        {
            new PdfNumber(rectangle.LLX.AsRaw(Unit.Points)),
            new PdfNumber(rectangle.LLY.AsRaw(Unit.Points)),
            new PdfNumber(rectangle.URX.AsRaw(Unit.Points)),
            new PdfNumber(rectangle.URY.AsRaw(Unit.Points))
        };

    /// <summary>
    /// Convert a <see cref="Rectangle"/> to a transformation matrix
    /// </summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to use</param>
    /// <returns>Returns a <see cref="Matrix"/> representing the provided <see cref="Rectangle"/>.</returns>
    public static Matrix AsPlacementMatrix(this Rectangle rectangle)
    {
        rectangle = rectangle.ConvertTo(Unit.Points);
        return new Matrix(
            rectangle.URX.Raw - rectangle.LLX.Raw, 0,
            0, rectangle.URY.Raw - rectangle.LLY.Raw,
            rectangle.LLX.Raw, rectangle.LLY.Raw);
    }
}
