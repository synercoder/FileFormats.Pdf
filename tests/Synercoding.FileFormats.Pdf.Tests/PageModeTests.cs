namespace Synercoding.FileFormats.Pdf.Tests;

public class PageModeTests
{
    [Fact]
    public void Test_PageMode_AllValues_HaveCorrectNames()
    {
        // Test that enum values match expected PDF specification names
        Assert.Equal("UseNone", PageMode.UseNone.ToString());
        Assert.Equal("UseOutlines", PageMode.UseOutlines.ToString());
        Assert.Equal("UseThumbs", PageMode.UseThumbs.ToString());
        Assert.Equal("FullScreen", PageMode.FullScreen.ToString());
        Assert.Equal("UseOC", PageMode.UseOC.ToString());
        Assert.Equal("UseAttachments", PageMode.UseAttachments.ToString());
    }

    [Fact]
    public void Test_PageMode_AllValues_CanBeEnumerated()
    {
        // Arrange
        var expectedValues = new[]
        {
            PageMode.UseNone,
            PageMode.UseOutlines,
            PageMode.UseThumbs,
            PageMode.FullScreen,
            PageMode.UseOC,
            PageMode.UseAttachments
        };

        // Act
        var allValues = Enum.GetValues<PageMode>();

        // Assert
        Assert.Equal(expectedValues.Length, allValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, allValues);
        }
    }

    [Theory]
    [InlineData(PageMode.UseNone)]
    [InlineData(PageMode.UseOutlines)]
    [InlineData(PageMode.UseThumbs)]
    [InlineData(PageMode.FullScreen)]
    [InlineData(PageMode.UseOC)]
    [InlineData(PageMode.UseAttachments)]
    public void Test_PageMode_CanParseFromString(PageMode pageMode)
    {
        // Act
        var parsed = Enum.Parse<PageMode>(pageMode.ToString());

        // Assert
        Assert.Equal(pageMode, parsed);
    }

    [Fact]
    public void Test_PageMode_DefaultValue_IsUseNone()
    {
        // Arrange
        PageMode defaultValue = default;

        // Assert
        Assert.Equal(PageMode.UseNone, defaultValue);
    }

    [Fact]
    public void Test_PageMode_ValuesAreUnique()
    {
        // Arrange
        var allValues = Enum.GetValues<PageMode>();

        // Act
        var distinctValues = allValues.Distinct().ToArray();

        // Assert
        Assert.Equal(allValues.Length, distinctValues.Length);
    }

    [Theory]
    [InlineData("UseNone", PageMode.UseNone)]
    [InlineData("UseOutlines", PageMode.UseOutlines)]
    [InlineData("UseThumbs", PageMode.UseThumbs)]
    [InlineData("FullScreen", PageMode.FullScreen)]
    [InlineData("UseOC", PageMode.UseOC)]
    [InlineData("UseAttachments", PageMode.UseAttachments)]
    public void Test_PageMode_StringRepresentation_MatchesExpected(string expected, PageMode pageMode)
    {
        // Act
        var stringValue = pageMode.ToString();

        // Assert
        Assert.Equal(expected, stringValue);
    }
}