using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a spot color non stroking operator (scn)
    /// </summary>
    public struct SpotNonStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="SpotNonStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public SpotNonStrokingColorOperator(SpotColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public SpotColor Color { get; init; }
    }
}
