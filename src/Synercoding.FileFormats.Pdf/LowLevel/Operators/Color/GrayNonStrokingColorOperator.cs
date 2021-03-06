using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a gray non stroking operator (g)
    /// </summary>
    public struct GrayNonStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="GrayNonStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public GrayNonStrokingColorOperator(GrayColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public GrayColor Color { get; init; }
    }
}
