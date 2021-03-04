namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct CmykStrokingColorOperator
    {
        public CmykStrokingColorOperator(Graphics.CmykColor color)
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
