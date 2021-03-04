namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct RgbStrokingColorOperator
    {
        public RgbStrokingColorOperator(Graphics.RgbColor color)
        {
            Red = color.Red;
            Green = color.Green;
            Blue = color.Blue;
        }

        public double Red { get; }
        public double Green { get; }
        public double Blue { get; }
    }
}
