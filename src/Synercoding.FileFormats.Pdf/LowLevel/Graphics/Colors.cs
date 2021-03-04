namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    public static class Colors
    {
        public static Color Cyan { get; } = new CmykColor(1, 0, 0, 0);
        public static Color Magenta { get; } = new CmykColor(0, 1, 0, 0);
        public static Color Yellow { get; } = new CmykColor(0, 0, 1, 0);
        public static Color Black { get; } = new GrayColor(0);
        public static Color White { get; } = new GrayColor(1);
        public static Color Red { get; } = new RgbColor(1, 0, 0);
        public static Color Green { get; } = new RgbColor(0, 1, 0);
        public static Color Blue { get; } = new RgbColor(0, 0, 1);
    }
}
