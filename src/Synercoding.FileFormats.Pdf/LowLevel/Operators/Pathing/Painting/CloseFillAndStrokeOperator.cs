namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting
{
    public struct CloseFillAndStrokeOperator
    {
        public CloseFillAndStrokeOperator(FillRule fillRule)
            => FillRule = fillRule;

        public FillRule FillRule { get; }
    }
}
