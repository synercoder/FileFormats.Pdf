using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf;

public interface ITextContentContext : IContentContext<ITextContentContext>
{
    double CharacterSpacing { get; }
    double WordSpacing { get; }
    double HorizontalScaling { get; }
    double TextLeading { get; }
    Font? Font { get; }
    double? FontSize { get; }
    TextRenderingMode TextRenderingMode { get; }
    double TextRise { get; }

    ITextContentContext SetCharacterSpacing(double spacing);
    ITextContentContext SetWordSpacing(double spacing);
    ITextContentContext SetHorizontalScaling(double scaling);
    ITextContentContext SetTextLeading(double leading);
    ITextContentContext SetFontAndSize(Font font, double size);
    ITextContentContext SetTextRenderingMode(TextRenderingMode mode);
    ITextContentContext SetTextRise(double textRise);

    ITextContentContext MoveToStartNewLine();
    ITextContentContext MoveToStartNextLine(double offsetX, double offsetY);
    ITextContentContext MoveToStartNextLineAndSetLeading(double offsetX, double offsetY);
    ITextContentContext ReplaceTextMatrix(Matrix matrix);

    ITextContentContext ShowText(string text);
    ITextContentContext ShowTextOnNextLine(string text);
    ITextContentContext ShowTextOnNextLine(string text, double wordSpacing, double characterSpacing);
}
