using System;

namespace Synercoding.FileFormats.Pdf.Helpers
{
    internal static class MathHelper
    {
        public static double DegreeToRad(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}
