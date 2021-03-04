using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf
{
    public interface IPath
    {
        IPath Move(double x, double y);
        IPath Move(Point point);
        IPath LineTo(double x, double y);
        IPath LineTo(Point point);
        IPath Rectangle(double x, double y, double width, double height);
        IPath Rectangle(Rectangle rectangle);
        IPath CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY);
        IPath CurveTo(Point cp1, Point cp2, Point final);
        IPath CurveToWithStartAnker(double cpX1, double cpY1, double finalX, double finalY);
        IPath CurveToWithStartAnker(Point cp, Point final);
        IPath CurveToWithEndAnker(double cpX1, double cpY1, double finalX, double finalY);
        IPath CurveToWithEndAnker(Point cp, Point final);

        IShapeContext Close();

    }
}
