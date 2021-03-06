using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a cubic bézier curve operator (y)
    /// </summary>
    public struct CubicBezierCurveFinalControlPointsOperator
    {
        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="x1">The X coordinate of the first control point</param>
        /// <param name="y1">The Y coordinate of the first control point</param>
        /// <param name="x3">The X coordinate of the end point of the curve</param>
        /// <param name="y3">The Y coordinate of the end point of the curve</param>
        public CubicBezierCurveFinalControlPointsOperator(double x1, double y1, double x3, double y3)
        {
            X1 = x1;
            Y1 = y1;
            X3 = x3;
            Y3 = y3;
        }

        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="cp1">The first control point</param>
        /// <param name="endpoint">The end point of the curve</param>
        public CubicBezierCurveFinalControlPointsOperator(Point cp1, Point endpoint)
        {
            cp1 = cp1.ConvertTo(Unit.Points);
            endpoint = endpoint.ConvertTo(Unit.Points);

            X1 = cp1.X.Raw;
            Y1 = cp1.Y.Raw;
            X3 = endpoint.X.Raw;
            Y3 = endpoint.Y.Raw;
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
        /// The X coordinate of the end point of the curve
        /// </summary>
        public double X3 { get; init; }

        /// <summary>
        /// The Y coordinate of the end point of the curve
        /// </summary>
        public double Y3 { get; init; }
    }
}
