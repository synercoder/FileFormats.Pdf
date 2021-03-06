using Synercoding.FileFormats.Pdf.LowLevel.Graphics;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    /// <summary>
    /// Struct representing a line join operator (j)
    /// </summary>
    public struct LineJoinOperator
    {
        /// <summary>
        /// Constructor for <see cref="LineJoinOperator"/>.
        /// </summary>
        /// <param name="style">The line join style to use.</param>
        public LineJoinOperator(LineJoinStyle style)
        {
            Style = style;
        }

        /// <summary>
        /// The <see cref="LineJoinStyle"/> to use.
        /// </summary>
        public LineJoinStyle Style { get; }
    }
}
