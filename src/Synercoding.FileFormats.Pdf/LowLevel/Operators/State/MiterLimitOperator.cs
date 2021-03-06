namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.State
{
    /// <summary>
    /// Class representing an miter limit operator for a content stream
    /// </summary>
    public struct MiterLimitOperator
    {
        /// <summary>
        /// Constructor for <see cref="MiterLimitOperator"/>
        /// </summary>
        /// <param name="limit">The miter limit</param>
        public MiterLimitOperator(double limit)
        {
            Limit = limit;
        }

        /// <summary>
        /// The limit that this operator represents
        /// </summary>
        public double Limit { get; }
    }
}
