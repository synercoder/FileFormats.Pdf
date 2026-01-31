using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfNumberTests
{
    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(42L)]
    [InlineData(-42L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Test_Constructor_Long_SetsValue(long value)
    {
        var pdfNumber = new PdfNumber(value);

        Assert.Equal(value, pdfNumber.Value);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    [InlineData(3.14159)]
    [InlineData(-3.14159)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.Epsilon)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Test_Constructor_Double_SetsValue(double value)
    {
        var pdfNumber = new PdfNumber(value);

        Assert.Equal(value, pdfNumber.Value);
    }

    [Fact]
    public void Test_Constructor_NaN_SetsValue()
    {
        var pdfNumber = new PdfNumber(double.NaN);

        Assert.True(double.IsNaN(pdfNumber.Value));
    }

    [Fact]
    public void Test_Equals_PdfNumber_SameIntegerValue_ReturnsTrue()
    {
        var num1 = new PdfNumber(42);
        var num2 = new PdfNumber(42);

        Assert.True(num1.Equals(num2));
        Assert.True(num1 == num2);
        Assert.False(num1 != num2);
    }

    [Fact]
    public void Test_Equals_PdfNumber_SameRealValue_ReturnsTrue()
    {
        var num1 = new PdfNumber(3.14159);
        var num2 = new PdfNumber(3.14159);

        Assert.True(num1.Equals(num2));
        Assert.True(num1 == num2);
        Assert.False(num1 != num2);
    }

    [Fact]
    public void Test_Equals_PdfNumber_DifferentIntegerValue_ReturnsFalse()
    {
        var num1 = new PdfNumber(42);
        var num2 = new PdfNumber(24);

        Assert.False(num1.Equals(num2));
        Assert.False(num1 == num2);
        Assert.True(num1 != num2);
    }

    [Fact]
    public void Test_Equals_PdfNumber_DifferentRealValue_ReturnsFalse()
    {
        var num1 = new PdfNumber(3.14159);
        var num2 = new PdfNumber(2.71828);

        Assert.False(num1.Equals(num2));
        Assert.False(num1 == num2);
        Assert.True(num1 != num2);
    }

    [Fact]
    public void Test_Equals_CrossType_IntegerAndReal_SameValue_ReturnsTrue()
    {
        var pdfInt = new PdfNumber(42);
        var pdfReal = new PdfNumber(42.0);

        Assert.True(pdfInt.Equals(pdfReal));
        Assert.True(pdfInt == pdfReal);
        Assert.False(pdfInt != pdfReal);
    }

    [Fact]
    public void Test_Equals_CrossType_IntegerAndReal_DifferentValue_ReturnsFalse()
    {
        var pdfInt = new PdfNumber(42);
        var pdfReal = new PdfNumber(24.0);

        Assert.False(pdfInt.Equals(pdfReal));
        Assert.False(pdfInt == pdfReal);
        Assert.True(pdfInt != pdfReal);
    }

    [Fact]
    public void Test_Equals_CrossType_IntegerAndReal_DecimalValue_ReturnsFalse()
    {
        var pdfInt = new PdfNumber(42);
        var pdfReal = new PdfNumber(42.5);

        Assert.False(pdfInt.Equals(pdfReal));
        Assert.False(pdfInt == pdfReal);
        Assert.True(pdfInt != pdfReal);
    }

    [Fact]
    public void Test_Equals_Object_SameIntegerValue_ReturnsTrue()
    {
        var num1 = new PdfNumber(42);
        object num2 = new PdfNumber(42);

        Assert.True(num1.Equals(num2));
    }

    [Fact]
    public void Test_Equals_Object_SameRealValue_ReturnsTrue()
    {
        var num1 = new PdfNumber(3.14159);
        object num2 = new PdfNumber(3.14159);

        Assert.True(num1.Equals(num2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var pdfNumber = new PdfNumber(42);
        object obj = "not a number";

        Assert.False(pdfNumber.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var pdfNumber = new PdfNumber(42);
        object? obj = null;

        Assert.False(pdfNumber.Equals(obj));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(42L)]
    [InlineData(-42L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Test_GetHashCode_SameLongValue_ReturnsSameHash(long value)
    {
        var num1 = new PdfNumber(value);
        var num2 = new PdfNumber(value);

        Assert.Equal(num1.GetHashCode(), num2.GetHashCode());
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(3.14159)]
    [InlineData(-3.14159)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public void Test_GetHashCode_SameDoubleValue_ReturnsSameHash(double value)
    {
        var num1 = new PdfNumber(value);
        var num2 = new PdfNumber(value);

        Assert.Equal(num1.GetHashCode(), num2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentIntegerValue_ReturnsDifferentHash()
    {
        var num1 = new PdfNumber(42);
        var num2 = new PdfNumber(24);

        Assert.NotEqual(num1.GetHashCode(), num2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentRealValue_ReturnsDifferentHash()
    {
        var num1 = new PdfNumber(3.14159);
        var num2 = new PdfNumber(2.71828);

        Assert.NotEqual(num1.GetHashCode(), num2.GetHashCode());
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(42L)]
    [InlineData(-42L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Test_ImplicitConversion_ToLong(long value)
    {
        var pdfNumber = new PdfNumber(value);

        long result = pdfNumber;

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-42)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Test_ImplicitConversion_ToInt(int value)
    {
        var pdfNumber = new PdfNumber(value);

        int result = pdfNumber;

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(255)]
    public void Test_ImplicitConversion_ToByte(byte value)
    {
        var pdfNumber = new PdfNumber(value);

        byte result = (byte)pdfNumber;

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(3.14159)]
    [InlineData(-3.14159)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public void Test_ImplicitConversion_ToDouble(double value)
    {
        var pdfNumber = new PdfNumber(value);

        double result = pdfNumber;

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(3.14159f)]
    [InlineData(-3.14159f)]
    [InlineData(float.MaxValue)]
    [InlineData(float.MinValue)]
    public void Test_ImplicitConversion_ToFloat(float value)
    {
        var pdfNumber = new PdfNumber(value);

        float result = pdfNumber;

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(42L)]
    [InlineData(-42L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Test_ExplicitConversion_FromLong(long value)
    {
        var pdfNumber = (PdfNumber)value;

        Assert.Equal(value, pdfNumber.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-42)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Test_ExplicitConversion_FromInt(int value)
    {
        var pdfNumber = (PdfNumber)value;

        Assert.Equal(value, pdfNumber.Value);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(3.14159)]
    [InlineData(-3.14159)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public void Test_ExplicitConversion_FromDouble(double value)
    {
        var pdfNumber = (PdfNumber)value;

        Assert.Equal(value, pdfNumber.Value);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(3.14159f)]
    [InlineData(-3.14159f)]
    [InlineData(float.MaxValue)]
    [InlineData(float.MinValue)]
    public void Test_ExplicitConversion_FromFloat(float value)
    {
        var pdfNumber = (PdfNumber)value;

        Assert.Equal(value, pdfNumber.Value, precision: 6);
    }

    [Fact]
    public void Test_EqualityOperators_WithLargeIntegerValues()
    {
        var num1 = new PdfNumber(long.MaxValue);
        var num2 = new PdfNumber(long.MaxValue);
        var num3 = new PdfNumber(long.MinValue);

        Assert.True(num1 == num2);
        Assert.False(num1 == num3);
        Assert.False(num1 != num2);
        Assert.True(num1 != num3);
    }

    [Fact]
    public void Test_EqualityOperators_WithSpecialRealValues()
    {
        var num1 = new PdfNumber(double.PositiveInfinity);
        var num2 = new PdfNumber(double.PositiveInfinity);
        var num3 = new PdfNumber(double.NegativeInfinity);

        Assert.True(num1 == num2);
        Assert.False(num1 == num3);
        Assert.False(num1 != num2);
        Assert.True(num1 != num3);
    }

    [Fact]
    public void Test_EqualityOperators_WithNaN()
    {
        var num1 = new PdfNumber(double.NaN);
        var num2 = new PdfNumber(double.NaN);

        Assert.False(num1 == num2);
        Assert.True(num1 != num2);
    }

    [Fact]
    public void Test_CrossTypeEquality_IntegerWithReal()
    {
        var pdfInt = new PdfNumber(100);
        var pdfReal = new PdfNumber(100.0);
        var pdfRealDecimal = new PdfNumber(100.5);

        Assert.True(pdfInt == pdfReal);
        Assert.False(pdfInt == pdfRealDecimal);
        Assert.False(pdfInt != pdfReal);
        Assert.True(pdfInt != pdfRealDecimal);
    }

    [Theory]
    [InlineData(0, 0.0)]
    [InlineData(42, 42.0)]
    [InlineData(-42, -42.0)]
    [InlineData(1000000, 1000000.0)]
    public void Test_CrossTypeEquality_ExactValues(long intValue, double realValue)
    {
        var pdfInt = new PdfNumber(intValue);
        var pdfReal = new PdfNumber(realValue);

        Assert.True(pdfInt.Equals(pdfReal));
        Assert.True(pdfInt == pdfReal);
    }

    [Fact]
    public void Test_BoundaryValues_IntegerMaxMinValues()
    {
        var maxInt = new PdfNumber(long.MaxValue);
        var minInt = new PdfNumber(long.MinValue);

        Assert.Equal(long.MaxValue, maxInt.Value);
        Assert.Equal(long.MinValue, minInt.Value);
        Assert.NotEqual(maxInt, minInt);
    }

    [Fact]
    public void Test_BoundaryValues_RealMaxMinValues()
    {
        var maxReal = new PdfNumber(double.MaxValue);
        var minReal = new PdfNumber(double.MinValue);
        var epsilon = new PdfNumber(double.Epsilon);

        Assert.Equal(double.MaxValue, maxReal.Value);
        Assert.Equal(double.MinValue, minReal.Value);
        Assert.Equal(double.Epsilon, epsilon.Value);
        Assert.NotEqual(maxReal, minReal);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(0.01)]
    [InlineData(0.001)]
    [InlineData(0.0001)]
    public void Test_SmallDecimalValues_PreservePrecision(double value)
    {
        var pdfNumber = new PdfNumber(value);

        Assert.Equal(value, pdfNumber.Value, precision: 15);
    }

    [Theory]
    [InlineData(1e10)]
    [InlineData(1e-10)]
    [InlineData(1.23456789e15)]
    [InlineData(9.87654321e-15)]
    public void Test_ScientificNotation_Values(double value)
    {
        var pdfNumber = new PdfNumber(value);

        Assert.Equal(value, pdfNumber.Value);
    }
}