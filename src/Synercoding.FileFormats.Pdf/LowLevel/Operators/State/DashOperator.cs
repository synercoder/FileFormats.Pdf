namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    /// <summary>
    /// Struct representing a dash operator (d)
    /// </summary>
    public struct DashOperator
    {
        /// <summary>
        /// Constructor for <see cref="DashOperator"/>
        /// </summary>
        /// <param name="array">The dash array</param>
        /// <param name="phase">The dash phase</param>
        public DashOperator(double[] array, double phase)
        {
            Array = array;
            Phase = phase;
        }

        /// <summary>
        /// The dash array
        /// </summary>
        public double[] Array { get; }

        /// <summary>
        /// The dash phase
        /// </summary>
        public double Phase { get; }
    }
}
