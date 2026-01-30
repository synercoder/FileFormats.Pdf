using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class TableBuilderTests
{
    [Fact]
    public void Test_ReserveId_ReturnsUniqueIds()
    {
        // Arrange
        var builder = new TableBuilder();

        // Act
        var id1 = builder.ReserveId();
        var id2 = builder.ReserveId();
        var id3 = builder.ReserveId();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id2, id3);
        Assert.NotEqual(id1, id3);
    }

    [Fact]
    public void Test_ReserveId_GeneratesSequentialObjectNumbers()
    {
        // Arrange
        var builder = new TableBuilder();

        // Act
        var id1 = builder.ReserveId();
        var id2 = builder.ReserveId();
        var id3 = builder.ReserveId();

        // Assert
        Assert.Equal(1, id1.ObjectNumber);
        Assert.Equal(2, id2.ObjectNumber);
        Assert.Equal(3, id3.ObjectNumber);

        // All should have generation 0
        Assert.Equal(0, id1.Generation);
        Assert.Equal(0, id2.Generation);
        Assert.Equal(0, id3.Generation);
    }

    [Fact]
    public void Test_TrySetPosition_ValidPosition_ReturnsTrue()
    {
        // Arrange
        var builder = new TableBuilder();
        var id = builder.ReserveId();

        // Act
        var result = builder.TrySetPosition(id, 123);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Test_TrySetPosition_NegativePosition_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var builder = new TableBuilder();
        var id = builder.ReserveId();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.TrySetPosition(id, -1));
    }

    [Fact]
    public void Test_TrySetPosition_AlreadySet_ReturnsFalse()
    {
        // Arrange
        var builder = new TableBuilder();
        var id = builder.ReserveId();
        builder.TrySetPosition(id, 123);

        // Act
        var result = builder.TrySetPosition(id, 456);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Test_Validate_AllPositionsSet_ReturnsTrue()
    {
        // Arrange
        var builder = new TableBuilder();
        var id1 = builder.ReserveId();
        var id2 = builder.ReserveId();

        builder.TrySetPosition(id1, 100);
        builder.TrySetPosition(id2, 200);

        // Act
        var result = builder.Validate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Test_Validate_UnsetPosition_ReturnsFalse()
    {
        // Arrange
        var builder = new TableBuilder();
        var id1 = builder.ReserveId();
        var id2 = builder.ReserveId();

        builder.TrySetPosition(id1, 100);
        // id2 position not set

        // Act
        var result = builder.Validate();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Test_Validate_EmptyTable_ReturnsTrue()
    {
        // Arrange
        var builder = new TableBuilder();

        // Act
        var result = builder.Validate();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Test_Count_EmptyTable_ReturnsZero()
    {
        // Arrange
        var builder = new TableBuilder();

        // Act
        var count = builder.Count;

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void Test_Count_WithObjects_ReturnsCorrectCount()
    {
        // Arrange
        var builder = new TableBuilder();
        builder.ReserveId();
        builder.ReserveId();
        builder.ReserveId();

        // Act
        var count = builder.Count;

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public void Test_GetSubsections_EmptyTable_ReturnsOnlyFreeObject()
    {
        // Arrange
        var builder = new TableBuilder();

        // Act
        var subsections = builder.GetSubsections().ToArray();

        // Assert
        Assert.Single(subsections);
        var subsection = subsections[0];

        Assert.Equal(0, subsection.FirstObjectNumber);
        Assert.Equal(1, subsection.Count);
        Assert.Single(subsection.Entries);

        var entry = subsection.Entries[0];
        Assert.Equal(0, entry.ByteOffset);
        Assert.Equal(65535, entry.Generation);
        Assert.False(entry.InUse);
    }

    [Fact]
    public void Test_GetSubsections_SingleObject_ReturnsCorrectSubsection()
    {
        // Arrange
        var builder = new TableBuilder();
        var id = builder.ReserveId();
        builder.TrySetPosition(id, 123);

        // Act
        var subsections = builder.GetSubsections().ToArray();

        // Assert
        Assert.Single(subsections);
        var subsection = subsections[0];

        Assert.Equal(0, subsection.FirstObjectNumber);
        Assert.Equal(2, subsection.Count); // Free object + 1 real object

        var entries = subsection.Entries.ToArray();
        Assert.Equal(2, entries.Length);

        // Free object
        Assert.Equal(0, entries[0].ByteOffset);
        Assert.Equal(65535, entries[0].Generation);
        Assert.False(entries[0].InUse);

        // Real object
        Assert.Equal(123, entries[1].ByteOffset);
        Assert.Equal(0, entries[1].Generation);
        Assert.True(entries[1].InUse);
    }

    [Fact]
    public void Test_GetSubsections_MultipleConsecutiveObjects_ReturnsSingleSubsection()
    {
        // Arrange
        var builder = new TableBuilder();
        var id1 = builder.ReserveId(); // Object 1
        var id2 = builder.ReserveId(); // Object 2
        var id3 = builder.ReserveId(); // Object 3

        builder.TrySetPosition(id1, 100);
        builder.TrySetPosition(id2, 200);
        builder.TrySetPosition(id3, 300);

        // Act
        var subsections = builder.GetSubsections().ToArray();

        // Assert
        Assert.Single(subsections);
        var subsection = subsections[0];

        Assert.Equal(0, subsection.FirstObjectNumber);
        Assert.Equal(4, subsection.Count); // Free object + 3 real objects

        var entries = subsection.Entries.ToArray();
        Assert.Equal(4, entries.Length);

        // Free object
        Assert.Equal(0, entries[0].ByteOffset);
        Assert.Equal(65535, entries[0].Generation);
        Assert.False(entries[0].InUse);

        // Real objects
        Assert.Equal(100, entries[1].ByteOffset);
        Assert.True(entries[1].InUse);
        Assert.Equal(200, entries[2].ByteOffset);
        Assert.True(entries[2].InUse);
        Assert.Equal(300, entries[3].ByteOffset);
        Assert.True(entries[3].InUse);
    }

    [Fact]
    public void Test_GetSubsections_WithUnwrittenObject_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = new TableBuilder();
        var id1 = builder.ReserveId();
        var id2 = builder.ReserveId();

        builder.TrySetPosition(id1, 100);
        // id2 position not set

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.GetSubsections().ToArray());
        Assert.Contains("has not been written", exception.Message);
    }

    [Fact]
    public void Test_GetSubsections_OrdersByObjectNumber()
    {
        // Arrange
        var builder = new TableBuilder();
        var id1 = builder.ReserveId(); // Object 1
        var id2 = builder.ReserveId(); // Object 2  
        var id3 = builder.ReserveId(); // Object 3

        // Set positions in different order
        builder.TrySetPosition(id3, 300);
        builder.TrySetPosition(id1, 100);
        builder.TrySetPosition(id2, 200);

        // Act
        var subsections = builder.GetSubsections().ToArray();

        // Assert
        Assert.Single(subsections);
        var entries = subsections[0].Entries.ToArray();

        // Should be ordered: free object (0), then objects 1, 2, 3
        Assert.Equal(4, entries.Length);
        Assert.Equal(0, entries[0].ByteOffset); // Free object
        Assert.Equal(100, entries[1].ByteOffset); // Object 1
        Assert.Equal(200, entries[2].ByteOffset); // Object 2
        Assert.Equal(300, entries[3].ByteOffset); // Object 3
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void Test_TrySetPosition_VariousPositions_WorksCorrectly(long position)
    {
        // Arrange
        var builder = new TableBuilder();
        var id = builder.ReserveId();

        // Act
        var result = builder.TrySetPosition(id, position);

        // Assert
        Assert.True(result);

        var subsections = builder.GetSubsections().ToArray();
        var entries = subsections[0].Entries.ToArray();
        var objectEntry = entries.First(e => e.InUse);

        Assert.Equal(position, objectEntry.ByteOffset);
    }
}