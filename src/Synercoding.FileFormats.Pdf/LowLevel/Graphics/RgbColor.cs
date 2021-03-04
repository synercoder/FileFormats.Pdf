using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    public sealed class RgbColor : Color
    {
        private const string COLOR_COMPONENT_OUT_OF_RANGE = "Color component value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).";

        private readonly double _red = 0;
        private readonly double _green = 0;
        private readonly double _blue = 0;

        public RgbColor(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public double Red
        {
            get => _red;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Red), COLOR_COMPONENT_OUT_OF_RANGE);

                _red = value;
            }
        }

        public double Green
        {
            get => _green;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Green), COLOR_COMPONENT_OUT_OF_RANGE);

                _green = value;
            }
        }

        public double Blue
        {
            get => _blue;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Blue), COLOR_COMPONENT_OUT_OF_RANGE);

                _blue = value;
            }
        }
    }
}
