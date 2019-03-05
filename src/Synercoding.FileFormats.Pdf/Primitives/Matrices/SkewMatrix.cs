using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Matrix for a skew operation
    /// </summary>
    public class SkewMatrix : Matrix
    {
        /// <summary>
        /// Constructor for <see cref="SkewMatrix"/>
        /// </summary>
        /// <param name="a">The amount of degree to skew the top left direction</param>
        /// <param name="b">The amount of degree to skew the bottom right direction</param>
        public SkewMatrix(double a, double b)
            : base(1, Math.Tan(MathHelper.DegreeToRad(a)), Math.Tan(MathHelper.DegreeToRad(b)), 1, 0, 0)
        { }
    }
}
