using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Text;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Class representing the grahpic state of a PDF at a certain moment in time.
/// </summary>
public sealed class GraphicsState
{
    internal GraphicsState()
    {
        CTM = Matrix.Identity;
        Fill = PredefinedColors.Black;
        Stroke = PredefinedColors.Black;
        LineWidth = 1.0;
        LineCap = LineCapStyle.ButtCap;
        LineJoin = LineJoinStyle.MiterJoin;
        MiterLimit = 10.0;
        DashPattern = new Dash();
        CharacterSpacing = 0.0;
        WordSpacing = 0.0;
        HorizontalScaling = 100.0;
        TextLeading = 0.0;
        Font = null;
        FontSize = null;
        TextRenderingMode = TextRenderingMode.Fill;
        TextRise = 0.0;
    }

    /// <summary>
    /// The current transformation matrix, which maps positions from user coordinates to device coordinates.
    /// This matrix is modified by each application of the coordinate transformation operator, cm
    /// </summary>
    public Matrix CTM { get; internal set; }

    /// <summary>
    /// The color used for filling operations
    /// </summary>
    public Color Fill { get; internal set; }

    /// <summary>
    /// The color used for stroking operations
    /// </summary>
    public Color Stroke { get; internal set; }

    /// <summary>
    /// The thickness, in user space units, of paths to be stroked.
    /// </summary>
    public double LineWidth { get; internal set; }

    /// <summary>
    /// A code specifying the shape of the endpoints for any open path that is stroked.
    /// </summary>
    public LineCapStyle LineCap { get; internal set; }

    /// <summary>
    /// A code specifying the shape of joints between connected segments of a stroked path.
    /// </summary>
    public LineJoinStyle LineJoin { get; internal set; }

    /// <summary>
    /// The maximum length of mitered line joins for stroked paths.
    /// This parameter limits the length of “spikes” produced when line segments join at sharp angles.
    /// </summary>
    public double MiterLimit { get; internal set; }

    /// <summary>
    /// A description of the dash pattern to be used when paths are stroked.
    /// </summary>
    public Dash DashPattern { get; internal set; }

    /// <summary>
    /// The spacing between characters
    /// </summary>
    public double CharacterSpacing { get; internal set; }

    /// <summary>
    /// The spacing between words
    /// </summary>
    public double WordSpacing { get; internal set; }

    /// <summary>
    /// The horizontal scaling is a number specifying the percentage of the normal width.
    /// </summary>
    public double HorizontalScaling { get; internal set; }

    /// <summary>
    /// The text leading is a number expressed in unscaled text space units.
    /// </summary>
    public double TextLeading { get; internal set; }

    /// <summary>
    /// The font used when placing text on the page
    /// </summary>
    public Font? Font { get; internal set; }

    internal FontUsageTracker? FontUsageTracker { get; set; }

    /// <summary>
    /// The font size used when placing text on the page
    /// </summary>
    public double? FontSize { get; internal set; }

    /// <summary>
    /// The text rendering mode, determines whether showing text causes glyph outlines to be stroked, filled, used as a clipping boundary, or some combination of the three.
    /// </summary>
    public TextRenderingMode TextRenderingMode { get; internal set; }

    /// <summary>
    /// Text rise, specifies the distance, in unscaled text space units, to move the baseline up or down from its default location.
    /// </summary>
    public double TextRise { get; internal set; }

    internal GraphicsState Clone()
    {
        return new GraphicsState()
        {
            CTM = CTM,
            Fill = Fill,
            Stroke = Stroke,
            LineWidth = LineWidth,
            LineCap = LineCap,
            LineJoin = LineJoin,
            MiterLimit = MiterLimit,
            DashPattern = new Dash(DashPattern.Array.ToArray(), DashPattern.Phase),
            CharacterSpacing = CharacterSpacing,
            WordSpacing = WordSpacing,
            HorizontalScaling = HorizontalScaling,
            TextLeading = TextLeading,
            Font = Font,
            FontUsageTracker = FontUsageTracker,
            FontSize = FontSize,
            TextRenderingMode = TextRenderingMode,
            TextRise = TextRise
        };
    }
}
