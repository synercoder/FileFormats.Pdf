using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    /// <summary>
    /// Matrix for a rotation operation
    /// </summary>
    public class RotateMatrix : Matrix
    {
        /// <summary>
        /// Constructor for <see cref="RotateMatrix"/>
        /// </summary>
        /// <param name="degree">The amount of degrees to rotate by</param>
        public RotateMatrix(double degree)
            : base(Math.Cos(MathHelper.DegreeToRad(degree)), Math.Sin(MathHelper.DegreeToRad(degree)), Math.Sin(MathHelper.DegreeToRad(degree)) * -1, Math.Cos(MathHelper.DegreeToRad(degree)), 0, 0)
        { }
    }
}