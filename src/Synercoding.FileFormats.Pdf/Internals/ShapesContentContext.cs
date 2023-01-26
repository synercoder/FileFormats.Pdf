using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Internals;

internal class ShapesContentContext : IShapeContentContext
{
    public ShapesContentContext(ContentStream contentStream, GraphicState graphicState)
    {
        RawContentStream = contentStream;
        GraphicState = graphicState;
    }

    public ContentStream RawContentStream { get; }

    public GraphicState GraphicState { get; }

    public IShapeContentContext ConcatenateMatrix(Matrix matrix)
    {
        GraphicState.CTM = GraphicState.CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public IShapeContentContext SetDashPattern(Dash dashPattern)
    {
        GraphicState.DashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public IShapeContentContext SetFill(Color fill)
    {
        GraphicState.FillColor = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public IShapeContentContext SetStroke(Color stroke)
    {
        GraphicState.StrokeColor = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public IShapeContentContext SetLineCap(LineCapStyle lineCap)
    {
        GraphicState.LineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public IShapeContentContext SetLineJoin(LineJoinStyle lineJoin)
    {
        GraphicState.LineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public IShapeContentContext SetLineWidth(double lineWidth)
    {
        GraphicState.LineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public IShapeContentContext SetMiterLimit(double miterLimit)
    {
        GraphicState.MiterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public IShapeContentContext WrapInState<T>(T data, Action<T, IShapeContentContext> contentOperations)
    {
        RawContentStream.SaveState();
        var wrappedContext = new ShapesContentContext(RawContentStream, GraphicState.Clone());
        contentOperations(data, wrappedContext);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<IShapeContentContext> WrapInStateAsync<T>(T data, Func<T, IShapeContentContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var wrappedContext = new ShapesContentContext(RawContentStream, GraphicState.Clone());
        await contentOperations(data, wrappedContext);
        RawContentStream.RestoreState();

        return this;
    }

    public IShapeContentContext Move(double x, double y)
    {
        RawContentStream.MoveTo(x, y);

        return this;
    }

    public IShapeContentContext LineTo(double x, double y)
    {
        RawContentStream.LineTo(x, y);

        return this;
    }

    public IShapeContentContext Rectangle(double x, double y, double width, double height)
    {
        RawContentStream.Rectangle(x, y, width, height);

        return this;
    }

    public IShapeContentContext CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurve(cpX1, cpY1, cpX2, cpY2, finalX, finalY);

        return this;
    }

    public IShapeContentContext CurveToWithStartAnker(double cpX2, double cpY2, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurveV(cpX2, cpY2, finalX, finalY);

        return this;
    }

    public IShapeContentContext CurveToWithEndAnker(double cpX1, double cpY1, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurveY(cpX1, cpY1, finalX, finalY);

        return this;
    }

    public IShapeContentContext CloseSubPath()
    {
        RawContentStream.Close();

        return this;
    }

    public IShapeContentContext MarkPathForClipping(FillRule fillRule)
    {
        RawContentStream.Clip(fillRule);

        return this;
    }

    public IShapeContentContext Stroke()
    {
        RawContentStream.Stroke();

        return this;
    }

    public IShapeContentContext CloseSubPathAndStroke()
    {
        RawContentStream.CloseAndStroke();

        return this;
    }

    public IShapeContentContext Fill(FillRule fillRule)
    {
        RawContentStream.Fill(fillRule);

        return this;
    }

    public IShapeContentContext FillThenStroke(FillRule fillRule)
    {
        RawContentStream.FillAndStroke(fillRule);

        return this;
    }

    public IShapeContentContext CloseSubPathFillStroke(FillRule fillRule)
    {
        RawContentStream.CloseFillAndStroke(fillRule);

        return this;
    }

    public IShapeContentContext EndPathNoStrokeNoFill()
    {
        RawContentStream.EndPath();

        return this;
    }
}
