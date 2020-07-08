using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    /// <summary>
    /// Extensions to use Values as input. These will be converted to points directly
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Create a matrix used for scaling
        /// </summary>
        /// <param name="x">The X scale</param>
        /// <param name="y">The Y scale</param>
        /// <returns>A scaled matrix</returns>
        public static Matrix CreateScaleMatrix(Value x, Value y)
            => new Matrix(
                x.ConvertTo(Unit.Points).Raw, 0,
                0, y.ConvertTo(Unit.Points).Raw,
                0, 0);

        /// <summary>
        /// Create a matrix used for translation
        /// </summary>
        /// <param name="x">The amount of X translation</param>
        /// <param name="y">The amount of Y translation</param>
        /// <returns>A matrix representing a translation</returns>
        public static Matrix CreateTranslationMatrix(Value x, Value y)
            => new Matrix(
                1, 0,
                0, 1,
                x.ConvertTo(Unit.Points).Raw, y.ConvertTo(Unit.Points).Raw);
    }
}
