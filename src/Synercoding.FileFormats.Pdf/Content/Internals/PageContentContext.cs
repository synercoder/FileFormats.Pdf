using Synercoding.FileFormats.Pdf.Content.Colors;

namespace Synercoding.FileFormats.Pdf.Content.Internals;

internal class PageContentContext : IPageContentContext
{
    public PageContentContext(ContentStream contentStream, GraphicsState graphicState)
    {
        RawContentStream = contentStream;
        GraphicState = graphicState;
    }

    public ContentStream RawContentStream { get; }

    public GraphicsState GraphicState { get; }

    public IPageContentContext AddImage(PdfImage image)
    {
        var name = RawContentStream.Resources.Add(image);

        RawContentStream.Paint(name);

        return this;
    }

    public IPageContentContext AddText(Action<ITextContentContext> textOperations)
        => AddText(textOperations, (o, c) => o(c));

    public IPageContentContext AddText<T>(T data, Action<T, ITextContentContext> textOperations)
    {
        RawContentStream.BeginText();

        var state = new TextContentContext(RawContentStream, GraphicState);
        textOperations(data, state);

        RawContentStream.EndText();

        return this;
    }

    public Task<IPageContentContext> AddTextAsync(Func<ITextContentContext, Task> textOperations)
        => AddTextAsync(textOperations, (o, c) => o(c));

    public async Task<IPageContentContext> AddTextAsync<T>(T data, Func<T, ITextContentContext, Task> textOperations)
    {
        RawContentStream.BeginText();

        var state = new TextContentContext(RawContentStream, GraphicState);
        await textOperations(data, state);

        RawContentStream.EndText();

        return this;
    }

    public IPageContentContext AddShapes(Action<IShapeContentContext> shapeOperations)
        => AddShapes(shapeOperations, (o, c) => o(c));

    public IPageContentContext AddShapes<T>(T data, Action<T, IShapeContentContext> shapeOperations)
    {
        var state = new ShapesContentContext(RawContentStream, GraphicState);
        shapeOperations(data, state);

        return this;
    }

    public Task<IPageContentContext> AddShapesAsync(Func<IShapeContentContext, Task> shapeOperations)
        => AddShapesAsync(shapeOperations, (o, c) => o(c));

    public async Task<IPageContentContext> AddShapesAsync<T>(T data, Func<T, IShapeContentContext, Task> shapeOperations)
    {
        var state = new ShapesContentContext(RawContentStream, GraphicState);
        await shapeOperations(data, state);

        return this;
    }

    public IPageContentContext ConcatenateMatrix(Matrix matrix)
    {
        GraphicState.CTM = GraphicState.CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public IPageContentContext SetDashPattern(Dash dashPattern)
    {
        GraphicState.DashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public IPageContentContext SetFill(Color fill)
    {
        GraphicState.Fill = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public IPageContentContext SetStroke(Color stroke)
    {
        GraphicState.Stroke = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public IPageContentContext SetLineCap(LineCapStyle lineCap)
    {
        GraphicState.LineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public IPageContentContext SetLineJoin(LineJoinStyle lineJoin)
    {
        GraphicState.LineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public IPageContentContext SetLineWidth(double lineWidth)
    {
        GraphicState.LineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public IPageContentContext SetMiterLimit(double miterLimit)
    {
        GraphicState.MiterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public IDisposable WrapInState()
        => new StateSaver<IPageContentContext>(this);

    public IPageContentContext WrapInState<T>(T data, Action<T, IPageContentContext> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new PageContentContext(RawContentStream, GraphicState.Clone());
        contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<IPageContentContext> WrapInStateAsync<T>(T data, Func<T, IPageContentContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new PageContentContext(RawContentStream, GraphicState.Clone());
        await contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public IPageContentContext SetExtendedGraphicsState(ExtendedGraphicsState extendedGraphicsState)
    {
        RawContentStream.SetExtendedGraphicsState(extendedGraphicsState);

        return this;
    }
}
