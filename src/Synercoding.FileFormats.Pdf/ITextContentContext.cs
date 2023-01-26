using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf;

public interface ITextContentContext : IContentContext<ITextContentContext>
{
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
