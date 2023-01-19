using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors.ColorSpaces;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors
{
    /// <summary>
    /// Class representing a RGB color
    /// </summary>
    public sealed class RgbColor : Color, IEquatable<RgbColor>
    {
        private const string COLOR_COMPONENT_OUT_OF_RANGE = "Color component value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).";

        private readonly double _red = 0;
        private readonly double _green = 0;
        private readonly double _blue = 0;

        /// <summary>
        /// Constructor for a <see cref="RgbColor"/>
        /// </summary>
        public RgbColor()
        { }

        /// <summary>
        /// Constructor for a <see cref="RgbColor"/>
        /// </summary>
        /// <param name="red">The red component of the color</param>
        /// <param name="green">The green component of the color</param>
        /// <param name="blue">The blue component of the color</param>
        public RgbColor(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// Construct a <see cref="RgbColor"/> from bytes values
        /// </summary>
        /// <param name="red">The red component of the color</param>
        /// <param name="green">The green component of the color</param>
        /// <param name="blue">The blue component of the color</param>
        public static RgbColor FromBytes(byte red, byte green, byte blue)
            => new RgbColor(red / 255d, green / 255d, blue / 255d);

        /// <summary>
        /// The red component of this color
        /// </summary>
        /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
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

        /// <summary>
        /// The green component of this color
        /// </summary>
        /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
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

        /// <summary>
        /// The blue component of this color
        /// </summary>
        /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
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

        /// <inheritdoc />
        public override double[] Components => new double[] { Red, Green, Blue };

        /// <inheritdoc />
        public override ColorSpace Colorspace
            => DeviceRGB.Instance;

        /// <inheritdoc />
        public bool Equals(RgbColor? other)
        {
            return other is not null
                && Red == other.Red
                && Green == other.Green
                && Blue == other.Blue;
        }

        /// <inheritdoc />
        public override bool Equals(Color? other)
            => Equals(other as RgbColor);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => Equals(obj as RgbColor);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(Red, Green, Blue);
    }
}
