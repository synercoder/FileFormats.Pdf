using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfObjectIdTests
{
    [Theory]
    [InlineData(1, 0)]
    [InlineData(42, 1)]
    [InlineData(123, 5)]
    [InlineData(999999, 99)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void Test_Constructor_SetsIdAndGeneration(int objectNumber, int generation)
    {
        var objectId = new PdfObjectId(objectNumber, generation);
        
        Assert.Equal(objectNumber, objectId.ObjectNumber);
        Assert.Equal(generation, objectId.Generation);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-42)]
    [InlineData(int.MinValue)]
    public void Test_Constructor_InvalidObjectNumber_ThrowsArgumentOutOfRangeException(int invalidObjectNumber)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new PdfObjectId(invalidObjectNumber, 0));
        
        Assert.Equal("objectNumber", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-42)]
    [InlineData(int.MinValue)]
    public void Test_Constructor_InvalidGeneration_ThrowsArgumentOutOfRangeException(int invalidGeneration)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new PdfObjectId(1, invalidGeneration));
        
        Assert.Equal("generation", exception.ParamName);
        Assert.Contains("Generation number must be a non-negative integer", exception.Message);
    }

    [Fact]
    public void Test_Constructor_BothInvalidParameters_ThrowsForObjectNumber()
    {
        // Should throw for objectNumber first since it's checked first
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new PdfObjectId(-1, -1));
        
        Assert.Equal("objectNumber", exception.ParamName);
    }

    [Fact]
    public void Test_Equals_SameIdAndGeneration_ReturnsTrue()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(42, 1);
        
        Assert.True(id1.Equals(id2));
        Assert.True(id1 == id2);
        Assert.False(id1 != id2);
    }

    [Fact]
    public void Test_Equals_DifferentId_ReturnsFalse()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(43, 1);
        
        Assert.False(id1.Equals(id2));
        Assert.False(id1 == id2);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Test_Equals_DifferentGeneration_ReturnsFalse()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(42, 2);
        
        Assert.False(id1.Equals(id2));
        Assert.False(id1 == id2);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Test_Equals_BothDifferent_ReturnsFalse()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(43, 2);
        
        Assert.False(id1.Equals(id2));
        Assert.False(id1 == id2);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Test_Equals_Object_SameIdAndGeneration_ReturnsTrue()
    {
        var id1 = new PdfObjectId(42, 1);
        object id2 = new PdfObjectId(42, 1);
        
        Assert.True(id1.Equals(id2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var objectId = new PdfObjectId(42, 1);
        object obj = "not an object id";
        
        Assert.False(objectId.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var objectId = new PdfObjectId(42, 1);
        object? obj = null;
        
        Assert.False(objectId.Equals(obj));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(42, 1)]
    [InlineData(123, 5)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void Test_GetHashCode_SameIdAndGeneration_ReturnsSameHash(int objectNumber, int generation)
    {
        var id1 = new PdfObjectId(objectNumber, generation);
        var id2 = new PdfObjectId(objectNumber, generation);
        
        Assert.Equal(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentId_ReturnsDifferentHash()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(43, 1);
        
        Assert.NotEqual(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentGeneration_ReturnsDifferentHash()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(42, 2);
        
        Assert.NotEqual(id1.GetHashCode(), id2.GetHashCode());
    }

    [Fact]
    public void Test_DefaultConstructor_SetsZeroValues()
    {
        var objectId = new PdfObjectId();
        
        Assert.Equal(0, objectId.ObjectNumber);
        Assert.Equal(0, objectId.Generation);
    }

    [Fact]
    public void Test_EqualityOperators_Reflexive()
    {
        var objectId = new PdfObjectId(42, 1);
        var sameObjectId = objectId;
        
        Assert.True(objectId == sameObjectId);
        Assert.False(objectId != sameObjectId);
        Assert.True(objectId.Equals(sameObjectId));
    }

    [Fact]
    public void Test_EqualityOperators_Symmetric()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(42, 1);
        
        Assert.True(id1 == id2);
        Assert.True(id2 == id1);
        Assert.True(id1.Equals(id2));
        Assert.True(id2.Equals(id1));
    }

    [Fact]
    public void Test_EqualityOperators_Transitive()
    {
        var id1 = new PdfObjectId(42, 1);
        var id2 = new PdfObjectId(42, 1);
        var id3 = new PdfObjectId(42, 1);
        
        Assert.True(id1 == id2);
        Assert.True(id2 == id3);
        Assert.True(id1 == id3);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(1, 1)]
    [InlineData(42, 1)]
    [InlineData(123, 456)]
    public void Test_PropertyInitializers_Work(int objectNumber, int generation)
    {
        var objectId = new PdfObjectId(42, 0) { ObjectNumber = objectNumber, Generation = generation };
        
        Assert.Equal(objectNumber, objectId.ObjectNumber);
        Assert.Equal(generation, objectId.Generation);
    }

    [Fact]
    public void Test_HashCodeConsistency()
    {
        var objectId = new PdfObjectId(42, 1);
        
        var hash1 = objectId.GetHashCode();
        var hash2 = objectId.GetHashCode();
        
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Test_BoundaryValues_MinValid()
    {
        var objectId = new PdfObjectId(1, 0);
        
        Assert.Equal(1, objectId.ObjectNumber);
        Assert.Equal(0, objectId.Generation);
    }

    [Fact]
    public void Test_BoundaryValues_MaxValue()
    {
        var objectId = new PdfObjectId(int.MaxValue, int.MaxValue);
        
        Assert.Equal(int.MaxValue, objectId.ObjectNumber);
        Assert.Equal(int.MaxValue, objectId.Generation);
    }

    [Fact]
    public void Test_MultipleObjectIds_UniqueHashCodes()
    {
        var objectIds = new[]
        {
            new PdfObjectId(1, 0),
            new PdfObjectId(2, 0),
            new PdfObjectId(1, 1),
            new PdfObjectId(2, 1),
            new PdfObjectId(42, 1),
            new PdfObjectId(1, 42)
        };
        
        var hashCodes = objectIds.Select(id => id.GetHashCode()).ToArray();
        var uniqueHashCodes = hashCodes.Distinct().Count();
        
        Assert.Equal(objectIds.Length, uniqueHashCodes);
    }

    [Fact]
    public void Test_ObjectId_InCollections()
    {
        var objectIds = new HashSet<PdfObjectId>
        {
            new PdfObjectId(1, 0),
            new PdfObjectId(2, 0),
            new PdfObjectId(1, 1),
            new PdfObjectId(1, 0) // Duplicate
        };
        
        Assert.Equal(3, objectIds.Count);
        Assert.Contains(new PdfObjectId(1, 0), objectIds);
        Assert.Contains(new PdfObjectId(2, 0), objectIds);
        Assert.Contains(new PdfObjectId(1, 1), objectIds);
    }
}
