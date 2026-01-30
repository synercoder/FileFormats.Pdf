namespace Synercoding.FileFormats.Pdf.Tests;

public class PageLayoutTests
{
    [Fact]
    public void Test_PageLayout_AllValues_HaveCorrectNames()
    {
        // Test that enum values match expected PDF specification names
        Assert.Equal("SinglePage", PageLayout.SinglePage.ToString());
        Assert.Equal("OneColumn", PageLayout.OneColumn.ToString());
        Assert.Equal("TwoColumnLeft", PageLayout.TwoColumnLeft.ToString());
        Assert.Equal("TwoColumnRight", PageLayout.TwoColumnRight.ToString());
        Assert.Equal("TwoPageLeft", PageLayout.TwoPageLeft.ToString());
        Assert.Equal("TwoPageRight", PageLayout.TwoPageRight.ToString());
    }

    [Fact]
    public void Test_PageLayout_AllValues_CanBeEnumerated()
    {
        // Arrange
        var expectedValues = new[]
        {
            PageLayout.SinglePage,
            PageLayout.OneColumn,
            PageLayout.TwoColumnLeft,
            PageLayout.TwoColumnRight,
            PageLayout.TwoPageLeft,
            PageLayout.TwoPageRight
        };

        // Act
        var allValues = Enum.GetValues<PageLayout>();

        // Assert
        Assert.Equal(expectedValues.Length, allValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, allValues);
        }
    }

    [Theory]
    [InlineData(PageLayout.SinglePage)]
    [InlineData(PageLayout.OneColumn)]
    [InlineData(PageLayout.TwoColumnLeft)]
    [InlineData(PageLayout.TwoColumnRight)]
    [InlineData(PageLayout.TwoPageLeft)]
    [InlineData(PageLayout.TwoPageRight)]
    public void Test_PageLayout_CanParseFromString(PageLayout pageLayout)
    {
        // Act
        var parsed = Enum.Parse<PageLayout>(pageLayout.ToString());

        // Assert
        Assert.Equal(pageLayout, parsed);
    }

    [Fact]
    public void Test_PageLayout_DefaultValue_IsSinglePage()
    {
        // Arrange
        PageLayout defaultValue = default;

        // Assert
        Assert.Equal(PageLayout.SinglePage, defaultValue);
    }

    [Fact]
    public void Test_PageLayout_ValuesAreUnique()
    {
        // Arrange
        var allValues = Enum.GetValues<PageLayout>();

        // Act
        var distinctValues = allValues.Distinct().ToArray();

        // Assert
        Assert.Equal(allValues.Length, distinctValues.Length);
    }

    [Theory]
    [InlineData("SinglePage", PageLayout.SinglePage)]
    [InlineData("OneColumn", PageLayout.OneColumn)]
    [InlineData("TwoColumnLeft", PageLayout.TwoColumnLeft)]
    [InlineData("TwoColumnRight", PageLayout.TwoColumnRight)]
    [InlineData("TwoPageLeft", PageLayout.TwoPageLeft)]
    [InlineData("TwoPageRight", PageLayout.TwoPageRight)]
    public void Test_PageLayout_StringRepresentation_MatchesExpected(string expected, PageLayout pageLayout)
    {
        // Act
        var stringValue = pageLayout.ToString();

        // Assert
        Assert.Equal(expected, stringValue);
    }

    [Fact]
    public void Test_PageLayout_SinglePageTwoPageDifference()
    {
        // Test the conceptual difference between single page and two page layouts

        // Single page layouts
        var singlePageLayouts = new[] { PageLayout.SinglePage, PageLayout.OneColumn };

        // Two page layouts  
        var twoPageLayouts = new[] { PageLayout.TwoPageLeft, PageLayout.TwoPageRight };

        // Two column layouts
        var twoColumnLayouts = new[] { PageLayout.TwoColumnLeft, PageLayout.TwoColumnRight };

        // Assert they are all different categories
        Assert.All(singlePageLayouts, layout =>
            Assert.DoesNotContain(layout, twoPageLayouts.Concat(twoColumnLayouts)));
        Assert.All(twoPageLayouts, layout =>
            Assert.DoesNotContain(layout, singlePageLayouts.Concat(twoColumnLayouts)));
        Assert.All(twoColumnLayouts, layout =>
            Assert.DoesNotContain(layout, singlePageLayouts.Concat(twoPageLayouts)));
    }

    [Theory]
    [InlineData(PageLayout.TwoColumnLeft, PageLayout.TwoColumnRight)]
    [InlineData(PageLayout.TwoPageLeft, PageLayout.TwoPageRight)]
    public void Test_PageLayout_LeftRightPairs_AreDifferent(PageLayout left, PageLayout right)
    {
        // Assert that left/right variants are different
        Assert.NotEqual(left, right);

        // Assert they both contain the expected directional indicators
        Assert.Contains("Left", left.ToString());
        Assert.Contains("Right", right.ToString());
    }
}