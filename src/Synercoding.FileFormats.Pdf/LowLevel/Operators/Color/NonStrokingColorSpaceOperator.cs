namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing non stroking color space change operator (cs)
    /// </summary>
    public struct NonStrokingColorSpaceOperator
    {
        /// <summary>
        /// Constructor for <see cref="NonStrokingColorSpaceOperator"/>.
        /// </summary>
        /// <param name="colorspace">The colorspace to use.</param>
        public NonStrokingColorSpaceOperator(PdfName colorspace)
        {
            ColorSpace = colorspace;
        }

        /// <summary>
        /// The <see cref="PdfName"/> of the colorspace that will be set
        /// </summary>
        public PdfName ColorSpace { get; }
    }
}
