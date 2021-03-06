using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    /// <summary>
    /// Extension methods for primitives
    /// </summary>
    public static class PrimitiveExtensions
    {
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
}
