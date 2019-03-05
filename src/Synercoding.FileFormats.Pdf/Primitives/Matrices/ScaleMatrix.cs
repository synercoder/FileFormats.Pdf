namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    public class ScaleMatrix : Matrix
    {
        public ScaleMatrix(double x, double y)
            : base(x, 0, 0, y, 0, 0)
        { }
    }
}
