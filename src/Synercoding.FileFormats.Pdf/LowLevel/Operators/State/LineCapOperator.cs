using Synercoding.FileFormats.Pdf.LowLevel.Graphics;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    public struct LineCapOperator
    {
        public LineCapOperator(LineCapStyle style)
        {
            Style = style;
        }

        public LineCapStyle Style { get; }
    }
}
