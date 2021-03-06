namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    /// <summary>
    /// Struct representing a line width operator (w)
    /// </summary>
    public struct LineWidthOperator
    {
        /// <summary>
        /// Constructor for a <see cref="LineWidthOperator"/>
        /// </summary>
        /// <param name="width">The width of the line</param>
        public LineWidthOperator(double width)
        {
            Width = width;
        }

        /// <summary>
        /// The width of the line
        /// </summary>
        public double Width { get; }
    }
}
