using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    public sealed class CmykColor : Color
    {
        private const string COLOR_COMPONENT_OUT_OF_RANGE = "Color component value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).";

        private readonly double _cyan = 0;
        private readonly double _magenta = 0;
        private readonly double _yellow = 0;
        private readonly double _key = 0;

        public CmykColor(double cyan, double magenta, double yellow, double key)
        {
            Cyan = cyan;
            Magenta = magenta;
            Yellow = yellow;
            Key = key;
        }

        public double Cyan
        {
            get => _cyan;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Cyan), COLOR_COMPONENT_OUT_OF_RANGE);

                _cyan = value;
            }
        }

        public double Magenta
        {
            get => _magenta;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Magenta), COLOR_COMPONENT_OUT_OF_RANGE);

                _magenta = value;
            }
        }

        public double Yellow
        {
            get => _yellow;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Yellow), COLOR_COMPONENT_OUT_OF_RANGE);

                _yellow = value;
            }
        }

        public double Key
        {
            get => _key;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Key), COLOR_COMPONENT_OUT_OF_RANGE);

                _key = value;
            }
        }
    }
}
