namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    public struct LineWidthOperator
    {
        public LineWidthOperator(double width)
        {
            Width = width;
        }

        public double Width { get; }
    }
}
