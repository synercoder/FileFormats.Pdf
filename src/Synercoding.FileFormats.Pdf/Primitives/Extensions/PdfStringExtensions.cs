using System.Globalization;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;

public static class PdfStringExtensions
{
    public static bool TryParseAsDateTimeOffset(this PdfString pdfString, out DateTimeOffset dateTimeOffset)
    {
        _ = pdfString ?? throw new ArgumentNullException(nameof(pdfString));

        dateTimeOffset = default;

        try
        {
            var raw = pdfString.Value;

            if (!raw.StartsWith("D:"))
                return false;

            var end = raw.EndsWith('\'')
                ? 1
                : 0;

            var formats = new string[]
            {
                "yyyyMMddHHmmsszz\\\'",
                "yyyyMMddHHmmsszz",
                "yyyyMMddHHmmss\\Z00\\\'00",
                "yyyyMMddHHmmss\\Z00\\\'",
                "yyyyMMddHHmmss\\Z00",
                "yyyyMMddHHmmss\\Z",
                "yyyyMMddHHmmss",
                "yyyyMMddHHmm",
                "yyyyMMddHH",
                "yyyyMMdd",
                "yyyyMM",
                "yyyy",
            };

            var formatProvider = CultureInfo.InvariantCulture.DateTimeFormat;

            if (DateTimeOffset.TryParseExact(raw[2..^end], formats, formatProvider, DateTimeStyles.AssumeUniversal, out dateTimeOffset))
                return true;

            if (DateTimeOffset.TryParseExact(raw[2..^( end + 2 )], formats[..2], formatProvider, DateTimeStyles.AssumeUniversal, out dateTimeOffset)
                && int.TryParse(raw[( raw.Length - end - 2 )..^end], out int minuteOffset))
            {
                var sign = dateTimeOffset.Offset.Hours < 0
                    ? -1
                    : 1;
                dateTimeOffset = new DateTimeOffset(dateTimeOffset.DateTime, new TimeSpan(dateTimeOffset.Offset.Hours, minuteOffset * sign, 0));
                return true;
            }
        }
        catch
        { }

        return false;
    }
}
