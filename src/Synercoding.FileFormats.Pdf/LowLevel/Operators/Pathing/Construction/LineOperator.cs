using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    public struct LineOperator
    {
        public LineOperator(double x, double y)
        {
            X = x;
            Y = y;
        }

        public LineOperator(Point point)
        {
            point = point.ConvertTo(Unit.Points);

            X = point.X.Raw;
            Y = point.Y.Raw;
        }

        public double X { get; }
        public double Y { get; }
    }
}
