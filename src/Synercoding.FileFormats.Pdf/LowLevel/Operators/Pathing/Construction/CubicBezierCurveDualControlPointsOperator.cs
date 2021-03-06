using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a cubic bézier curve operator (c)
    /// </summary>
    public struct CubicBezierCurveDualControlPointsOperator
    {
        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="x1">The X coordinate of the first control point</param>
        /// <param name="y1">The Y coordinate of the first control point</param>
        /// <param name="x2">The X coordinate of the second control point</param>
        /// <param name="y2">The Y coordinate of the second control point</param>
        /// <param name="x3">The X coordinate of the end point of the curve</param>
        /// <param name="y3">The Y coordinate of the end point of the curve</param>
        public CubicBezierCurveDualControlPointsOperator(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
        }

        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="cp1">The first control point</param>
        /// <param name="cp2">The second control point</param>
        /// <param name="endpoint">The end point of the curve</param>
        public CubicBezierCurveDualControlPointsOperator(Point cp1, Point cp2, Point endpoint)
        {
            cp1 = cp1.ConvertTo(Unit.Points);
            cp2 = cp2.ConvertTo(Unit.Points);
            endpoint = endpoint.ConvertTo(Unit.Points);

            X1 = cp1.X.Raw;
            Y1 = cp1.Y.Raw;
            X2 = cp2.X.Raw;
            Y2 = cp2.Y.Raw;
            Y3 = endpoint.Y.Raw;
            X3 = endpoint.X.Raw;
        }

        /// <summary>
        /// The X coordinate of the first control point
        /// </summary>
        public double X1 { get; init; }

        /// <summary>
        /// The Y coordinate of the first control point
        /// </summary>
        public double Y1 { get; init; }

        /// <summary>
        /// The X coordinate of the second control point
        /// </summary>
        public double X2 { get; init; }

        /// <summary>
        /// The Y coordinate of the second control point
        /// </summary>
        public double Y2 { get; init; }

        /// <summary>
        /// The X coordinate of the end point of the curve
        /// </summary>
        public double X3 { get; init; }

        /// <summary>
        /// The Y coordinate of the end point of the curve
        /// </summary>
        public double Y3 { get; init; }
    }
}
