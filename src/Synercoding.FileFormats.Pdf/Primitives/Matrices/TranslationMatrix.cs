namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Matrix for a translation operation
    /// </summary>
    public class TranslationMatrix : Matrix
    {
        /// <summary>
        /// Constructor for <see cref="TranslationMatrix"/>
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public TranslationMatrix(double x, double y)
            : base(1, 0, 0, 1, x, y)
        { }
    }
}
