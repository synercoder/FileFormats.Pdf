using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors
{
    /// <summary>
    /// Base class of all <see cref="Color"/>s.
    /// </summary>
    public abstract class Color : IEquatable<Color>
    {
        /// <summary>
        /// The <see cref="ColorSpace"/> of this <see cref="Color"/>.
        /// </summary>
        public abstract ColorSpace Colorspace { get; }

        /// <summary>
        /// Retrieve the components that make up this color.
        /// </summary>
        public abstract double[] Components { get; }

        /// <inheritdoc />
        public abstract bool Equals(Color? other);

        /// <inheritdoc />
        public abstract override bool Equals(object? obj);

        /// <inheritdoc />
        public abstract override int GetHashCode();

        /// <summary>
        /// Indicates whether the left <see cref="Color"/> is equal to the right <see cref="Color"/>.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left side of the ==</param>
        /// <param name="right">The <see cref="Color"/> on the right side of the ==</param>
        /// <returns>true if the left Color is equal to the right; otherwise, false.</returns>
        public static bool operator ==(Color left, Color right)
            => left.Equals(right);

        /// <summary>
        /// Indicates whether the left <see cref="Color"/> is not equal to the right <see cref="Color"/>.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left side of the !=</param>
        /// <param name="right">The <see cref="Color"/> on the right side of the !=</param>
        /// <returns>true if the left Color is not equal to the right; otherwise, false.</returns>
        public static bool operator !=(Color left, Color right)
            => !( left == right );
    }
}
