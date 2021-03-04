namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct NonStrokingColorSpaceOperator
    {
        public NonStrokingColorSpaceOperator(PdfName colorspace)
        {
            ColorSpace = colorspace;
        }

        public PdfName ColorSpace { get; }
    }
}
