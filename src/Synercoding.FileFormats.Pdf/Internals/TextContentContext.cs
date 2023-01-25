using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Internals;

internal class TextContentContext : ITextContentContext
{
    private readonly IPageContentContext _parent;

    public TextContentContext(ContentStream contentStream, IPageContentContext parent)
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

    private double? _characterSpacing;
    public double CharacterSpacing
        => _characterSpacing ?? 0.0;

    private double? _wordSpacing;
    public double WordSpacing
        => _wordSpacing ?? 0.0;

    private double? _horizontalScale;
    public double HorizontalScaling
        => _horizontalScale ?? 100.0;

    private double? _textLeading;
    public double TextLeading
        => _textLeading ?? 0.0;

    public Font? Font { get; private set; }

    public double? FontSize { get; private set; }

    private TextRenderingMode? _textRenderingMode;
    public TextRenderingMode TextRenderingMode
        => _textRenderingMode ?? TextRenderingMode.Fill;

    private double? _textRise;
    public double TextRise
        => _textRise ?? 0.0;

    public ITextContentContext ConcatenateMatrix(Matrix matrix)
    {
        _ctm = CTM.Multiply(matrix);
        RawContentStream.CTM(matrix);

        return this;
    }

    public ITextContentContext SetDashPattern(Dash dashPattern)
    {
        _dashPattern = dashPattern;

        RawContentStream.SetDashPattern(dashPattern);

        return this;
    }

    public ITextContentContext SetFill(Color fill)
    {
        _fill = fill;

        RawContentStream.SetFillColor(fill);

        return this;
    }

    public ITextContentContext SetStroke(Color stroke)
    {
        _stroke = stroke;

        RawContentStream.SetStrokeColor(stroke);

        return this;
    }

    public ITextContentContext SetLineCap(LineCapStyle lineCap)
    {
        _lineCap = lineCap;

        RawContentStream.SetLineCap(lineCap);

        return this;
    }

    public ITextContentContext SetLineJoin(LineJoinStyle lineJoin)
    {
        _lineJoin = lineJoin;

        RawContentStream.SetLineJoin(lineJoin);

        return this;
    }

    public ITextContentContext SetLineWidth(double lineWidth)
    {
        _lineWidth = lineWidth;

        RawContentStream.SetLineWidth(lineWidth);

        return this;
    }

    public ITextContentContext SetMiterLimit(double miterLimit)
    {
        _miterLimit = miterLimit;

        RawContentStream.SetMiterLimit(miterLimit);

        return this;
    }

    public ITextContentContext WrapInState<T>(T data, Action<T, ITextContentContext> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new WrappedTextContentContext(RawContentStream, this);
        contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public async Task<ITextContentContext> WrapInStateAsync<T>(T data, Func<T, ITextContentContext, Task> contentOperations)
    {
        RawContentStream.SaveState();
        var state = new WrappedTextContentContext(RawContentStream, this);
        await contentOperations(data, state);
        RawContentStream.RestoreState();

        return this;
    }

    public ITextContentContext SetCharacterSpacing(double spacing)
    {
        _characterSpacing = spacing;

        RawContentStream.SetCharacterSpacing(spacing);

        return this;
    }

    public ITextContentContext SetWordSpacing(double spacing)
    {
        _wordSpacing = spacing;

        RawContentStream.SetWordSpacing(spacing);

        return this;
    }

    public ITextContentContext SetHorizontalScaling(double scaling)
    {
        _horizontalScale = scaling;

        RawContentStream.SetHorizontalScaling(scaling);

        return this;
    }

    public ITextContentContext SetTextLeading(double leading)
    {
        _textLeading = leading;

        RawContentStream.SetTextLeading(leading);

        return this;
    }

    public ITextContentContext SetFontAndSize(Font font, double size)
    {
        var fontName = font switch
        {
            Type1StandardFont stdFont => RawContentStream.Resources.AddStandardFont(stdFont),
            _ => throw new NotImplementedException($"Font of type {font?.GetType()} is currently not implemented.")
        };

        Font = font;
        FontSize = size;
        RawContentStream.SetFontAndSize(fontName, size);

        return this;
    }

    public ITextContentContext SetTextRenderingMode(TextRenderingMode mode)
    {
        _textRenderingMode = mode;

        RawContentStream.SetTextRenderMode(mode);

        return this;
    }

    public ITextContentContext SetTextRise(double rise)
    {
        _textRise = rise;

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
        RawContentStream.ShowTextTj(text);

        return this;
    }

    public ITextContentContext ShowTextOnNextLine(string text)
    {
        RawContentStream.MoveNextLineShowText(text);

        return this;
    }

    public ITextContentContext ShowTextOnNextLine(string text, double wordSpacing, double characterSpacing)
    {
        RawContentStream.MoveNextLineShowText(text, wordSpacing, characterSpacing);

        return this;
    }
}
