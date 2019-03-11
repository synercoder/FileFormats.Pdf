using System;

namespace Synercoding.FileFormats.Pdf.Helpers
{
    internal static class PdfTypeHelper
    {
        public static string ToPdfHexadecimalString(string input)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            var builder = new System.Text.StringBuilder(bytes.Length + 2);
            builder.Append("<");
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("X2"));
            }
            builder.Append(">");
            return builder.ToString();
        }

        public static string ToPdfDate(DateTimeOffset input)
        {
            var datePart = input.ToString("yyyyMMddHHmmss");

            var builder = new System.Text.StringBuilder(22);
            builder.Append("(D:");
            builder.Append(datePart);

            var hours = input.Offset.Hours;
            var minutes = input.Offset.Minutes;

            if (hours == 0 && minutes == 0)
            {
                builder.Append("Z00'00");
            }
            else
            {
                if (hours > 0 || ( hours == 0 && minutes > 0 ))
                {
                    builder.Append("+");
                }
                else
                {
                    builder.Append("-");
                }
                builder.Append(Math.Abs(hours).ToString().PadLeft(2, '0'));
                builder.Append("'");
                builder.Append(minutes.ToString().PadLeft(2, '0'));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
