using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Internals;

internal class PageContentContext : IPageContentContext
{
    private readonly PageContentContext? _parent;

    public PageContentContext(ContentStream contentStream)
        : this(contentStream, null)
    { }

    public PageContentContext(ContentStream contentStream, PageContentContext? parent)
    {
        RawContentStream = contentStream;
        _parent = parent;
    }

    public ContentStream RawContentStream { get; }

    private Matrix? _ctm;
    public Matrix CTM
        => _ctm ?? _parent?.CTM ?? Matrix.Identity;

    private Color? _fill;
    public Color FillColor
        => _fill ?? _parent?.FillColor ?? PredefinedColors.Black;

    private Color? _stroke;
    public Color StrokeColor
        => _stroke ?? _parent?.StrokeColor ?? PredefinedColors.Black;

    private double? _lineWidth;
    public double LineWidth
        => _lineWidth ?? _parent?.LineWidth ?? 1.0;

    private LineCapStyle? _lineCap;
    public LineCapStyle LineCap
        => _lineCap ?? _parent?.LineCap ?? LineCapStyle.ButtCap;

    private LineJoinStyle? _lineJoin;
    public LineJoinStyle LineJoin
        => _lineJoin ?? _parent?.LineJoin ?? LineJoinStyle.MiterJoin;

    private double? _miterLimit;
    public double MiterLimit
        => _miterLimit ?? _parent?.MiterLimit ?? 10.0;

    private Dash? _dashPattern;
    public Dash DashPattern
        => _dashPattern ?? _parent?.DashPattern ?? new Dash();

    public IPageContentContext AddImage(Image image)
    {
        var name = RawContentStream.Resources.AddImage(image);

        RawContentStream.Paint(name);

        return this;
    }

    public IPageContentContext AddText<T>(T data, Action<T, ITextContentContext> textOperations)
    {
        RawContentStream.BeginText();

        var state = new TextContentContext(RawContentStream, this);
        textOperations(data, state);

        RawContentStream.EndText();

        return this;
    }

    public async Task<IPageContentContext> AddTextAsync<T>(T data, Func<T, ITextContentContext, Task> textOperations)
    {
        RawContentStream.BeginText();

        var state = new TextContentContext(RawContentStream, this);
        await textOperations(data, state);

        RawContentStream.EndText();

        return this;
    }

    public IPageContentContext ConcatenateMatrix(Matrix matrix)
    {
        _ctm = CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public IPageContentContext SetDashPattern(Dash dashPattern)
    {
        _dashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public IPageContentContext SetFill(Color fill)
    {
        _fill = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public IPageContentContext SetStroke(Color stroke)
    {
        _stroke = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public IPageContentContext SetLineCap(LineCapStyle lineCap)
    {
        _lineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public IPageContentContext SetLineJoin(LineJoinStyle lineJoin)
    {
        _lineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public IPageContentContext SetLineWidth(double lineWidth)
    {
        _lineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public IPageContentContext SetMiterLimit(double miterLimit)
    {
        _miterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public IPageContentContext WrapInState<T>(T data, Action<T, IPageContentContext> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new PageContentContext(RawContentStream, this);
        contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<IPageContentContext> WrapInStateAsync<T>(T data, Func<T, IPageContentContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new PageContentContext(RawContentStream, this);
        await contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public IPageContentContext AddShapes<T>(T data, Action<T, IShapeContext> shapeOperations)
    {
        var state = new ShapesContext(RawContentStream, this);
        shapeOperations(data, state);

        return this;
    }

    public async Task<IPageContentContext> AddShapesAsync<T>(T data, Func<T, IShapeContext, Task> shapeOperations)
    {
        var state = new ShapesContext(RawContentStream, this);
        await shapeOperations(data, state);

        return this;
    }
}
