using Synercoding.FileFormats.Pdf.Helpers;
using System;

namespace Synercoding.FileFormats.Pdf.Primitives.Matrices
{
    public class RotateMatrix : Matrix
    {
        public RotateMatrix(double degree)
            : base(Math.Cos(MathHelper.DegreeToRad(degree)), Math.Sin(MathHelper.DegreeToRad(degree)), Math.Sin(MathHelper.DegreeToRad(degree)) * -1, Math.Cos(MathHelper.DegreeToRad(degree)), 0, 0)
        { }
    }
}