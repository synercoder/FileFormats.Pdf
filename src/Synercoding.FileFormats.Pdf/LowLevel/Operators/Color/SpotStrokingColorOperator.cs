using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a spot color stroking operator (SCN)
    /// </summary>
    public struct SpotStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="SpotStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public SpotStrokingColorOperator(SpotColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public SpotColor Color { get; init; }
    }
}
