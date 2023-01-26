using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Context to enable text operations on the content
/// </summary>
public interface ITextContentContext : IContentContext<ITextContentContext>
{
    /// <summary>
    /// Set the character spacing
    /// </summary>
    /// <param name="spacing">The spacing value</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetCharacterSpacing(double spacing);

    /// <summary>
    /// Set the word spacing
    /// </summary>
    /// <param name="spacing">The spacing value</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetWordSpacing(double spacing);

    /// <summary>
    /// Set the horizontal scaling
    /// </summary>
    /// <param name="scaling">The scaling value</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetHorizontalScaling(double scaling);

    /// <summary>
    /// Set the text leading
    /// </summary>
    /// <param name="leading">The leading value</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetTextLeading(double leading);

    /// <summary>
    /// Set the font and size
    /// </summary>
    /// <param name="font">The font to set</param>
    /// <param name="size">The size to set</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetFontAndSize(Font font, double size);

    /// <summary>
    /// Set the text rendering mode
    /// </summary>
    /// <param name="mode">The mode to set</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetTextRenderingMode(TextRenderingMode mode);

    /// <summary>
    /// Set the text rise
    /// </summary>
    /// <param name="textRise">The text rise to set</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext SetTextRise(double textRise);

    /// <summary>
    /// Move to the start of a new line
    /// </summary>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext MoveToStartNewLine();

    /// <summary>
    /// Move to the start of a new line by way of <paramref name="offsetX"/> and <paramref name="offsetY"/>
    /// </summary>
    /// <param name="offsetX">The x offset to use</param>
    /// <param name="offsetY">The y offset to use</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext MoveToStartNextLine(double offsetX, double offsetY);

    /// <summary>
    /// Move to the start of a new line by way of <paramref name="offsetX"/> and <paramref name="offsetY"/>, and set text leading to <paramref name="offsetY"/>
    /// </summary>
    /// <param name="offsetX">The x offset to use</param>
    /// <param name="offsetY">The y offset to use</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext MoveToStartNextLineAndSetLeading(double offsetX, double offsetY);

    /// <summary>
    /// Replace the current text matrix with <paramref name="matrix"/>
    /// </summary>
    /// <param name="matrix">The new <see cref="Matrix"/> to use</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext ReplaceTextMatrix(Matrix matrix);

    /// <summary>
    /// Operation to show text
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext ShowText(string text);

    /// <summary>
    /// Operation to show text on the next line
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext ShowTextOnNextLine(string text);

    /// <summary>
    /// Operation to show text on the next line and setting the <see cref="GraphicState.WordSpacing"/> and <see cref="GraphicState.CharacterSpacing"/>
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <param name="wordSpacing">The word spacing to set</param>
    /// <param name="characterSpacing">The character spacing to set</param>
    /// <returns>This <see cref="ITextContentContext"/> to enable chaining operations</returns>
    ITextContentContext ShowTextOnNextLine(string text, double wordSpacing, double characterSpacing);
}
