using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a stroking cmyk operation (K)
    /// </summary>
    public struct CmykStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="CmykStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public CmykStrokingColorOperator(CmykColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public CmykColor Color { get; init; }
    }
}
