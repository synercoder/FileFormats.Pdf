using Synercoding.FileFormats.Pdf.IO.Filters;

namespace Synercoding.FileFormats.Pdf.Tests.IO.Filters;

public class PredictorsTests
{
    [Fact]
    public void Test_DecodePng_EmptyInput()
    {
        var input = Array.Empty<byte>();

        var result = Predictors.DecodePng(input, 3);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_DecodePng_NullInput()
    {
        var result = Predictors.DecodePng(null!, 3);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(0, new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 })] // None filter
    [InlineData(1, new byte[] { 1, 1, 1 }, new byte[] { 1, 2, 3 })] // Sub filter
    public void Test_DecodePng_FilterTypes(byte filterType, byte[] filteredData, byte[] expected)
    {
        var input = new List<byte> { filterType };
        input.AddRange(filteredData);

        var result = Predictors.DecodePng(input.ToArray(), filteredData.Length);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_UpFilter()
    {
        // First row: None filter [1, 2, 3]
        // Second row: Up filter [0, 1, 2] (should add to previous row: [1, 3, 5])
        var input = new byte[]
        {
            0, 1, 2, 3,  // First row: None filter
            2, 0, 1, 2   // Second row: Up filter
        };

        var result = Predictors.DecodePng(input, 3);

        var expected = new byte[] { 1, 2, 3, 1, 3, 5 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_AverageFilter()
    {
        // First row: None filter [10, 20, 30]
        // Second row: Average filter [0, 5, 12] (should decode to [5, 17, 35])
        var input = new byte[]
        {
            0, 10, 20, 30,  // First row: None filter
            3, 0, 5, 12     // Second row: Average filter
        };

        var result = Predictors.DecodePng(input, 3);

        var expected = new byte[] { 10, 20, 30, 5, 17, 35 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_PaethFilter()
    {
        // First row: None filter [100, 50, 25]
        // Second row: Paeth filter [10, 20, 55]
        var input = new byte[]
        {
            0, 100, 50, 25,  // First row: None filter
            4, 10, 20, 55    // Second row: Paeth filter
        };

        var result = Predictors.DecodePng(input, 3);

        // Expected: first row [100, 50, 25], second row [110, 70, 105] (after Paeth prediction)
        var expected = new byte[] { 100, 50, 25, 110, 70, 105 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_InvalidFilterType()
    {
        var input = new byte[] { 5, 1, 2, 3 }; // Invalid filter type 5

        Assert.Throws<InvalidOperationException>(() => Predictors.DecodePng(input, 3));
    }
}
