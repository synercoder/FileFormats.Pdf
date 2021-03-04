namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Color
{
    public struct StrokingColorSpaceOperator
    {
        public StrokingColorSpaceOperator(PdfName colorspace)
        {
            ColorSpace = colorspace;
        }

        public PdfName ColorSpace { get; }
    }
}
