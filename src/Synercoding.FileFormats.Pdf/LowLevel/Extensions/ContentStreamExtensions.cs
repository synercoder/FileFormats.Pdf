using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions
{
    /// <summary>
    /// Class containing extension methods for <see cref="ContentStream"/>.
    /// </summary>
    public static class ContentStreamExtensions
    {
        /// <summary>
        /// Write a (multi-line) text to the PDF
        /// </summary>
        /// <param name="stream">The <see cref="ContentStream"/> this extension method is for.</param>
        /// <param name="text">The text to write</param>
        /// <returns>The <paramref name="stream"/> to support chaining operations.</returns>
        public static ContentStream ShowText(this ContentStream stream, string text)
        {
            var lines = _splitOnNewLines(text).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    stream.ShowTextTj(lines[i]);
                }
                else
                {
                    stream.MoveNextLineShowText(lines[i]);
                }
            }

            return stream;
        }

        private static IEnumerable<string> _splitOnNewLines(string text)
        {
            var builder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var n = i < text.Length - 1
                    ? text[i + 1]
                    : '0';
                if (( c == '\r' && n == '\n' ) || c == '\r' || c == '\n')
                {
                    yield return builder.ToString();
                    builder.Clear();

                    if (n == '\n')
                        i++; // extra skip to also skip the \n
                }
                else
                {
                    builder.Append(c);
                }
            }

            yield return builder.ToString();
        }
    }
}
