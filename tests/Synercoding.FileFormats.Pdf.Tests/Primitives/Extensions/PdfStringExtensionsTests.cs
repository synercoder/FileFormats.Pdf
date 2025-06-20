using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives.Extensions;

public class PdfStringExtensionsTests
{
    [Theory]
    [InlineData("D:20250620023915+02'30'")]
    [InlineData("D:20250620023915+02'30")]
    [InlineData("D:20250620023915+02''")]
    [InlineData("D:20250620023915+02'")]
    [InlineData("D:20250620023915+02")]
    [InlineData("D:20250620023915Z'")]
    [InlineData("D:20250620023915Z")]
    [InlineData("D:20250620023915'")]
    [InlineData("D:20250620023915")]
    [InlineData("D:202506200239'")]
    [InlineData("D:202506200239")]
    [InlineData("D:2025062002'")]
    [InlineData("D:2025062002")]
    [InlineData("D:20250620'")]
    [InlineData("D:20250620")]
    [InlineData("D:202506'")]
    [InlineData("D:202506")]
    [InlineData("D:2025'")]
    [InlineData("D:2025")]
    public void Test_TryParseAsDateTimeOffset_ReturnsTrue(string input)
    {
        var pdfString = _encodeToPdfString(input);

        var actual = pdfString.TryParseAsDateTimeOffset(out _);

        Assert.True(actual);
    }

    [Theory]
    [InlineData("D:20250620023915+02'30'", "2025-06-20T02:39:15+02:30")]
    [InlineData("D:20250620023915-05'00'", "2025-06-20T02:39:15-05:00")]
    [InlineData("D:20250620023915+00'00'", "2025-06-20T02:39:15+00:00")]
    [InlineData("D:20250620023915Z", "2025-06-20T02:39:15+00:00")]
    [InlineData("D:20250620023915", "2025-06-20T02:39:15+00:00")]
    [InlineData("D:202506200239", "2025-06-20T02:39:00+00:00")]
    [InlineData("D:2025062002", "2025-06-20T02:00:00+00:00")]
    [InlineData("D:20250620", "2025-06-20T00:00:00+00:00")]
    [InlineData("D:202506", "2025-06-01T00:00:00+00:00")]
    [InlineData("D:2025", "2025-01-01T00:00:00+00:00")]
    public void Test_TryParseAsDateTimeOffset_ParsedValuesCorrect(string input, string expectedIso)
    {
        var pdfString = _encodeToPdfString(input);
        var expected = DateTimeOffset.Parse(expectedIso);

        var success = pdfString.TryParseAsDateTimeOffset(out var actual);

        Assert.True(success);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("20250620023915")]
    [InlineData("D:")]
    [InlineData("D:invalid")]
    [InlineData("D:20251301")]
    [InlineData("D:20250632")]
    [InlineData("D:2025062025")]
    [InlineData("D:202506200261")]
    [InlineData("D:20250620023961")]
    [InlineData("D:20250620023915+25'00'")]
    [InlineData("D:20250620023915-15'00'")]
    [InlineData("NotADate")]
    public void Test_TryParseAsDateTimeOffset_InvalidFormats_ReturnsFalse(string input)
    {
        var pdfString = _encodeToPdfString(input);

        var success = pdfString.TryParseAsDateTimeOffset(out var result);

        Assert.False(success);
    }

    [Theory]
    [InlineData("D:20250620023915+02'30'", "02:30:00")]
    [InlineData("D:20250620023915-08'00'", "-08:00:00")]
    [InlineData("D:20250620023915+00'00'", "00:00:00")]
    [InlineData("D:20250620023915+14'00'", "14:00:00")]
    [InlineData("D:20250620023915-12'00'", "-12:00:00")]
    [InlineData("D:20250620023915+05'45'", "05:45:00")]
    [InlineData("D:20250620023915-03'30'", "-03:30:00")]
    public void Test_TryParseAsDateTimeOffset_TimezoneOffsets_Correct(string input, string expectedOffsetString)
    {
        var pdfString = _encodeToPdfString(input);
        var expectedOffset = TimeSpan.Parse(expectedOffsetString);

        var success = pdfString.TryParseAsDateTimeOffset(out var actual);

        Assert.True(success);
        Assert.Equal(expectedOffset, actual.Offset);
    }

    [Theory]
    [InlineData("D:2025", 2025, 1, 1, 0, 0, 0)]
    [InlineData("D:202506", 2025, 6, 1, 0, 0, 0)]
    [InlineData("D:20250620", 2025, 6, 20, 0, 0, 0)]
    [InlineData("D:2025062002", 2025, 6, 20, 2, 0, 0)]
    [InlineData("D:202506200239", 2025, 6, 20, 2, 39, 0)]
    [InlineData("D:20250620023915", 2025, 6, 20, 2, 39, 15)]
    public void Test_TryParseAsDateTimeOffset_PrecisionLevels_Correct(string input, int year, int month, int day, int hour, int minute, int second)
    {
        var pdfString = _encodeToPdfString(input);
        var expected = new DateTime(year, month, day, hour, minute, second);

        var success = pdfString.TryParseAsDateTimeOffset(out var actual);

        Assert.True(success);
        Assert.Equal(expected, actual.DateTime);
    }

    [Fact]
    public void Test_TryParseAsDateTimeOffset_NullInput_ThrowsArgumentNullException()
    {
        PdfString? nullString = null;

        Assert.Throws<ArgumentNullException>(() => nullString!.TryParseAsDateTimeOffset(out _));
    }

    [Theory]
    [InlineData("D:20250620023915Z")]
    [InlineData("D:20250620023915Z'")]
    [InlineData("D:20250620023915Z00'")]
    [InlineData("D:20250620023915Z00'00")]
    public void Test_TryParseAsDateTimeOffset_ZuluTime_ParsesAsUtc(string input)
    {
        var pdfString = _encodeToPdfString(input);

        var success = pdfString.TryParseAsDateTimeOffset(out var actual);

        Assert.True(success);
        Assert.Equal(TimeSpan.Zero, actual.Offset);
    }

    [Theory]
    [InlineData("D:19700101000000", "1970-01-01T00:00:00+00:00")]
    [InlineData("D:99991231235959", "9999-12-31T23:59:59+00:00")]
    [InlineData("D:20000229000000", "2000-02-29T00:00:00+00:00")]
    [InlineData("D:21000228235959", "2100-02-28T23:59:59+00:00")]
    public void Test_TryParseAsDateTimeOffset_BoundaryDates_ParseCorrectly(string input, string expectedIso)
    {
        var pdfString = _encodeToPdfString(input);
        var expected = DateTimeOffset.Parse(expectedIso);

        var success = pdfString.TryParseAsDateTimeOffset(out var actual);

        Assert.True(success);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("D:20250620023915+02'61'")]
    [InlineData("D:20250620023915+02'99'")]
    public void Test_TryParseAsDateTimeOffset_InvalidMinuteOffsets_CurrentlyAccepted(string input)
    {
        var pdfString = _encodeToPdfString(input);

        var success = pdfString.TryParseAsDateTimeOffset(out _);

        Assert.True(success);
    }

    private static PdfString _encodeToPdfString(string input)
    {
        return new PdfString([.. Encoding.UTF8.Preamble, .. Encoding.UTF8.GetBytes(input)], false);
    }
}
