namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    public struct MiterLimitOperator
    {
        public MiterLimitOperator(double limit)
        {
            Limit = limit;
        }

        public double Limit { get; }
    }
}
