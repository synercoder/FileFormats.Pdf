using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class XRefSubsectionTests
{
    [Fact]
    public void Test_Constructor_ValidParams_CreatesSubsection()
    {
        // Arrange
        var entries = new List<XRefEntry>
        {
            new XRefEntry(100, 0, true),
            new XRefEntry(200, 0, true),
            new XRefEntry(300, 0, true)
        };

        // Act
        var subsection = new XRefSubsection(1, entries);

        // Assert
        Assert.Equal(1, subsection.FirstObjectNumber);
        Assert.Equal(3, subsection.Count);
        Assert.Equal(entries, subsection.Entries);
    }

    [Fact]
    public void Test_Constructor_EmptyEntries_CreatesSubsectionWithZeroCount()
    {
        // Arrange
        var entries = new List<XRefEntry>();

        // Act
        var subsection = new XRefSubsection(5, entries);

        // Assert
        Assert.Equal(5, subsection.FirstObjectNumber);
        Assert.Equal(0, subsection.Count);
        Assert.Empty(subsection.Entries);
    }

    [Fact]
    public void Test_Constructor_NullEntries_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new XRefSubsection(1, null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void Test_FirstObjectNumber_VariousValues_StoresCorrectly(int firstObjectNumber)
    {
        // Arrange
        var entries = new List<XRefEntry> { new XRefEntry(123, 0, true) };

        // Act
        var subsection = new XRefSubsection(firstObjectNumber, entries);

        // Assert
        Assert.Equal(firstObjectNumber, subsection.FirstObjectNumber);
    }

    [Fact]
    public void Test_Count_MatchesEntriesCount()
    {
        // Arrange
        var entries = new List<XRefEntry>();
        for (int i = 0; i < 10; i++)
        {
            entries.Add(new XRefEntry(i * 100, 0, true));
        }

        // Act
        var subsection = new XRefSubsection(1, entries);

        // Assert
        Assert.Equal(entries.Count, subsection.Count);
        Assert.Equal(10, subsection.Count);
    }

    [Fact]
    public void Test_Entries_IsReadOnly()
    {
        // Arrange
        var entries = new List<XRefEntry>
        {
            new XRefEntry(100, 0, true),
            new XRefEntry(200, 0, true)
        };

        // Act
        var subsection = new XRefSubsection(1, entries);

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<XRefEntry>>(subsection.Entries);
        Assert.Equal(2, subsection.Entries.Count);
        Assert.Equal(100, subsection.Entries[0].ByteOffset);
        Assert.Equal(200, subsection.Entries[1].ByteOffset);
    }
}

public class XRefEntryTests
{
    [Fact]
    public void Test_Constructor_ValidParams_CreatesEntry()
    {
        // Act
        var entry = new XRefEntry(12345, 2, true);

        // Assert
        Assert.Equal(12345, entry.ByteOffset);
        Assert.Equal(2, entry.Generation);
        Assert.True(entry.InUse);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(0, 0, false)]
    [InlineData(999999, 65535, true)]
    [InlineData(123456789, 1, false)]
    public void Test_Properties_VariousValues_StoreCorrectly(long byteOffset, int generation, bool inUse)
    {
        // Act
        var entry = new XRefEntry(byteOffset, generation, inUse);

        // Assert
        Assert.Equal(byteOffset, entry.ByteOffset);
        Assert.Equal(generation, entry.Generation);
        Assert.Equal(inUse, entry.InUse);
    }

    [Fact]
    public void Test_FreeObject_Generation65535_InUseFalse()
    {
        // Arrange & Act
        var freeEntry = new XRefEntry(0, 65535, false);

        // Assert
        Assert.Equal(0, freeEntry.ByteOffset);
        Assert.Equal(65535, freeEntry.Generation);
        Assert.False(freeEntry.InUse);
    }

    [Fact]
    public void Test_UsedObject_TypicalValues()
    {
        // Arrange & Act
        var usedEntry = new XRefEntry(1234, 0, true);

        // Assert
        Assert.Equal(1234, usedEntry.ByteOffset);
        Assert.Equal(0, usedEntry.Generation);
        Assert.True(usedEntry.InUse);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(long.MinValue)]
    public void Test_NegativeByteOffset_AcceptsNegativeValues(long byteOffset)
    {
        // Note: While PDF spec typically doesn't have negative offsets,
        // the class itself doesn't enforce this constraint

        // Act
        var entry = new XRefEntry(byteOffset, 0, true);

        // Assert
        Assert.Equal(byteOffset, entry.ByteOffset);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void Test_NegativeGeneration_AcceptsNegativeValues(int generation)
    {
        // Note: While PDF spec typically doesn't have negative generations,
        // the class itself doesn't enforce this constraint

        // Act
        var entry = new XRefEntry(123, generation, true);

        // Assert
        Assert.Equal(generation, entry.Generation);
    }
}