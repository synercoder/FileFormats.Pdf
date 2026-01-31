using System.Text;

namespace Synercoding.FileFormats.Pdf.Logging;

internal static class SimpleLogFormatter
{
    public static string Format(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        return exception is not null
            ? $"[{level}]: {category}: {_format(message, args)}\n{exception.ToString()}"
            : $"[{level}]: {category}: {_format(message, args)}";
    }

    private static string _format(string message, object?[] args)
    {
        var builder = new StringBuilder();

        bool inArgument = false;
        int argumentIndex = 0;

        for (int i = 0; i < message.Length; i++)
        {
            var atEnd = ( i + 1 ) == message.Length;

            if (message[i] == '{' && !atEnd && message[i + 1] == '{')
            {
                i++;
                builder.Append('{');
            }
            else if (message[i] == '}' && !atEnd && message[i + 1] == '}')
            {
                i++;
                builder.Append('}');
            }
            else if (message[i] == '{' && !inArgument)
            {
                inArgument = true;
                builder.Append(args?.ElementAtOrDefault(argumentIndex) ?? "null");
                argumentIndex++;
            }
            else if (message[i] == '}' && inArgument)
            {
                inArgument = false;
            }
            else if (!inArgument)
            {
                builder.Append(message[i]);
            }
        }

        return builder.ToString();
    }
}
