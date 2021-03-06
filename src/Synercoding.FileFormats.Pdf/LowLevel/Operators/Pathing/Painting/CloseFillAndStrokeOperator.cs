namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting
{
    /// <summary>
    /// Struct representing a close, fill and stroke operator (b or b*)
    /// </summary>
    public struct CloseFillAndStrokeOperator
    {
        /// <summary>
        /// Constructor for <see cref="CloseFillAndStrokeOperator"/>
        /// </summary>
        /// <param name="fillRule">The <see cref="Painting.FillRule"/> to use to determine what area to fill.</param>
        public CloseFillAndStrokeOperator(FillRule fillRule)
        {
            FillRule = fillRule;
        }

        /// <summary>
        /// The <see cref="Painting.FillRule"/> to use during filling.
        /// </summary>
        public FillRule FillRule { get; }
    }
}
