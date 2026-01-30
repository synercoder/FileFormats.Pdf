using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class IdGeneratorTests
{
    [Fact]
    public void Test_Constructor_DefaultStartsAtOne()
    {
        // Arrange & Act
        var generator = new IdGenerator();
        var id = generator.GetId();

        // Assert
        Assert.Equal(1, id.ObjectNumber);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Test_Constructor_WithCustomStart(int start)
    {
        // Arrange & Act
        var generator = new IdGenerator(start);
        var id = generator.GetId();

        // Assert
        Assert.Equal(start, id.ObjectNumber);
    }

    [Fact]
    public void Test_GetId_IncrementsSequentially()
    {
        // Arrange
        var generator = new IdGenerator();

        // Act
        var id1 = generator.GetId();
        var id2 = generator.GetId();
        var id3 = generator.GetId();
        var id4 = generator.GetId();
        var id5 = generator.GetId();

        // Assert
        Assert.Equal(1, id1.ObjectNumber);
        Assert.Equal(2, id2.ObjectNumber);
        Assert.Equal(3, id3.ObjectNumber);
        Assert.Equal(4, id4.ObjectNumber);
        Assert.Equal(5, id5.ObjectNumber);
    }

    [Fact]
    public void Test_GetId_WithCustomStart_IncrementsSequentially()
    {
        // Arrange
        var generator = new IdGenerator(100);

        // Act
        var id1 = generator.GetId();
        var id2 = generator.GetId();
        var id3 = generator.GetId();

        // Assert
        Assert.Equal(100, id1.ObjectNumber);
        Assert.Equal(101, id2.ObjectNumber);
        Assert.Equal(102, id3.ObjectNumber);
    }

    [Fact]
    public void Test_GetId_LargeNumberOfIds()
    {
        // Arrange
        var generator = new IdGenerator();
        const int count = 10000;

        // Act
        var ids = new List<PdfObjectId>();
        for (int i = 0; i < count; i++)
        {
            ids.Add(generator.GetId());
        }

        // Assert
        Assert.Equal(count, ids.Count);
        for (int i = 0; i < ids.Count; i++)
        {
            Assert.Equal(i + 1, ids[i].ObjectNumber);
        }
    }

    [Fact]
    public void Test_GetId_StartingFromZero()
    {
        // Arrange & Act
        var generator = new IdGenerator(0);

        // Act & Assert - PdfObjectId doesn't allow 0, so this should throw
        Assert.Throws<ArgumentOutOfRangeException>(() => generator.GetId());
    }

    [Fact]
    public void Test_GetId_StartingFromNegative()
    {
        // Arrange & Act
        var generator = new IdGenerator(-5);

        // Act & Assert - PdfObjectId doesn't allow negative numbers, so this should throw
        Assert.Throws<ArgumentOutOfRangeException>(() => generator.GetId());
    }
}
