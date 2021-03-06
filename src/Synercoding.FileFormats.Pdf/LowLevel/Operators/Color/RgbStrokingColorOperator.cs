using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a rgb stroking operator (RG)
    /// </summary>
    public struct RgbStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="RgbStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public RgbStrokingColorOperator(RgbColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public RgbColor Color { get; init; }
    }
}
