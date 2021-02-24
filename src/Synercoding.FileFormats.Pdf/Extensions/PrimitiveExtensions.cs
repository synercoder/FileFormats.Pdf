using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    public static class PrimitiveExtensions
    {
        public static Matrix AsPlacementMatrix(this Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);
            return new Matrix(
                rectangle.URX.Raw - rectangle.LLX.Raw, 0,
                0, rectangle.URY.Raw - rectangle.LLY.Raw,
                rectangle.LLX.Raw, rectangle.LLY.Raw);
        }
    }
}
