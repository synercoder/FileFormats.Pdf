namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting
{
    /// <summary>
    /// Struct representing a fill operator (f or f*)
    /// </summary>
    public struct FillOperator
    {
        /// <summary>
        /// Constructor for a <see cref="FillAndStrokeOperator"/>
        /// </summary>
        /// <param name="fillRule">The <see cref="Painting.FillRule"/> to use to determine what area to fill.</param>
        public FillOperator(FillRule fillRule)
        {
            FillRule = fillRule;
        }


        /// <summary>
        /// The <see cref="Painting.FillRule"/> to use during filling.
        /// </summary>
        public FillRule FillRule { get; }
    }
}
