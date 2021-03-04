namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    public struct DashOperator
    {
        public DashOperator(double[] array, double phase)
        {
            Array = array;
            Phase = phase;
        }

        public double[] Array { get; }
        public double Phase { get; }
    }
}
