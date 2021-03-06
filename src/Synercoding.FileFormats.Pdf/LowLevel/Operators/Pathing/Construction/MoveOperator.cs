using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a move operator (m)
    /// </summary>
    public struct MoveOperator
    {
        /// <summary>
        /// Constructor for a <see cref="MoveOperator"/>
        /// </summary>
        /// <param name="x">The X coordinate of the move operator</param>
        /// <param name="y">The Y coordinate of the move operator</param>
        public MoveOperator(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructor for a <see cref="MoveOperator"/>
        /// </summary>
        /// <param name="point">The end point of the line</param>
        public MoveOperator(Point point)
        {
            point = point.ConvertTo(Unit.Points);

            X = point.X.Raw;
            Y = point.Y.Raw;
        }

        /// <summary>
        /// The X coordinate of the move operator
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y coordinate of the move operator
        /// </summary>
        public double Y { get; }
    }
}
