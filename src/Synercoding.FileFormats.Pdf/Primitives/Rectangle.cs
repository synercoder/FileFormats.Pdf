using System;

namespace Synercoding.FileFormats.Pdf.Primitives
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static Rectangle Zero { get; } = new Rectangle(0, 0, 0, 0);

        public Rectangle(double llx, double lly, double urx, double ury)
        {
            LLX = llx;
            LLY = lly;
            URX = urx;
            URY = ury;
        }

        public double LLX { get; }
        public double LLY { get; }
        public double URX { get; }
        public double URY { get; }

        public double Width { get => URX - LLX; }
        public double Height { get => URY - LLY; }
        
        public bool Equals(Rectangle other)
        {
            return Equals(LLX, other.LLX)
                && Equals(LLY, other.LLY)
                && Equals(URX, other.URX)
                && Equals(URY, other.URY);
        }

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