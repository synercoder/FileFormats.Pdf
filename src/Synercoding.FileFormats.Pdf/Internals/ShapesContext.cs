using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Internals;

internal class ShapesContext : IShapeContext
{
    private readonly PageContentContext _parent;

    public ShapesContext(ContentStream contentStream, PageContentContext parent)
    {
        RawContentStream = contentStream;
        _parent = parent;
    }

    public ContentStream RawContentStream { get; }

    private Matrix? _ctm;
    public Matrix CTM
        => _ctm ?? _parent.CTM;

    private Color? _fill;
    public Color FillColor
        => _fill ?? _parent.FillColor;

    private Color? _stroke;
    public Color StrokeColor
        => _stroke ?? _parent.StrokeColor;

    private double? _lineWidth;
    public double LineWidth
        => _lineWidth ?? _parent.LineWidth;

    private LineCapStyle? _lineCap;
    public LineCapStyle LineCap
        => _lineCap ?? _parent.LineCap;

    private LineJoinStyle? _lineJoin;
    public LineJoinStyle LineJoin
        => _lineJoin ?? _parent.LineJoin;

    private double? _miterLimit;
    public double MiterLimit
        => _miterLimit ?? _parent.MiterLimit;

    private Dash? _dashPattern;
    public Dash DashPattern
        => _dashPattern ?? _parent.DashPattern;

    public IShapeContext ConcatenateMatrix(Matrix matrix)
    {
        _ctm = CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public IShapeContext SetDashPattern(Dash dashPattern)
    {
        _dashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public IShapeContext SetFill(Color fill)
    {
        _fill = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public IShapeContext SetStroke(Color stroke)
    {
        _stroke = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public IShapeContext SetLineCap(LineCapStyle lineCap)
    {
        _lineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public IShapeContext SetLineJoin(LineJoinStyle lineJoin)
    {
        _lineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public IShapeContext SetLineWidth(double lineWidth)
    {
        _lineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public IShapeContext SetMiterLimit(double miterLimit)
    {
        _miterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public IShapeContext WrapInState<T>(T data, Action<T, IShapeContext> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new WrappedShapesContext(RawContentStream, this);
        contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<IShapeContext> WrapInStateAsync<T>(T data, Func<T, IShapeContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new WrappedShapesContext(RawContentStream, this);
        await contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public IShapeContext Move(double x, double y)
    {
        RawContentStream.MoveTo(x, y);

        return this;
    }

    public IShapeContext LineTo(double x, double y)
    {
        RawContentStream.LineTo(x, y);

        return this;
    }

    public IShapeContext Rectangle(double x, double y, double width, double height)
    {
        RawContentStream.Rectangle(x, y, width, height);

        return this;
    }

    public IShapeContext CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurve(cpX1, cpY1, cpX2, cpY2, finalX, finalY);

        return this;
    }

    public IShapeContext CurveToWithStartAnker(double cpX2, double cpY2, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurveV(cpX2, cpY2, finalX, finalY);

        return this;
    }

    public IShapeContext CurveToWithEndAnker(double cpX1, double cpY1, double finalX, double finalY)
    {
        RawContentStream.CubicBezierCurveY(cpX1, cpY1, finalX, finalY);

        return this;
    }

    public IShapeContext CloseSubPath()
    {
        RawContentStream.Close();

        return this;
    }

    public IShapeContext MarkPathForClipping(FillRule fillRule)
    {
        RawContentStream.Clip(fillRule);

        return this;
    }

    public IShapeContext Stroke()
    {
        RawContentStream.Stroke();

        return this;
    }

    public IShapeContext CloseSubPathAndStroke()
    {
        RawContentStream.CloseAndStroke();

        return this;
    }

    public IShapeContext Fill(FillRule fillRule)
    {
        RawContentStream.Fill(fillRule);

        return this;
    }

    public IShapeContext FillThenStroke(FillRule fillRule)
    {
        RawContentStream.FillAndStroke(fillRule);

        return this;
    }

    public IShapeContext CloseSubPathFillStroke(FillRule fillRule)
    {
        RawContentStream.CloseFillAndStroke(fillRule);

        return this;
    }

    public IShapeContext EndPathNoStrokeNoFill()
    {
        RawContentStream.EndPath();

        return this;
    }
}
