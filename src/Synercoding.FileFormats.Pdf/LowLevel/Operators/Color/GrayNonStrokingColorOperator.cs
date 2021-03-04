namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct GrayNonStrokingColorOperator
    {
        public GrayNonStrokingColorOperator(Graphics.GrayColor color)
        {
            Gray = color.Gray;
        }

        public double Gray { get; }
    }
}
