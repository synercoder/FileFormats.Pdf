namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Matrix for a scale operation
    /// </summary>
    public class ScaleMatrix : Matrix
    {
        /// <summary>
        /// Constructor for <see cref="ScaleMatrix"/>
        /// </summary>
        /// <param name="x">The amount of X translation</param>
        /// <param name="y">The amount of Y translation</param>
        public ScaleMatrix(double x, double y)
            : base(x, 0, 0, y, 0, 0)
        { }
    }
}
