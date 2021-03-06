using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors
{
    /// <summary>
    /// Base class of all <see cref="Color"/>s.
    /// </summary>
    public abstract class Color : IEquatable<Color>
    {
        /// <inheritdoc />
        public abstract bool Equals(Color? other);

        /// <inheritdoc />
        public abstract override bool Equals(object? obj);

        /// <inheritdoc />
        public abstract override int GetHashCode();

        /// <summary>
        /// Indicates whether the left Color is equal to the right Color.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left side of the ==</param>
        /// <param name="right">The <see cref="Color"/> on the right side of the ==</param>
        /// <returns>true if the left Color is equal to the right; otherwise, false.</returns>
        public static bool operator ==(Color left, Color right)
            => left.Equals(right);

        /// <summary>
        /// Indicates whether the left Color is not equal to the right Color.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left side of the !=</param>
        /// <param name="right">The <see cref="Color"/> on the right side of the !=</param>
        /// <returns>true if the left Color is not equal to the right; otherwise, false.</returns>
        public static bool operator !=(Color left, Color right)
            => !( left == right );
    }
}
