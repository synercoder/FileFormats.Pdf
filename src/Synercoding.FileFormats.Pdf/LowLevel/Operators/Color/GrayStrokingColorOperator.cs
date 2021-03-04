namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct GrayStrokingColorOperator
    {
        public GrayStrokingColorOperator(Graphics.GrayColor color)
        {
            Gray = color.Gray;
        }

        public double Gray { get; }
    }
}
