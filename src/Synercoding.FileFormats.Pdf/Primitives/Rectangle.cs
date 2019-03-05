using System;

namespace Synercoding.FileFormats.Pdf.Primitives
{
    /// <summary>
    /// Struct that represents a Rectangle
    /// </summary>
    public struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// Rectangle representing nothing
        /// </summary>
        public static Rectangle Zero { get; } = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Constructor for <see cref="Rectangle"/>
        /// </summary>
        /// <param name="llx">The lower left X</param>
        /// <param name="lly">The lower left Y</param>
        /// <param name="urx">The upper right X</param>
        /// <param name="ury">The upper right Y</param>
        public Rectangle(double llx, double lly, double urx, double ury)
        {
            LLX = llx;
            LLY = lly;
            URX = urx;
            URY = ury;
        }

        /// <summary>
        /// The lower left X
        /// </summary>
        public double LLX { get; }

        /// <summary>
        /// The lower left Y
        /// </summary>
        public double LLY { get; }

        /// <summary>
        /// The upper right X
        /// </summary>
        public double URX { get; }

        /// <summary>
        /// The upper right Y
        /// </summary>
        public double URY { get; }

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public double Width { get => URX - LLX; }

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public double Height { get => URY - LLY; }

        /// <summary>
        /// Check equality with another <see cref="Rectangle"/>
        /// </summary>
        /// <param name="other">The other rectangle to check equality against</param>
        /// <returns>True if the rectangles are the same, otherwise false</returns>
        public bool Equals(Rectangle other)
        {
            return Equals(LLX, other.LLX)
                && Equals(LLY, other.LLY)
                && Equals(URX, other.URX)
                && Equals(URY, other.URY);
        }

        /// <summary>
        /// Check equality with another <see cref="object"/>
        /// </summary>
        /// <param name="other">The other object to check against</param>
        /// <returns>True if the provided object is a rectangle and they are the same, otherwise false</returns>
        public override bool Equals(object other)
        {
            switch(other)
            {
                case Rectangle otherRect:
                    return Equals(otherRect);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the hashcode for this <see cref="Rectangle"/>
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            // Based upon the algorithm stated in https://stackoverflow.com/a/263416/637425 by Jon Skeet
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = (hash * 23) + LLX.GetHashCode();
                hash = (hash * 23) + LLY.GetHashCode();
                hash = (hash * 23) + URX.GetHashCode();
                hash = (hash * 23) + URY.GetHashCode();
                return hash;
            }
        }
    }
}