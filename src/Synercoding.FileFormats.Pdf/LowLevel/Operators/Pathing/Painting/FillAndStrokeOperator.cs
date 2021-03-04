namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting
{
    public struct FillAndStrokeOperator
    {
        public FillAndStrokeOperator(FillRule fillRule)
            => FillRule = fillRule;

        public FillRule FillRule { get; }
    }
}
