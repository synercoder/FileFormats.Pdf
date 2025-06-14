using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfBooleanTests
{
    [Fact]
    public void Test_True_Constant_HasTrueValue()
    {
        Assert.True(PdfBoolean.True.Value);
    }

    [Fact]
    public void Test_False_Constant_HasFalseValue()
    {
        Assert.False(PdfBoolean.False.Value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Constructor_SetsValue(bool value)
    {
        var pdfBool = new PdfBoolean(value);
        
        Assert.Equal(value, pdfBool.Value);
    }

    [Fact]
    public void Test_Equals_SameValue_ReturnsTrue()
    {
        var bool1 = new PdfBoolean(true);
        var bool2 = new PdfBoolean(true);
        
        Assert.True(bool1.Equals(bool2));
        Assert.True(bool1 == bool2);
        Assert.False(bool1 != bool2);
    }

    [Fact]
    public void Test_Equals_DifferentValue_ReturnsFalse()
    {
        var bool1 = new PdfBoolean(true);
        var bool2 = new PdfBoolean(false);
        
        Assert.False(bool1.Equals(bool2));
        Assert.False(bool1 == bool2);
        Assert.True(bool1 != bool2);
    }

    [Fact]
    public void Test_Equals_Object_SameValue_ReturnsTrue()
    {
        var bool1 = new PdfBoolean(true);
        object bool2 = new PdfBoolean(true);
        
        Assert.True(bool1.Equals(bool2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var pdfBool = new PdfBoolean(true);
        object obj = "not a boolean";
        
        Assert.False(pdfBool.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var pdfBool = new PdfBoolean(true);
        object? obj = null;
        
        Assert.False(pdfBool.Equals(obj));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_GetHashCode_SameValue_ReturnsSameHash(bool value)
    {
        var bool1 = new PdfBoolean(value);
        var bool2 = new PdfBoolean(value);
        
        Assert.Equal(bool1.GetHashCode(), bool2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        var bool1 = new PdfBoolean(true);
        var bool2 = new PdfBoolean(false);
        
        Assert.NotEqual(bool1.GetHashCode(), bool2.GetHashCode());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ImplicitConversion_ToBool(bool value)
    {
        var pdfBool = new PdfBoolean(value);
        
        bool result = pdfBool;
        
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_ExplicitConversion_FromBool(bool value)
    {
        var pdfBool = (PdfBoolean)value;
        
        Assert.Equal(value, pdfBool.Value);
    }

    [Fact]
    public void Test_Constants_AreDifferent()
    {
        Assert.NotEqual(PdfBoolean.True, PdfBoolean.False);
        Assert.True(PdfBoolean.True != PdfBoolean.False);
    }

    [Fact]
    public void Test_Constants_AreCorrectValues()
    {
        Assert.True(PdfBoolean.True.Value);
        Assert.False(PdfBoolean.False.Value);
    }

    [Fact]
    public void Test_EqualityOperators_WithConstants()
    {
        var trueValue = new PdfBoolean(true);
        var falseValue = new PdfBoolean(false);
        
        Assert.True(trueValue == PdfBoolean.True);
        Assert.True(falseValue == PdfBoolean.False);
        Assert.False(trueValue == PdfBoolean.False);
        Assert.False(falseValue == PdfBoolean.True);
    }
}
