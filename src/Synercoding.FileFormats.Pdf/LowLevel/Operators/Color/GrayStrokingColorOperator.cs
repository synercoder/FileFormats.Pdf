using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a gray stroking operator (G)
    /// </summary>
    public struct GrayStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="GrayStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public GrayStrokingColorOperator(GrayColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public GrayColor Color { get; init; }
    }
}
