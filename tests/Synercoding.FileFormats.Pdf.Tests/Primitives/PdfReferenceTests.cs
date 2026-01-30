using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfReferenceTests
{
    [Fact]
    public void Test_Id_Property_ReturnsCorrectValue()
    {
        var objectId = new PdfObjectId(42, 0);
        var reference = new PdfReference { Id = objectId };

        Assert.Equal(objectId, reference.Id);
        Assert.Equal(42, reference.Id.ObjectNumber);
        Assert.Equal(0, reference.Id.Generation);
    }

    [Fact]
    public void Test_Equals_SameReference_ReturnsTrue()
    {
        var objectId = new PdfObjectId(42, 1);
        var ref1 = new PdfReference { Id = objectId };
        var ref2 = new PdfReference { Id = objectId };

        Assert.True(ref1.Equals(ref2));
        Assert.True(ref1 == ref2);
        Assert.False(ref1 != ref2);
    }

    [Fact]
    public void Test_Equals_DifferentId_ReturnsFalse()
    {
        var objectId1 = new PdfObjectId(42, 1);
        var objectId2 = new PdfObjectId(43, 1);
        var ref1 = new PdfReference { Id = objectId1 };
        var ref2 = new PdfReference { Id = objectId2 };

        Assert.False(ref1.Equals(ref2));
        Assert.False(ref1 == ref2);
        Assert.True(ref1 != ref2);
    }

    [Fact]
    public void Test_Equals_DifferentGeneration_ReturnsFalse()
    {
        var objectId1 = new PdfObjectId(42, 1);
        var objectId2 = new PdfObjectId(42, 2);
        var ref1 = new PdfReference { Id = objectId1 };
        var ref2 = new PdfReference { Id = objectId2 };

        Assert.False(ref1.Equals(ref2));
        Assert.False(ref1 == ref2);
        Assert.True(ref1 != ref2);
    }

    [Fact]
    public void Test_Equals_Object_SameReference_ReturnsTrue()
    {
        var objectId = new PdfObjectId(42, 1);
        var ref1 = new PdfReference { Id = objectId };
        object ref2 = new PdfReference { Id = objectId };

        Assert.True(ref1.Equals(ref2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var objectId = new PdfObjectId(42, 1);
        var reference = new PdfReference { Id = objectId };
        object obj = "not a reference";

        Assert.False(reference.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var objectId = new PdfObjectId(42, 1);
        var reference = new PdfReference { Id = objectId };
        object? obj = null;

        Assert.False(reference.Equals(obj));
    }

    [Fact]
    public void Test_GetHashCode_SameReference_ReturnsSameHash()
    {
        var objectId = new PdfObjectId(42, 1);
        var ref1 = new PdfReference { Id = objectId };
        var ref2 = new PdfReference { Id = objectId };

        Assert.Equal(ref1.GetHashCode(), ref2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentId_ReturnsDifferentHash()
    {
        var objectId1 = new PdfObjectId(42, 1);
        var objectId2 = new PdfObjectId(43, 1);
        var ref1 = new PdfReference { Id = objectId1 };
        var ref2 = new PdfReference { Id = objectId2 };

        Assert.NotEqual(ref1.GetHashCode(), ref2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentGeneration_ReturnsDifferentHash()
    {
        var objectId1 = new PdfObjectId(42, 1);
        var objectId2 = new PdfObjectId(42, 2);
        var ref1 = new PdfReference { Id = objectId1 };
        var ref2 = new PdfReference { Id = objectId2 };

        Assert.NotEqual(ref1.GetHashCode(), ref2.GetHashCode());
    }

    [Fact]
    public void Test_DefaultReference_HasDefaultObjectId()
    {
        var reference = new PdfReference();

        Assert.Equal(0, reference.Id.ObjectNumber);
        Assert.Equal(0, reference.Id.Generation);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(42, 1)]
    [InlineData(999999, 99)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void Test_Reference_WithVariousObjectIds(int objectNumber, int generation)
    {
        var objectId = new PdfObjectId(objectNumber, generation);
        var reference = new PdfReference { Id = objectId };

        Assert.Equal(objectNumber, reference.Id.ObjectNumber);
        Assert.Equal(generation, reference.Id.Generation);
    }

    [Fact]
    public void Test_EqualityOperators_MultipleReferences()
    {
        var objectId1 = new PdfObjectId(42, 1);
        var objectId2 = new PdfObjectId(42, 1);
        var objectId3 = new PdfObjectId(43, 1);

        var ref1 = new PdfReference { Id = objectId1 };
        var ref2 = new PdfReference { Id = objectId2 };
        var ref3 = new PdfReference { Id = objectId3 };

        Assert.True(ref1 == ref2);
        Assert.False(ref1 == ref3);
        Assert.False(ref1 != ref2);
        Assert.True(ref1 != ref3);
    }

    [Fact]
    public void Test_Reference_WithMaxValues()
    {
        var objectId = new PdfObjectId(int.MaxValue, int.MaxValue);
        var reference = new PdfReference { Id = objectId };

        Assert.Equal(int.MaxValue, reference.Id.ObjectNumber);
        Assert.Equal(int.MaxValue, reference.Id.Generation);
    }

    [Fact]
    public void Test_Reference_WithMinValidValues()
    {
        var objectId = new PdfObjectId(1, 0);
        var reference = new PdfReference { Id = objectId };

        Assert.Equal(1, reference.Id.ObjectNumber);
        Assert.Equal(0, reference.Id.Generation);
    }

    [Fact]
    public void Test_Reference_HashCodeConsistency()
    {
        var objectId = new PdfObjectId(42, 1);
        var reference = new PdfReference { Id = objectId };

        var hash1 = reference.GetHashCode();
        var hash2 = reference.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Test_Reference_EqualsConsistency()
    {
        var objectId = new PdfObjectId(42, 1);
        var ref1 = new PdfReference { Id = objectId };
        var ref2 = new PdfReference { Id = objectId };

        Assert.True(ref1.Equals(ref2));
        Assert.True(ref2.Equals(ref1));
        Assert.True(ref1.Equals(ref1));
    }
}
