using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    /// <summary>
    /// Struct representing a non stroking cmyk operation (k)
    /// </summary>
    public struct CmykNonStrokingColorOperator
    {
        /// <summary>
        /// Constructor for <see cref="CmykNonStrokingColorOperator"/>.
        /// </summary>
        /// <param name="color">The color to use.</param>
        public CmykNonStrokingColorOperator(CmykColor color)
        {
            Color = color;
        }

        /// <summary>
        /// The color used in the operation
        /// </summary>
        public CmykColor Color { get; init;  }
    }
}
