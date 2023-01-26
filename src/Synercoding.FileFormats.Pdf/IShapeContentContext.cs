using Synercoding.FileFormats.Pdf.LowLevel;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Context to enable shape operations on the content
/// </summary>
public interface IShapeContentContext : IContentContext<IShapeContentContext>
{
    /// <summary>
    /// Begin a new subpath by moving the current point to the coordinates (<paramref name="x"/>, <paramref name="y"/>),
    /// omitting any connecting line segment. Appends an (m) operator to the content stream
    /// </summary>
    /// <param name="x">The X coordinate of the move</param>
    /// <param name="y">The Y coordinate of the move</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext Move(double x, double y);


    /// <summary>
    /// Add a line (l) operator to the content stream
    /// </summary>
    /// <param name="x">The X coordinate of the line end point</param>
    /// <param name="y">The Y coordinate of the line end point</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext LineTo(double x, double y);

    /// <summary>
    /// Add a rectangle (re) operator to the content stream
    /// </summary>
    /// <param name="x">The X coordinate of the rectangle</param>
    /// <param name="y">The Y coordinate of the rectangle</param>
    /// <param name="width">The width of the rectangle</param>
    /// <param name="height">The height of the rectangle</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext Rectangle(double x, double y, double width, double height);

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
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY);

    /// <summary>
    /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="finalX"/>, <paramref name="finalY"/>),
    /// using the current point and (<paramref name="cpX2"/>, <paramref name="cpY2"/>) as the Bézier control points.
    /// Adds a Cubic Bézier Curve (v) operator to the content stream.
    /// </summary>
    /// <param name="cpX2">The X coordinate of the second control point</param>
    /// <param name="cpY2">The Y coordinate of the second control point</param>
    /// <param name="finalX">The X coordinate of the endpoint of the curve</param>
    /// <param name="finalY">The Y coordinate of the endpoint of the curve</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CurveToWithStartAnker(double cpX2, double cpY2, double finalX, double finalY);

    /// <summary>
    /// Append a cubic Bézier curve to the current path. The curve shall extend from the current point to the point (<paramref name="finalX"/>, <paramref name="finalY"/>),
    /// using (<paramref name="cpX1"/>, <paramref name="cpY1"/>) and (<paramref name="finalX"/>, <paramref name="finalY"/>) as the Bézier control points.
    /// Adds a Cubic Bézier Curve (y) operator to the content stream.
    /// </summary>
    /// <param name="cpX1">The X coordinate of the first control point</param>
    /// <param name="cpY1">The Y coordinate of the first control point</param>
    /// <param name="finalX">The X coordinate of the endpoint of the curve</param>
    /// <param name="finalY">The Y coordinate of the endpoint of the curve</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CurveToWithEndAnker(double cpX1, double cpY1, double finalX, double finalY);

    /// <summary>
    /// Close the active subpath in this <see cref="IShapeContentContext"/>
    /// </summary>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CloseSubPath();

    /// <summary>
    /// Mark the current path to be used as a clipping mask.
    /// </summary>
    /// <param name="fillRule">The <see cref="FillRule"/> to use</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext MarkPathForClipping(FillRule fillRule);

    /// <summary>
    /// Stroke the current path
    /// </summary>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext Stroke();

    /// <summary>
    /// Close and stroke the path. 
    /// </summary>
    /// <remarks>This operator has the same effect as the sequence <see cref="CloseSubPath"/>() and then <see cref="Stroke"/>().</remarks>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CloseSubPathAndStroke();

    /// <summary>
    /// Fill the current path using <paramref name="fillRule"/> as the fill rule.
    /// </summary>
    /// <param name="fillRule">The <see cref="FillRule"/> to use</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext Fill(FillRule fillRule);

    /// <summary>
    /// Fill the current path using <paramref name="fillRule"/> as the fill rule, and then stroke the path.
    /// </summary>
    /// <param name="fillRule">The <see cref="FillRule"/> to use</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext FillThenStroke(FillRule fillRule);

    /// <summary>
    /// Close the current path, then fill the current path using <paramref name="fillRule"/> as the fill rule, and then stroke the path.
    /// </summary>
    /// <param name="fillRule">The <see cref="FillRule"/> to use</param>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext CloseSubPathFillStroke(FillRule fillRule);

    /// <summary>
    /// End the path object without filling or stroking it. This operator is a path-painting no-op, used primarily for the side effect of changing the current clipping path.
    /// </summary>
    /// <returns>The calling <see cref="IShapeContentContext"/> to support chaining operations.</returns>
    IShapeContentContext EndPathNoStrokeNoFill();
}
