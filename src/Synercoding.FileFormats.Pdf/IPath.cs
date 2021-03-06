using Synercoding.Primitives;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// Interface representing a path in the content stream of a page
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// Begin a new subpath by moving the current point to the coordinates (<paramref name="x"/>, <paramref name="y"/>),
        /// ommitting any connecting line segment. Appends an (m) operator to the content stream
        /// </summary>
        /// <param name="x">The X coordinate of the move</param>
        /// <param name="y">The Y coordinate of the move</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath Move(double x, double y);

        /// <summary>
        /// Begin a new subpath by moving the current point to the coordinates (<paramref name="point"/>),
        /// ommitting any connecting line segment. Appends an (m) operator to the content stream
        /// </summary>
        /// <param name="point">The point to where to move</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath Move(Point point);

        /// <summary>
        /// Add a line (l) operator to the content stream
        /// </summary>
        /// <param name="x">The X coordinate of the line end point</param>
        /// <param name="y">The Y coordinate of the line end point</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath LineTo(double x, double y);

        /// <summary>
        /// Add a line (l) operator to the content stream
        /// </summary>
        /// <param name="point">The point to where to line to</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath LineTo(Point point);

        /// <summary>
        /// Add a rectangle (re) operator to the content stream
        /// </summary>
        /// <param name="x">The X coordinate of the rectangle</param>
        /// <param name="y">The Y coordinate of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath Rectangle(double x, double y, double width, double height);

        /// <summary>
        /// Add a rectangle (re) operator to the content stream
        /// </summary>
        /// <param name="rectangle">The rectangle to add to the content stream</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath Rectangle(Rectangle rectangle);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="finalX"/>, <paramref name="finalY"/>),
        /// using (<paramref name="cpX1"/>, <paramref name="cpY1"/>) and (<paramref name="cpX2"/>, <paramref name="cpY2"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (c) operator to the content stream.
        /// </summary>
        /// <param name="cpX1">The X coordinate of the first control point</param>
        /// <param name="cpY1">The Y coordinate of the first control point</param>
        /// <param name="cpX2">The X coordinate of the second control point</param>
        /// <param name="cpY2">The Y coordinate of the second control point</param>
        /// <param name="finalX">The X coordinate of the endpoint of the curve</param>
        /// <param name="finalY">The Y coordinate of the endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="final"/>),
        /// using (<paramref name="cp1"/>) and (<paramref name="cp2"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (c) operator to the content stream.
        /// </summary>
        /// <param name="cp1">The first control point</param>
        /// <param name="cp2">The second control point</param>
        /// <param name="final">The endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveTo(Point cp1, Point cp2, Point final);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="finalX"/>, <paramref name="finalY"/>),
        /// using the current point and (<paramref name="cpX2"/>, <paramref name="cpY2"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (v) operator to the content stream.
        /// </summary>
        /// <param name="cpX2">The X coordinate of the second control point</param>
        /// <param name="cpY2">The Y coordinate of the second control point</param>
        /// <param name="finalX">The X coordinate of the endpoint of the curve</param>
        /// <param name="finalY">The Y coordinate of the endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveToWithStartAnker(double cpX2, double cpY2, double finalX, double finalY);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="final"/>),
        /// using the current point and (<paramref name="cp2"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (v) operator to the content stream.
        /// </summary>
        /// <param name="cp2">The second control point</param>
        /// <param name="final">The endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveToWithStartAnker(Point cp2, Point final);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="finalX"/>, <paramref name="finalY"/>),
        /// using (<paramref name="cpX1"/>, <paramref name="cpY1"/>) and (<paramref name="finalX"/>, <paramref name="finalY"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (y) operator to the content stream.
        /// </summary>
        /// <param name="cpX1">The X coordinate of the first control point</param>
        /// <param name="cpY1">The Y coordinate of the first control point</param>
        /// <param name="finalX">The X coordinate of the endpoint of the curve</param>
        /// <param name="finalY">The Y coordinate of the endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveToWithEndAnker(double cpX1, double cpY1, double finalX, double finalY);

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="final"/>),
        /// using (<paramref name="cp1"/>) and (<paramref name="final"/>) as the Bézier control points.
        /// Adds a Cubic Bézier Curve (y) operator to the content stream.
        /// </summary>
        /// <param name="cp1">The first control point</param>
        /// <param name="final">The endpoint of the curve</param>
        /// <returns>The calling <see cref="IPath"/> to support chaining operations.</returns>
        IPath CurveToWithEndAnker(Point cp1, Point final);

        /// <summary>
        /// Close the <see cref="IPath"/>
        /// </summary>
        void Close();
    }
}
