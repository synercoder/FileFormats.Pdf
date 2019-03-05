namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Extension class for <see cref="Matrix"/>
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Apply a rotation operation on the <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">The matrix to apply the operation on</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public static Matrix Rotate(this Matrix matrix, double degrees)
        {
            return matrix.Multiply(new RotateMatrix(degrees));
        }

        /// <summary>
        /// Apply a scale operation on the <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">The matrix to apply the operation on</param>
        /// <param name="x">The X amount to scale</param>
        /// <param name="y">The Y amount to scale</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public static Matrix Scale(this Matrix matrix, double x, double y)
        {
            return matrix.Multiply(new ScaleMatrix(x, y));
        }

        /// <summary>
        /// Apply a skew operation on the <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">The matrix to apply the operation on</param>
        /// <param name="a">The amount of degrees to skew (top left direction)</param>
        /// <param name="b">The amount of degrees to skew (bottom right direction)</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public static Matrix Skew(this Matrix matrix, double a, double b)
        {
            return matrix.Multiply(new SkewMatrix(a, b));
        }

        /// <summary>
        /// Apply a translation operation on the <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">The matrix to apply the operation on</param>
        /// <param name="x">The X distance to translate</param>
        /// <param name="y">The Y distance to translate</param>
        /// <returns>The new <see cref="Matrix"/></returns>
        public static Matrix Translate(this Matrix matrix, double x, double y)
        {
            return matrix.Multiply(new TranslationMatrix(x, y));
        }
    }
}
