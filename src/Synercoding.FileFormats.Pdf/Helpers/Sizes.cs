using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Helpers
{
    public static class Sizes
    {
        public static Rectangle A4Portrait { get; } = new Rectangle(0, 0, _mmToPts(210), _mmToPts(297));
        public static Rectangle A4Landscape { get; } = new Rectangle(0, 0, _mmToPts(297), _mmToPts(210));

        private static double _mmToPts(double mm) => mm / 25.4 * 72;
    }
}