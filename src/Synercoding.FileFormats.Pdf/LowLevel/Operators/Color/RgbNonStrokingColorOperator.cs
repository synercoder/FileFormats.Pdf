using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a rgb non stroking operator (rg)
    /// </summary>
    public struct RgbNonStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="RgbNonStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public RgbNonStrokingColorOperator(RgbColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public RgbColor Color { get; init; }
    }
}
