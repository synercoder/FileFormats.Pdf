using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    public struct RectangleOperator
    {
        public RectangleOperator(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleOperator(Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);

            X = rectangle.LLX.Raw;
            Y = rectangle.LLY.Raw;
            Width = rectangle.Width.Raw;
            Height = rectangle.Height.Raw;
        }

        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
    }
}
