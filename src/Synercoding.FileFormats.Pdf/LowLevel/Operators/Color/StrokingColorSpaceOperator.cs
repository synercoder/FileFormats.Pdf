namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing stroking color space change operator (CS)
    /// </summary>
    public struct StrokingColorSpaceOperator
    {
        /// <summary>
        /// Constructor for <see cref="RgbNonStrokingColorOperator"/>.
        /// </summary>
        /// <param name="colorspace">The colorspace to use.</param>
        public StrokingColorSpaceOperator(PdfName colorspace)
        {
            ColorSpace = colorspace;
        }

        /// <summary>
        /// The <see cref="PdfName"/> of the colorspace that will be set
        /// </summary>
        public PdfName ColorSpace { get; }
    }
}
