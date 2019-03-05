namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    public static class MatrixExtensions
    {
        public static Matrix Rotate(this Matrix matrix, double degrees)
        {
            return matrix.Multiply(new RotateMatrix(degrees));
        }

        public static Matrix Scale(this Matrix matrix, double x, double y)
        {
            return matrix.Multiply(new ScaleMatrix(x, y));
        }

        public static Matrix Skew(this Matrix matrix, double a, double b)
        {
            return matrix.Multiply(new SkewMatrix(a, b));
        }

        public static Matrix Translate(this Matrix matrix, double x, double y)
        {
            return matrix.Multiply(new TranslationMatrix(x, y));
        }
    }
}
