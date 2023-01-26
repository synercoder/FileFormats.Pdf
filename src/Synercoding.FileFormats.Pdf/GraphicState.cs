using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using System.Linq;

namespace Synercoding.FileFormats.Pdf;

public sealed class GraphicState
{
    internal GraphicState()
    {
        CTM = Matrix.Identity;
        FillColor = PredefinedColors.Black;
        StrokeColor = PredefinedColors.Black;
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

    public Matrix CTM { get; internal set; } 
    public Color FillColor { get; internal set; } 
    public Color StrokeColor { get; internal set; } 
    public double LineWidth { get; internal set; } 
    public LineCapStyle LineCap { get; internal set; } 
    public LineJoinStyle LineJoin { get; internal set; }
    public double MiterLimit { get; internal set; }
    public Dash DashPattern { get; internal set; } 
    public double CharacterSpacing { get; internal set; } 
    public double WordSpacing { get; internal set; } 
    public double HorizontalScaling { get; internal set; }
    public double TextLeading { get; internal set; }
    public Font? Font { get; internal set; }
    public double? FontSize { get; internal set; }
    public TextRenderingMode TextRenderingMode { get; internal set; }
    public double TextRise { get; internal set; }

    internal GraphicState Clone()
    {
        return new GraphicState()
        {
            CTM = CTM,
            FillColor = FillColor,
            StrokeColor = StrokeColor,
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
            FontSize = FontSize,
            TextRenderingMode = TextRenderingMode,
            TextRise = TextRise
        };
    }
}

