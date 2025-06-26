using Synercoding.FileFormats.Pdf.Parsing.Filters;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;

public class PredictorsTests
{
    [Fact]
    public void Test_DecodePng_EmptyInput()
    {
        var predictors = new Predictors();
        var input = Array.Empty<byte>();

        var result = predictors.DecodePng(input, 3);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_DecodePng_NullInput()
    {
        var predictors = new Predictors();

        var result = predictors.DecodePng(null!, 3);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(0, new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 })] // None filter
    [InlineData(1, new byte[] { 1, 1, 1 }, new byte[] { 1, 2, 3 })] // Sub filter
    public void Test_DecodePng_FilterTypes(byte filterType, byte[] filteredData, byte[] expected)
    {
        var predictors = new Predictors();
        var input = new List<byte> { filterType };
        input.AddRange(filteredData);

        var result = predictors.DecodePng(input.ToArray(), filteredData.Length);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_UpFilter()
    {
        var predictors = new Predictors();
        // First row: None filter [1, 2, 3]
        // Second row: Up filter [0, 1, 2] (should add to previous row: [1, 3, 5])
        var input = new byte[]
        {
            0, 1, 2, 3,  // First row: None filter
            2, 0, 1, 2   // Second row: Up filter
        };

        var result = predictors.DecodePng(input, 3);

        var expected = new byte[] { 1, 2, 3, 1, 3, 5 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_AverageFilter()
    {
        var predictors = new Predictors();
        // First row: None filter [10, 20, 30]
        // Second row: Average filter [0, 5, 12] (should decode to [5, 17, 35])
        var input = new byte[]
        {
            0, 10, 20, 30,  // First row: None filter
            3, 0, 5, 12     // Second row: Average filter
        };

        var result = predictors.DecodePng(input, 3);

        var expected = new byte[] { 10, 20, 30, 5, 17, 35 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_PaethFilter()
    {
        var predictors = new Predictors();
        // First row: None filter [100, 50, 25]
        // Second row: Paeth filter [10, 20, 55]
        var input = new byte[]
        {
            0, 100, 50, 25,  // First row: None filter
            4, 10, 20, 55    // Second row: Paeth filter
        };

        var result = predictors.DecodePng(input, 3);

        // Expected: first row [100, 50, 25], second row [110, 70, 105] (after Paeth prediction)
        var expected = new byte[] { 100, 50, 25, 110, 70, 105 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodePng_InvalidFilterType()
    {
        var predictors = new Predictors();
        var input = new byte[] { 5, 1, 2, 3 }; // Invalid filter type 5

        Assert.Throws<InvalidOperationException>(() => predictors.DecodePng(input, 3));
    }

    [Fact]
    public void Test_DecodeTiff_EmptyInput()
    {
        var predictors = new Predictors();
        var input = Array.Empty<byte>();

        var result = predictors.DecodeTiff(input, 3, 8, 1);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_DecodeTiff_NullInput()
    {
        var predictors = new Predictors();

        var result = predictors.DecodeTiff(null!, 3, 8, 1);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(8, 1)] // 8 bits per component, 1 component per sample
    [InlineData(8, 3)] // 8 bits per component, 3 components per sample (RGB)
    [InlineData(16, 1)] // 16 bits per component, 1 component per sample
    public void Test_DecodeTiff_BitsPerComponentAndColors(int bitsPerComponent, int componentsPerSample)
    {
        var predictors = new Predictors();
        
        // Create test data with differential encoding
        byte[] input;
        if (bitsPerComponent == 8)
        {
            if (componentsPerSample == 1)
            {
                // Single component: [10, 5, 3] -> should decode to [10, 15, 18]
                input = new byte[] { 10, 5, 3 };
            }
            else
            {
                // RGB: [10,20,30, 5,10,15] -> should decode to [10,20,30, 15,30,45]
                input = new byte[] { 10, 20, 30, 5, 10, 15 };
            }
        }
        else
        {
            // 16-bit: [0,10, 0,5] -> should decode to [0,10, 0,15] (big-endian)
            input = new byte[] { 0, 10, 0, 5 };
        }

        var result = predictors.DecodeTiff(input, 2, bitsPerComponent, componentsPerSample);

        Assert.NotNull(result);
        Assert.Equal(input.Length, result.Length);
        
        // First sample should remain unchanged
        if (bitsPerComponent == 8)
        {
            Assert.Equal(input[0], result[0]);
        }
        else
        {
            Assert.Equal(input[0], result[0]);
            Assert.Equal(input[1], result[1]);
        }
    }

    [Fact]
    public void Test_DecodeTiff_8Bit_SingleComponent()
    {
        var predictors = new Predictors();
        // Input: [100, 10, 5, 3] (4 samples)
        // Expected: [100, 110, 115, 118] (each sample adds to previous)
        var input = new byte[] { 100, 10, 5, 3 };

        var result = predictors.DecodeTiff(input, 4, 8, 1);

        var expected = new byte[] { 100, 110, 115, 118 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodeTiff_8Bit_RGB()
    {
        var predictors = new Predictors();
        // Input: [100,150,200, 10,20,30] (2 RGB samples)
        // Expected: [100,150,200, 110,170,230] (each component adds to corresponding component of previous sample)
        var input = new byte[] { 100, 150, 200, 10, 20, 30 };

        var result = predictors.DecodeTiff(input, 2, 8, 3);

        var expected = new byte[] { 100, 150, 200, 110, 170, 230 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodeTiff_16Bit_SingleComponent()
    {
        var predictors = new Predictors();
        // Input: [1,0, 0,10] (2 samples, big-endian: 256, 10)
        // Expected: [1,0, 1,10] (256, 266)
        var input = new byte[] { 1, 0, 0, 10 };

        var result = predictors.DecodeTiff(input, 2, 16, 1);

        var expected = new byte[] { 1, 0, 1, 10 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_DecodeTiff_UnsupportedBitsPerComponent()
    {
        var predictors = new Predictors();
        // Need enough data for 2 samples with 32 bits per component (4 bytes each) = 8 bytes total
        var input = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        Assert.Throws<NotSupportedException>(() => predictors.DecodeTiff(input, 2, 32, 1));
    }

    [Theory]
    [InlineData(1, 8, 1)]
    [InlineData(5, 8, 3)]
    [InlineData(10, 16, 1)]
    public void Test_DecodeTiff_MultipleRows(int columns, int bitsPerComponent, int componentsPerSample)
    {
        var predictors = new Predictors();
        int bytesPerComponent = (bitsPerComponent + 7) / 8;
        int bytesPerSample = bytesPerComponent * componentsPerSample;
        int bytesPerRow = columns * bytesPerSample;
        
        // Create input with 2 rows
        var input = new byte[bytesPerRow * 2];
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = (byte)(i % 256);
        }

        var result = predictors.DecodeTiff(input, columns, bitsPerComponent, componentsPerSample);

        Assert.NotNull(result);
        Assert.Equal(input.Length, result.Length);
        
        // First sample of each row should remain unchanged
        Assert.Equal(input[0], result[0]);
        Assert.Equal(input[bytesPerRow], result[bytesPerRow]);
    }
}