using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors
{
    /// <summary>
    /// Class representing a gray color
    /// </summary>
    public sealed class GrayColor : Color, IEquatable<GrayColor>
    {
        /// <summary>
        /// Construct a new <see cref="GrayColor"/> object
        /// </summary>
        /// <param name="gray"></param>
        public GrayColor(double gray)
        {
            Gray = gray;
        }

        private readonly double _gray = 0;

        /// <summary>
        /// The single component that represents the gray color. 
        /// </summary>
        /// <remarks>
        /// 0.0 = Black
        /// 1.0 = white
        /// </remarks>
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

        /// <inheritdoc />
        public bool Equals(GrayColor? other)
        {
            return other is not null
                && Gray == other.Gray;
        }

        /// <inheritdoc />
        public override bool Equals(Color? other)
            => other is GrayColor color && Equals(color);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is GrayColor color && Equals(color);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(Gray);
    }
}
