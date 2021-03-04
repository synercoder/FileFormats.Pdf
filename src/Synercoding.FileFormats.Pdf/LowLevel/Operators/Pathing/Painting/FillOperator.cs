namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting
{
    public struct FillOperator
    {
        public FillOperator(FillRule fillRule)
            => FillRule = fillRule;

        public FillRule FillRule { get; }
    }
}
