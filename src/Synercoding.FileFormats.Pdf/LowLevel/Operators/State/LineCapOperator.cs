using Synercoding.FileFormats.Pdf.LowLevel.Graphics;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    /// <summary>
    /// Struct representing a line cap operator (J)
    /// </summary>
    public struct LineCapOperator
    {
        /// <summary>
        /// Constructor for <see cref="LineCapOperator"/>
        /// </summary>
        /// <param name="style">The style to use</param>
        public LineCapOperator(LineCapStyle style)
        {
            Style = style;
        }

        /// <summary>
        /// The <see cref="LineCapStyle"/> to use
        /// </summary>
        public LineCapStyle Style { get; }
    }
}
