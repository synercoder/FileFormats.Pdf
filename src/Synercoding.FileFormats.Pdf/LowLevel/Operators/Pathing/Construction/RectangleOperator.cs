using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction
{
    /// <summary>
    /// Struct representing a rectangle operator (re)
    /// </summary>
    public struct RectangleOperator
    {
        /// <summary>
        /// Constructor for a <see cref="RectangleOperator"/>
        /// </summary>
        /// <param name="x">The X coordinate of the lower left corner of the rectangle</param>
        /// <param name="y">The Y coordinate of the lower left corner of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public RectangleOperator(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructor for a <see cref="RectangleOperator"/>
        /// </summary>
        /// <param name="rectangle"></param>
        public RectangleOperator(Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);

            X = rectangle.LLX.Raw;
            Y = rectangle.LLY.Raw;
            Width = rectangle.Width.Raw;
            Height = rectangle.Height.Raw;
        }

        /// <summary>
        /// The X coordinate of the lower left corner of the rectangle
        /// </summary>
        public double X { get; }

        /// <summary>
        /// The Y coordinate of the lower left corner of the rectangle
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public double Width { get; }

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public double Height { get; }
    }
}
