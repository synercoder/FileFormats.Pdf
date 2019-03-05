using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    public class SkewMatrix : Matrix
    {
        public SkewMatrix(double a, double b)
            : base(1, Math.Tan(MathHelper.DegreeToRad(a)), Math.Tan(MathHelper.DegreeToRad(b)), 1, 0, 0)
        { }
    }
}
