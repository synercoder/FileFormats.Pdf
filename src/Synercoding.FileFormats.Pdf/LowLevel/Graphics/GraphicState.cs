using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    public sealed class GraphicsState
    {
        public double LineWidth { get; set; } = 1;
        public Color? Fill { get; set; } = null;
        public Color? Stroke { get; set; } = null;
        public Dash Dash { get; set; } = new Dash();
        public double MiterLimit { get; set; } = 10;
        public LineCapStyle LineCap { get; set; } = LineCapStyle.ButtCap;
        public LineJoinStyle LineJoin { get; set; } = LineJoinStyle.MiterJoin;
        public FillRule FillRule { get; set; } = FillRule.NonZeroWindingNumber;

        internal GraphicsState Clone()
            => new GraphicsState()
            {
                LineWidth = LineWidth,
                Fill = Fill,
                Stroke = Stroke,
                Dash = new Dash()
                {
                    Array = Dash.Array,
                    Phase = Dash.Phase
                },
                MiterLimit = MiterLimit,
                LineCap = LineCap,
                LineJoin = LineJoin
            };
    }
}
