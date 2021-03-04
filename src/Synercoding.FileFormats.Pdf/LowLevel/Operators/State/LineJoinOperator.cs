using Synercoding.FileFormats.Pdf.LowLevel.Graphics;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    public struct LineJoinOperator
    {
        public LineJoinOperator(LineJoinStyle style)
        {
            Style = style;
        }

        public LineJoinStyle Style { get; }
    }
}
