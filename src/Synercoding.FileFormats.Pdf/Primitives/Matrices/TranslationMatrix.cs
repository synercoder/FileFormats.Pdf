namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    public class TranslationMatrix : Matrix
    {
        public TranslationMatrix(double x, double y)
            : base(1, 0, 0, 1, x, y)
        { }
    }
}
