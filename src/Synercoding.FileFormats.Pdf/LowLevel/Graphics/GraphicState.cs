using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Internal;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    /// <summary>
    /// Class representing the graphic state of a content stream
    /// </summary>
    public sealed class GraphicsState
    {
        internal GraphicsState(PageResources resources)
        {
            Resources = resources;
        }

        internal PageResources Resources { get; }

        /// <summary>
        /// The width of the line
        /// </summary>
        public double LineWidth { get; set; } = 1;

        /// <summary>
        /// The <see cref="Color"/> of the fill of the path
        /// </summary>
        /// <remarks>If null no filling will occur.</remarks>
        public Color? Fill { get; set; } = null;

        /// <summary>
        /// The <see cref="Color"/> of the stroke of the path
        /// </summary>
        /// <remarks>If null no stroking will occur.</remarks>
        public Color? Stroke { get; set; } = null;

        /// <summary>
        /// The dash settings of the path
        /// </summary>
        public Dash Dash { get; set; } = new Dash();

        /// <summary>
        /// The miter limit
        /// </summary>
        public double MiterLimit { get; set; } = 10;

        /// <summary>
        /// The <see cref="LineCapStyle"/> of the path
        /// </summary>
        public LineCapStyle LineCap { get; set; } = LineCapStyle.ButtCap;

        /// <summary>
        /// The <see cref="LineJoinStyle"/> of the path
        /// </summary>
        public LineJoinStyle LineJoin { get; set; } = LineJoinStyle.MiterJoin;

        /// <summary>
        /// The <see cref="Operators.Pathing.Painting.FillRule"/> used to determine what areas to fill.
        /// </summary>
        public FillRule FillRule { get; set; } = FillRule.NonZeroWindingNumber;

        internal GraphicsState Clone()
            => new GraphicsState(Resources)
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
