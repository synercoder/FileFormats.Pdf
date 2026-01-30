using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Text;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts;

namespace Synercoding.FileFormats.Pdf.Content.Internals;

internal class TextContentContext : ITextContentContext
{
    public TextContentContext(ContentStream contentStream, GraphicsState graphicState)
    {
        RawContentStream = contentStream;
        GraphicState = graphicState;
    }

    public ContentStream RawContentStream { get; }

    public GraphicsState GraphicState { get; }

    public ITextContentContext ConcatenateMatrix(Matrix matrix)
    {
        GraphicState.CTM = GraphicState.CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public ITextContentContext SetDashPattern(Dash dashPattern)
    {
        GraphicState.DashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public ITextContentContext SetFill(Color fill)
    {
        GraphicState.Fill = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public ITextContentContext SetStroke(Color stroke)
    {
        GraphicState.Stroke = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public ITextContentContext SetLineCap(LineCapStyle lineCap)
    {
        GraphicState.LineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public ITextContentContext SetLineJoin(LineJoinStyle lineJoin)
    {
        GraphicState.LineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public ITextContentContext SetLineWidth(double lineWidth)
    {
        GraphicState.LineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public ITextContentContext SetMiterLimit(double miterLimit)
    {
        GraphicState.MiterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public IDisposable WrapInState()
        => new StateSaver<ITextContentContext>(this);

    public ITextContentContext WrapInState<T>(T data, Action<T, ITextContentContext> contentOperations)
    {
        RawContentStream.SaveState();
        var wrappedContext = new TextContentContext(RawContentStream, GraphicState.Clone());
        contentOperations(data, wrappedContext);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<ITextContentContext> WrapInStateAsync<T>(T data, Func<T, ITextContentContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var wrappedContext = new TextContentContext(RawContentStream, GraphicState.Clone());
        await contentOperations(data, wrappedContext);
        RawContentStream.RestoreState();

        return this;
    }

    public ITextContentContext SetCharacterSpacing(double spacing)
    {
        GraphicState.CharacterSpacing = spacing;

        RawContentStream.SetCharacterSpacing(spacing);

        return this;
    }

    public ITextContentContext SetWordSpacing(double spacing)
    {
        GraphicState.WordSpacing = spacing;

        RawContentStream.SetWordSpacing(spacing);

        return this;
    }

    public ITextContentContext SetHorizontalScaling(double scaling)
    {
        GraphicState.HorizontalScaling = scaling;

        RawContentStream.SetHorizontalScaling(scaling);

        return this;
    }

    public ITextContentContext SetTextLeading(double leading)
    {
        GraphicState.TextLeading = leading;

        RawContentStream.SetTextLeading(leading);

        return this;
    }

    public ITextContentContext SetFontAndSize(Font font, double size)
    {
        var (fontName, tracker) = RawContentStream.Resources.Add(font);

        GraphicState.Font = font;
        GraphicState.FontUsageTracker = tracker;
        GraphicState.FontSize = size;
        RawContentStream.SetFontAndSize(fontName, size);

        return this;
    }

    public ITextContentContext SetTextRenderingMode(TextRenderingMode mode)
    {
        GraphicState.TextRenderingMode = mode;

        RawContentStream.SetTextRenderMode(mode);

        return this;
    }

    public ITextContentContext SetTextRise(double rise)
    {
        GraphicState.TextRise = rise;

        RawContentStream.SetTextRise(rise);

        return this;
    }

    public ITextContentContext MoveToStartNewLine()
    {
        RawContentStream.MoveNextLine();

        return this;
    }

    public ITextContentContext MoveToStartNextLine(double offsetX, double offsetY)
    {
        RawContentStream.MoveNextLineAndOffsetTextPosition(offsetX, offsetY);

        return this;
    }

    public ITextContentContext MoveToStartNextLineAndSetLeading(double offsetX, double offsetY)
    {
        RawContentStream.MoveNextLineSetLeadingAndOffsetTextPosition(offsetX, offsetY);

        return this;
    }

    public ITextContentContext ReplaceTextMatrix(Matrix matrix)
    {
        RawContentStream.Tm(matrix);

        return this;
    }

    public ITextContentContext ShowText(string text)
    {
        var encoded = GraphicState.Font?.EncodeText(text)
            ?? throw new Exceptions.PdfException($"Before calling {nameof(ShowText)}, a font must be set.");
        RawContentStream.ShowTextTj(encoded);
        GraphicState.FontUsageTracker?.AddText(text);

        return this;
    }

    public ITextContentContext ShowTextOnNextLine(string text)
    {
        var encoded = GraphicState.Font?.EncodeText(text)
            ?? throw new Exceptions.PdfException($"Before calling {nameof(ShowTextOnNextLine)}, a font must be set.");
        RawContentStream.MoveNextLineShowText(encoded);
        GraphicState.FontUsageTracker?.AddText(text);

        return this;
    }

    public ITextContentContext ShowTextOnNextLine(string text, double wordSpacing, double characterSpacing)
    {
        var encoded = GraphicState.Font?.EncodeText(text)
            ?? throw new Exceptions.PdfException($"Before calling {nameof(ShowTextOnNextLine)}, a font must be set.");
        RawContentStream.MoveNextLineShowText(encoded, wordSpacing, characterSpacing);
        GraphicState.FontUsageTracker?.AddText(text);

        return this;
    }

    public ITextContentContext SetExtendedGraphicsState(ExtendedGraphicsState extendedGraphicsState)
    {
        RawContentStream.SetExtendedGraphicsState(extendedGraphicsState);

        return this;
    }
}
