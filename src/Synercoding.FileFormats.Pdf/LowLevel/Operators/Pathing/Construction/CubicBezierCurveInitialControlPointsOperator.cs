using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a cubic bézier curve operator (v)
    /// </summary>
    public struct CubicBezierCurveInitialControlPointsOperator
    {
        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="x2">The X coordinate of the second control point</param>
        /// <param name="y2">The Y coordinate of the second control point</param>
        /// <param name="x3">The X coordinate of the end point of the curve</param>
        /// <param name="y3">The Y coordinate of the end point of the curve</param>
        public CubicBezierCurveInitialControlPointsOperator(double x2, double y2, double x3, double y3)
        {
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
        }

        /// <summary>
        /// Constructor for <see cref="CubicBezierCurveDualControlPointsOperator"/>
        /// </summary>
        /// <param name="cp2">The second control point</param>
        /// <param name="endpoint">The end point of the curve</param>
        public CubicBezierCurveInitialControlPointsOperator(Point cp2, Point endpoint)
        {
            cp2 = cp2.ConvertTo(Unit.Points);
            endpoint = endpoint.ConvertTo(Unit.Points);

            X2 = cp2.X.Raw;
            Y2 = cp2.Y.Raw;
            X3 = endpoint.X.Raw;
            Y3 = endpoint.Y.Raw;
        }

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
