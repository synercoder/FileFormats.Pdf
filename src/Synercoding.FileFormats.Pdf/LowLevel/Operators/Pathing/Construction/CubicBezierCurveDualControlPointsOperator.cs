using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    public struct CubicBezierCurveDualControlPointsOperator
    {
        public CubicBezierCurveDualControlPointsOperator(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
        }

        public CubicBezierCurveDualControlPointsOperator(Point cp1, Point cp2, Point final)
        {
            cp1 = cp1.ConvertTo(Unit.Points);
            cp2 = cp2.ConvertTo(Unit.Points);
            final = final.ConvertTo(Unit.Points);

            X1 = cp1.X.Raw;
            Y1 = cp1.Y.Raw;
            X2 = cp2.X.Raw;
            Y2 = cp2.Y.Raw;
            Y3 = final.Y.Raw;
            X3 = final.X.Raw;
        }

        public double X1 { get; init; }

        public double Y1 { get; init; }

        public double X2 { get; init; }

        public double Y2 { get; init; }

        public double X3 { get; init; }

        public double Y3 { get; init; }
    }
}
