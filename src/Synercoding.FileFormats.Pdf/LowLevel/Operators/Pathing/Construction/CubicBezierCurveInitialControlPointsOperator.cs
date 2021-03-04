using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    public struct CubicBezierCurveInitialControlPointsOperator
    {
        public CubicBezierCurveInitialControlPointsOperator(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public CubicBezierCurveInitialControlPointsOperator(Point cp1, Point final)
        {
            cp1 = cp1.ConvertTo(Unit.Points);
            final = final.ConvertTo(Unit.Points);

            X1 = cp1.X.Raw;
            Y1 = cp1.Y.Raw;
            X2 = final.X.Raw;
            Y2 = final.Y.Raw;
        }

        public double X1 { get; }

        public double Y1 { get; }

        public double X2 { get; }

        public double Y2 { get; }
    }
}
