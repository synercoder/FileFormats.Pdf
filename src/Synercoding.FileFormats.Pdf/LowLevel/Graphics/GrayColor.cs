using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    public sealed class GrayColor : Color
    {
        public GrayColor(double gray)
        {
            Gray = gray;
        }

        private readonly double _gray = 0;

        public double Gray
        {
            get => _gray;
            init
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(Gray), "Gray value must be between 0.0 (black) and 1.0 (white).");

                _gray = value;
            }
        }
    }
}
