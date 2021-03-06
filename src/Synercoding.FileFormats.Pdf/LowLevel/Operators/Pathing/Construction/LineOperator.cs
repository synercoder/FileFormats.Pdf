using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a line operator (l)
    /// </summary>
    public struct LineOperator
    {
        /// <summary>
        /// Constructor for a <see cref="LineOperator"/>
        /// </summary>
        /// <param name="x">The X coordinate of the line end</param>
        /// <param name="y">The Y coordinate of the line end</param>
        public LineOperator(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructor for a <see cref="LineOperator"/>
        /// </summary>
        /// <param name="point">The end point of the line</param>
        public LineOperator(Point point)
        {
            point = point.ConvertTo(Unit.Points);

            X = point.X.Raw;
            Y = point.Y.Raw;
        }

        /// <summary>
        /// The X coordinate of the line end
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y coordinate of the line end
        /// </summary>
        public double Y { get; }
    }
}
