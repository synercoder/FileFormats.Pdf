namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct CmykNonStrokingColorOperator
    {
        public CmykNonStrokingColorOperator(Graphics.CmykColor color)
        {
            Cyan = color.Cyan;
            Magenta = color.Magenta;
            Yellow = color.Yellow;
            Key = color.Key;
        }

        public double Cyan { get; }
        public double Magenta { get; }
        public double Yellow { get; }
        public double Key { get; }
    }
}
