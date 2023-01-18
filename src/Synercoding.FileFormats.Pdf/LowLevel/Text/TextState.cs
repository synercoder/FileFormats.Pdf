using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;

namespace Synercoding.FileFormats.Pdf.LowLevel.Text
{
    /// <summary>
    /// Class representing the text state of a content stream
    /// </summary>
    public sealed class TextState
    {
        internal TextState()
            : this(StandardFonts.Helvetica, 12)
        { }

        /// <summary>
        /// Constructor for <see cref="TextState"/>
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="fontSize">The font size</param>
        public TextState(Type1StandardFont font, float fontSize)
        {
            Font = font;
            FontSize = fontSize;
        }

        /// <summary>
        /// The <see cref="Color"/> of the fill of the text
        /// </summary>
        /// <remarks>If null no filling will occur.</remarks>
        public Color? Fill { get; set; } = null;

        /// <summary>
        /// The <see cref="Color"/> of the stroke of the text
        /// </summary>
        /// <remarks>If null no stroking will occur.</remarks>
        public Color? Stroke { get; set; } = null;

        /// <summary>
        /// The width of the line
        /// </summary>
        public double? LineWidth { get; set; } = 1;

        /// <summary>
        /// The dash settings of the path
        /// </summary>
        public Dash? Dash { get; set; } = new Dash();

        /// <summary>
        /// The miter limit
        /// </summary>
        public double? MiterLimit { get; set; } = 10;

        /// <summary>
        /// The <see cref="LineCapStyle"/> of the path
        /// </summary>
        public LineCapStyle? LineCap { get; set; }

        /// <summary>
        /// The <see cref="LineJoinStyle"/> of the path
        /// </summary>
        public LineJoinStyle? LineJoin { get; set; }

        /// <summary>
        /// The type1 standard font
        /// </summary>
        public Type1StandardFont Font { get; set; }

        /// <summary>
        /// The font size
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// Set the text rise
        /// </summary>
        public float? TextRise { get; set; }

        /// <summary>
        /// The text rendering mode
        /// </summary>
        public TextRenderingMode? RenderingMode { get; set; }

        /// <summary>
        /// Set the leading for the text
        /// </summary>
        public float? Leading { get; set; }

        /// <summary>
        /// The horizontal scaling of the text
        /// </summary>
        public float? HorizontalScaling { get; set; }

        /// <summary>
        /// The spacing between the words
        /// </summary>
        public float? WordSpacing { get; set; }

        /// <summary>
        /// The spacing between the character glyps
        /// </summary>
        public float? CharacterSpacing { get; set; }
    }
}
