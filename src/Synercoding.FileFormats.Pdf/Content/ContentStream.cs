using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Text;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Class to represent a content stream
/// </summary>
public sealed class ContentStream : IDisposable
{
    private const string UNKNOWN_FILL_RULE = "Unknown fill rule";
    private readonly DirectObjectSerializer _serializer;

    internal ContentStream(PdfObjectId id, PageResources pageResources)
    {
        Resources = pageResources;
        InnerStream = new PdfStream(new MemoryStream());
        _serializer = new DirectObjectSerializer(InnerStream);

        Id = id;
    }

    internal PdfStream InnerStream { get; private set; }

    internal PageResources Resources { get; }

    /// <summary>
    /// A pdf id object that can be used to reference to this object
    /// </summary>
    public PdfObjectId Id { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        InnerStream.Dispose();
    }

    /// <summary>
    /// Write a save state operator (q) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SaveState()
    {
        InnerStream.Write('q').NewLine();

        return this;
    }

    /// <summary>
    /// Write a restore state operator (Q) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream RestoreState()
    {
        InnerStream.Write('Q').NewLine();

        return this;
    }

    /// <summary>
    /// Write a transformation matrix operator (cm) to the stream
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix"/> to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CTM(Matrix matrix)
    {
        InnerStream
            .Write(matrix.A)
            .Space()
            .Write(matrix.B)
            .Space()
            .Write(matrix.C)
            .Space()
            .Write(matrix.D)
            .Space()
            .Write(matrix.E)
            .Space()
            .Write(matrix.F)
            .Space()
            .Write("cm")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the BT operator to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream BeginText()
    {
        InnerStream
            .Write("BT")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the font used to the content stream
    /// </summary>
    /// <param name="font">The font to write to the stream</param>
    /// <param name="size">The font size to use</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetFontAndSize(PdfName font, double size)
    {
        _serializer.WriteDirect(font);
        InnerStream
            .Space()
            .Write(size)
            .Space()
            .Write("Tf")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the text leading to the content stream
    /// </summary>
    /// <param name="leading">The leading value to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetTextLeading(double leading)
    {
        InnerStream
            .Write(leading)
            .Space()
            .Write("TL")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the character spacing to the content stream
    /// </summary>
    /// <param name="characterSpace">The character spacing value.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetCharacterSpacing(double characterSpace)
    {
        InnerStream
            .Write(characterSpace)
            .Space()
            .Write("Tc")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the horizontal scaling to the content stream
    /// </summary>
    /// <param name="horizontalScaling">The horizontal scaling value.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetHorizontalScaling(double horizontalScaling)
    {
        InnerStream
            .Write(horizontalScaling)
            .Space()
            .Write("Tz")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the text rise to the content stream
    /// </summary>
    /// <param name="textRise">The text rise value.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetTextRise(double textRise)
    {
        InnerStream
            .Write(textRise)
            .Space()
            .Write("Ts")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the text rendering mode to the content stream
    /// </summary>
    /// <param name="textRenderingMode">The text rendering mode value.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetTextRenderMode(TextRenderingMode textRenderingMode)
    {
        InnerStream
            .Write((int)textRenderingMode)
            .Space()
            .Write("Tr")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the word spacing to the content stream
    /// </summary>
    /// <param name="wordSpace">The word space value.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetWordSpacing(double wordSpace)
    {
        InnerStream
            .Write(wordSpace)
            .Space()
            .Write("Tw")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the end text operator to the content stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream EndText()
    {
        InnerStream
            .Write("ET")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write text to the content stream using the Tj operator
    /// </summary>
    /// <param name="line">The encoded text to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream ShowTextTj(byte[] line)
    {
        InnerStream
            .WriteStringLiteral(line)
            .Space()
            .Write("Tj")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write text to the content stream using the ' operator
    /// </summary>
    /// <param name="line">The encoded text to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineShowText(byte[] line)
    {
        InnerStream
            .WriteStringLiteral(line)
            .Space()
            .Write("'")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Move to the next line using the T* operator
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLine()
    {
        InnerStream
            .Write("T*")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write text to the content stream and use certain character and word spacing using the " operator.
    /// </summary>
    /// <param name="line">The encoded text to write</param>
    /// <param name="wordSpacing">The word spacing to set</param>
    /// <param name="characterSpacing">The character spacing to set</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineShowText(byte[] line, double wordSpacing, double characterSpacing)
    {
        InnerStream
            .Write(wordSpacing)
            .Space()
            .Write(characterSpacing)
            .Space()
            .WriteStringLiteral(line)
            .Space()
            .Write("\"")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the <paramref name="offset"/> to the content stream with a Td operator
    /// </summary>
    /// <param name="offset">The offset to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineAndOffsetTextPosition(Point offset)
        => MoveNextLineAndOffsetTextPosition(offset.X.AsRaw(Unit.Points), offset.Y.AsRaw(Unit.Points));

    /// <summary>
    /// Write the <paramref name="x"/> and <paramref name="y"/> offsets to the content stream with a Td operator
    /// </summary>
    /// <param name="x">The x offset</param>
    /// <param name="y">The y offset</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineAndOffsetTextPosition(double x, double y)
    {
        InnerStream
            .Write(x)
            .Space()
            .Write(y)
            .Space()
            .Write("Td")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the <paramref name="offset"/> to the content stream with a TD operator
    /// </summary>
    /// <param name="offset">The offset to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineSetLeadingAndOffsetTextPosition(Point offset)
        => MoveNextLineSetLeadingAndOffsetTextPosition(offset.X.AsRaw(Unit.Points), offset.Y.AsRaw(Unit.Points));

    /// <summary>
    /// Write the <paramref name="x"/> and <paramref name="y"/> offsets to the content stream with a TD operator
    /// </summary>
    /// <param name="x">The x offset</param>
    /// <param name="y">The y offset</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveNextLineSetLeadingAndOffsetTextPosition(double x, double y)
    {
        InnerStream
            .Write(x)
            .Space()
            .Write(y)
            .Space()
            .Write("TD")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write a text transformation matrix operator (Tm) to the stream
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix"/> to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Tm(Matrix matrix)
    {
        InnerStream
            .Write(matrix.A)
            .Space()
            .Write(matrix.B)
            .Space()
            .Write(matrix.C)
            .Space()
            .Write(matrix.D)
            .Space()
            .Write(matrix.E)
            .Space()
            .Write(matrix.F)
            .Space()
            .Write("Tm")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write a xobject paint operator (Do) to the stream
    /// </summary>
    /// <param name="resource">The <see cref="PdfName"/> of the xobject to write.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Paint(PdfName resource)
    {
        _serializer.WriteDirect(resource);
        InnerStream.Space().Write("Do").NewLine();

        return this;
    }

    /// <summary>
    /// Set an extended graphics state (ExtGState) dictionary using a gs operator..
    /// </summary>
    /// <param name="state">The state to apply.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetExtendedGraphicsState(ExtendedGraphicsState state)
    {
        var name = Resources.Add(state);

        _serializer.WriteDirect(name);
        InnerStream.Space().Write("gs").NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (m) to the stream
    /// </summary>
    /// <param name="x">The x coordinate of the target point.</param>
    /// <param name="y">The y coordinate of the target point.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream MoveTo(double x, double y)
    {
        InnerStream
            .Write(x).Space().Write(y).Space()
            .Write('m').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (l) to the stream
    /// </summary>
    /// <param name="x">The x coordinate of the target point.</param>
    /// <param name="y">The y coordinate of the target point.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream LineTo(double x, double y)
    {
        InnerStream
            .Write(x).Space().Write(y).Space()
            .Write('l').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (c) to the stream
    /// </summary>
    /// <param name="x1">The x coordinate of the first control point</param>
    /// <param name="y1">The y coordinate of the first control point</param>
    /// <param name="x2">The x coordinate of the second control point</param>
    /// <param name="y2">The y coordinate of the second control point</param>
    /// <param name="x3">The x coordinate of the target point.</param>
    /// <param name="y3">The y coordinate of the target point.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CubicBezierCurve(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        InnerStream
            .Write(x1).Space().Write(y1).Space()
            .Write(x2).Space().Write(y2).Space()
            .Write(x3).Space().Write(y3).Space()
            .Write('c').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (v) to the stream
    /// </summary>
    /// <param name="x2">The x coordinate of the second control point</param>
    /// <param name="y2">The y coordinate of the second control point</param>
    /// <param name="x3">The x coordinate of the target point.</param>
    /// <param name="y3">The y coordinate of the target point.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CubicBezierCurveV(double x2, double y2, double x3, double y3)
    {
        InnerStream
            .Write(x2).Space().Write(y2).Space()
            .Write(x3).Space().Write(y3).Space()
            .Write('v').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (y) to the stream
    /// </summary>
    /// <param name="x1">The x coordinate of the first control point</param>
    /// <param name="y1">The y coordinate of the first control point</param>
    /// <param name="x3">The x coordinate of the target point and second control point.</param>
    /// <param name="y3">The y coordinate of the target point and second control point.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CubicBezierCurveY(double x1, double y1, double x3, double y3)
    {
        InnerStream
            .Write(x1).Space().Write(y1).Space()
            .Write(x3).Space().Write(y3).Space()
            .Write('y').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (re) to the stream
    /// </summary>
    /// <param name="rectangle">The rectangle to draw.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Rectangle(Rectangle rectangle)
    {
        return Rectangle(rectangle.LLX.AsRaw(Unit.Points), rectangle.LLY.AsRaw(Unit.Points), rectangle.Width.AsRaw(Unit.Points), rectangle.Height.AsRaw(Unit.Points));
    }

    /// <summary>
    /// Write the operator (re) to the stream
    /// </summary>
    /// <param name="x">The x coordinate of the lower left corner of the rectangle</param>
    /// <param name="y">The y coordinate of the lower left corner of the rectangle</param>
    /// <param name="width">The width of the rectangle</param>
    /// <param name="height">The height of the rectangle</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Rectangle(double x, double y, double width, double height)
    {
        InnerStream
            .Write(x).Space().Write(y).Space()
            .Write(width).Space().Write(height).Space()
            .Write("re").NewLine();

        return this;
    }

    /// <summary>
    /// Mark the current path for use as a clipping mask
    /// </summary>
    /// <param name="fillRule">The <see cref="FillRule"/> to use.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    /// <exception cref="InvalidOperationException">Will throw when a unsupported value for <paramref name="fillRule"/> is used.</exception>
    public ContentStream Clip(FillRule fillRule)
    {
        _ = fillRule switch
        {
            FillRule.NonZeroWindingNumber => InnerStream.Write('W').NewLine(),
            FillRule.EvenOdd => InnerStream.Write('W').Write('*').NewLine(),
            _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
        };

        return this;
    }

    /// <summary>
    /// Write the operator (h) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Close()
    {
        InnerStream
            .Write('h').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (S) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Stroke()
    {
        InnerStream
            .Write('S').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (s) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CloseAndStroke()
    {
        InnerStream
            .Write('s').NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (f or f*) to the stream
    /// </summary>
    /// <param name="fillRule">The fill rule to use.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream Fill(FillRule fillRule)
    {
        _ = fillRule switch
        {
            FillRule.NonZeroWindingNumber => InnerStream.Write('f').NewLine(),
            FillRule.EvenOdd => InnerStream.Write('f').Write('*').NewLine(),
            _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
        };

        return this;
    }

    /// <summary>
    /// Write the operator (B or B*) to the stream
    /// </summary>
    /// <param name="fillRule">The fill rule to use.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream FillAndStroke(FillRule fillRule)
    {
        _ = fillRule switch
        {
            FillRule.NonZeroWindingNumber => InnerStream.Write('B').NewLine(),
            FillRule.EvenOdd => InnerStream.Write('B').Write('*').NewLine(),
            _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
        };

        return this;
    }

    /// <summary>
    /// Write the operator (b or b*) to the stream
    /// </summary>
    /// <param name="fillRule">The fill rule to use.</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream CloseFillAndStroke(FillRule fillRule)
    {
        _ = fillRule switch
        {
            FillRule.NonZeroWindingNumber => InnerStream.Write('b').NewLine(),
            FillRule.EvenOdd => InnerStream.Write('b').Write('*').NewLine(),
            _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
        };

        return this;
    }

    /// <summary>
    /// Write the operator (n) to the stream
    /// </summary>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream EndPath()
    {
        InnerStream
            .Write('n').NewLine();

        return this;
    }

    /// <summary>
    /// Set the color used for filling operations
    /// </summary>
    /// <param name="color">The color to use</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    /// <exception cref="NotImplementedException">Will throw if the implementing color type is not supported.</exception>
    public ContentStream SetFillColor(Color color)
    {
        return color switch
        {
            GrayColor gray => SetFillColor(gray),
            RgbColor rgb => SetFillColor(rgb),
            CmykColor cmyk => SetFillColor(cmyk),
            SpotColor spot => SetFillColor(spot),
            _ => throw new NotImplementedException($"The color type {color.GetType().Name} is not implemented.")
        };
    }

    /// <summary>
    /// Set the color used for stroking operations
    /// </summary>
    /// <param name="color">The color to use</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    /// <exception cref="NotImplementedException">Will throw if the implementing color type is not supported.</exception>
    public ContentStream SetStrokeColor(Color color)
    {
        return color switch
        {
            GrayColor gray => SetStrokeColor(gray),
            RgbColor rgb => SetStrokeColor(rgb),
            CmykColor cmyk => SetStrokeColor(cmyk),
            SpotColor spot => SetStrokeColor(spot),
            _ => throw new NotImplementedException($"The color type {color.GetType().Name} is not implemented.")
        };
    }

    /// <summary>
    /// Write the operator (G) to the stream
    /// </summary>
    /// <param name="color">The color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetStrokeColor(GrayColor color)
    {
        InnerStream
            .Write(color.Gray)
            .Space()
            .Write('G')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (g) to the stream
    /// </summary>
    /// <param name="color">The color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetFillColor(GrayColor color)
    {
        InnerStream
            .Write(color.Gray)
            .Space()
            .Write('g')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (RG) to the stream
    /// </summary>
    /// <param name="color">The color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetStrokeColor(RgbColor color)
    {
        InnerStream
            .Write(color.Red)
            .Space()
            .Write(color.Green)
            .Space()
            .Write(color.Blue)
            .Space()
            .Write('R')
            .Write('G')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (rg) to the stream
    /// </summary>
    /// <param name="color">The color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetFillColor(RgbColor color)
    {
        InnerStream
            .Write(color.Red)
            .Space()
            .Write(color.Green)
            .Space()
            .Write(color.Blue)
            .Space()
            .Write('r')
            .Write('g')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (K) to the stream
    /// </summary>
    /// <param name="color">The CMYK color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetStrokeColor(CmykColor color)
    {
        InnerStream
            .Write(color.Cyan)
            .Space()
            .Write(color.Magenta)
            .Space()
            .Write(color.Yellow)
            .Space()
            .Write(color.Key)
            .Space()
            .Write('K')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (k) to the stream
    /// </summary>
    /// <param name="color">The CMYK color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetFillColor(CmykColor color)
    {
        InnerStream
            .Write(color.Cyan)
            .Space()
            .Write(color.Magenta)
            .Space()
            .Write(color.Yellow)
            .Space()
            .Write(color.Key)
            .Space()
            .Write('k')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (CS &amp; SCN) to the stream
    /// </summary>
    /// <param name="color">The spot color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetStrokeColor(SpotColor color)
    {
        var name = Resources.Add(color.Separation);

        _serializer.WriteDirect(name);
        InnerStream
            .Space()
            .Write("CS")
            .Space()
            .Write(color.Tint)
            .Space()
            .Write("SCN")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the operator (cs &amp; scn) to the stream
    /// </summary>
    /// <param name="color">The spot color to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetFillColor(SpotColor color)
    {
        var name = Resources.Add(color.Separation);

        _serializer.WriteDirect(name);
        InnerStream
            .Space()
            .Write("cs")
            .Space()
            .Write(color.Tint)
            .Space()
            .Write("scn")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the line width to the content stream
    /// </summary>
    /// <param name="width">The line width to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetLineWidth(double width)
    {
        InnerStream
            .Write(width)
            .Space()
            .Write('w')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the line cap style to the content stream
    /// </summary>
    /// <param name="lineCap">The style to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetLineCap(LineCapStyle lineCap)
    {
        InnerStream
            .Write((int)lineCap)
            .Space()
            .Write('J')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the line join style to the content stream
    /// </summary>
    /// <param name="lineJoin">The style to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetLineJoin(LineJoinStyle lineJoin)
    {
        InnerStream
            .Write((int)lineJoin)
            .Space()
            .Write('j')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the miter limit to the content stream
    /// </summary>
    /// <param name="miterLimit">The miter limit to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetMiterLimit(double miterLimit)
    {
        InnerStream
            .Write(miterLimit)
            .Space()
            .Write('M')
            .NewLine();

        return this;
    }

    /// <summary>
    /// Write the dash pattern to the content stream
    /// </summary>
    /// <param name="dashPattern">The dash pattern to write</param>
    /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
    public ContentStream SetDashPattern(Dash dashPattern)
    {
        InnerStream
            .Write(dashPattern.Array.ToArray())
            .Space()
            .Write(dashPattern.Phase)
            .Space()
            .Write('d')
            .NewLine();

        return this;
    }
}
