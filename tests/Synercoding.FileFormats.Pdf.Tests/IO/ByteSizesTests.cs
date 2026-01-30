using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Tests.IO;

public class ByteSizesTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(9, 1)]
    [InlineData(10, 2)]
    [InlineData(99, 2)]
    [InlineData(100, 3)]
    [InlineData(999, 3)]
    [InlineData(1000, 4)]
    [InlineData(12345, 5)]
    [InlineData(int.MaxValue, 10)]
    public void Test_Size_Int_PositiveValues(int value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Theory]
    [InlineData(-1, 2)]
    [InlineData(-9, 2)]
    [InlineData(-10, 3)]
    [InlineData(-99, 3)]
    [InlineData(-100, 4)]
    [InlineData(-999, 4)]
    [InlineData(-1000, 5)]
    [InlineData(-12345, 6)]
    [InlineData(int.MinValue, 11)]
    public void Test_Size_Int_NegativeValues(int value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Theory]
    [InlineData(0u, 1)]
    [InlineData(1u, 1)]
    [InlineData(9u, 1)]
    [InlineData(10u, 2)]
    [InlineData(99u, 2)]
    [InlineData(100u, 3)]
    [InlineData(999u, 3)]
    [InlineData(1000u, 4)]
    [InlineData(12345u, 5)]
    [InlineData(uint.MaxValue, 10)]
    public void Test_Size_UInt(uint value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Theory]
    [InlineData(0L, 1)]
    [InlineData(1L, 1)]
    [InlineData(9L, 1)]
    [InlineData(10L, 2)]
    [InlineData(99L, 2)]
    [InlineData(100L, 3)]
    [InlineData(999L, 3)]
    [InlineData(1000L, 4)]
    [InlineData(12345L, 5)]
    [InlineData(long.MaxValue, 19)]
    public void Test_Size_Long_PositiveValues(long value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Theory]
    [InlineData(-1L, 2)]
    [InlineData(-9L, 2)]
    [InlineData(-10L, 3)]
    [InlineData(-99L, 3)]
    [InlineData(-100L, 4)]
    [InlineData(-999L, 4)]
    [InlineData(-1000L, 5)]
    [InlineData(-12345L, 6)]
    [InlineData(long.MinValue, 20)]
    public void Test_Size_Long_NegativeValues(long value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Theory]
    [InlineData(0UL, 1)]
    [InlineData(1UL, 1)]
    [InlineData(9UL, 1)]
    [InlineData(10UL, 2)]
    [InlineData(99UL, 2)]
    [InlineData(100UL, 3)]
    [InlineData(999UL, 3)]
    [InlineData(1000UL, 4)]
    [InlineData(12345UL, 5)]
    [InlineData(ulong.MaxValue, 20)]
    public void Test_Size_ULong(ulong value, int expectedSize)
    {
        // Act
        var result = ByteSizes.Size(value);

        // Assert
        Assert.Equal(expectedSize, result);
    }

    [Fact]
    public void Test_Size_Int_EdgeCases()
    {
        // Test boundary values around powers of 10
        Assert.Equal(1, ByteSizes.Size(9));
        Assert.Equal(2, ByteSizes.Size(10));
        Assert.Equal(2, ByteSizes.Size(99));
        Assert.Equal(3, ByteSizes.Size(100));
        Assert.Equal(3, ByteSizes.Size(999));
        Assert.Equal(4, ByteSizes.Size(1000));
    }

    [Fact]
    public void Test_Size_Long_LargeValues()
    {
        // Test very large values
        Assert.Equal(10, ByteSizes.Size(1234567890L));
        Assert.Equal(15, ByteSizes.Size(123456789012345L));
        Assert.Equal(18, ByteSizes.Size(123456789012345678L));
    }

    [Fact]
    public void Test_Size_Consistency_Between_Overloads()
    {
        // Test that different overloads give consistent results for overlapping ranges
        var testValues = new[] { 0, 1, 10, 100, 1000, 12345 };

        foreach (var value in testValues)
        {
            var intResult = ByteSizes.Size(value);
            var uintResult = ByteSizes.Size((uint)value);
            var longResult = ByteSizes.Size((long)value);
            var ulongResult = ByteSizes.Size((ulong)value);

            Assert.Equal(intResult, uintResult);
            Assert.Equal(intResult, longResult);
            Assert.Equal(intResult, ulongResult);
        }
    }
}