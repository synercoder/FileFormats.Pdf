namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    /// <summary>
    /// Class representing a dash configuration for a stroking action
    /// </summary>
    public class Dash
    {
        /// <summary>
        /// Array representing the dash
        /// </summary>
        public double[] Array { get; set; } = System.Array.Empty<double>();

        /// <summary>
        /// The starting phase of the dash
        /// </summary>
        public double Phase { get; set; } = 0;
    }
}
