using System.Text;

namespace Synercoding.FileFormats.Pdf.Internals;
internal static class StringHelper
{
    public static IEnumerable<string> SplitOnNewLines(string text)
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
