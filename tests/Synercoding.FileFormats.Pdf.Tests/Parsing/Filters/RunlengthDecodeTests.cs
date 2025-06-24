using Synercoding.FileFormats.Pdf.Parsing.Filters;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;

public class RunlengthDecodeTests
{
    [Theory]
    [InlineData(new byte[] { 2, 0x41, 0x42, 0x43, 128 }, new byte[] { 0x41, 0x42, 0x43 })] // Copy 3 bytes literal
    [InlineData(new byte[] { 0, 0x41, 128 }, new byte[] { 0x41 })] // Copy 1 byte literal
    [InlineData(new byte[] { 254, 0x41, 128 }, new byte[] { 0x41, 0x41, 0x41 })] // Repeat 0x41 three times (257-254=3)
    [InlineData(new byte[] { 129, 0x42, 128 }, new byte[] { 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42 })] // Repeat 0x42 128 times (257-129=128)
    [InlineData(new byte[] { 128 }, new byte[] { })] // EOD marker only - empty output
    public void Test_Decode_ValidInput(byte[] input, byte[] expected)
    {
        var filter = new RunLengthDecode();

        var result = filter.Decode(input, null, null!);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Decode_MixedLiteralAndRepeated()
    {
        // 2 literal bytes (0x41, 0x42, 0x43), then repeat 0x44 five times, then EOD
        var input = new byte[] { 2, 0x41, 0x42, 0x43, 252, 0x44, 128 };
        var expected = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x44, 0x44, 0x44, 0x44 }; // 257-252=5 repeats

        var filter = new RunLengthDecode();
        var result = filter.Decode(input, null, null!);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Decode_MaximumLiteralRun()
    {
        // Create input with maximum literal run (128 bytes)
        var literalData = new byte[128];
        for (int i = 0; i < 128; i++)
            literalData[i] = (byte)(i % 256);

        var input = new byte[130]; // 127 (length) + 128 (data) + 1 (EOD)
        input[0] = 127; // 127 means copy 128 bytes (127+1)
        Array.Copy(literalData, 0, input, 1, 128);
        input[129] = 128; // EOD

        var filter = new RunLengthDecode();
        var result = filter.Decode(input, null, null!);

        Assert.Equal(literalData, result);
    }

    [Fact]
    public void Test_Decode_MaximumRepeatedRun()
    {
        // 129 means repeat next byte 128 times (257-129=128)
        var input = new byte[] { 129, 0xFF, 128 };
        var expected = new byte[128];
        Array.Fill(expected, (byte)0xFF);

        var filter = new RunLengthDecode();
        var result = filter.Decode(input, null, null!);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Decode_EmptyInput()
    {
        var filter = new RunLengthDecode();
        var input = Array.Empty<byte>();

        var result = filter.Decode(input, null, null!);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_Decode_WithoutEODMarker()
    {
        // Should still work correctly when input ends without explicit EOD marker
        var input = new byte[] { 1, 0x41, 0x42 }; // Copy 2 bytes literal, no EOD
        var expected = new byte[] { 0x41, 0x42 };

        var filter = new RunLengthDecode();
        var result = filter.Decode(input, null, null!);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x41, 0x42, 0x43 }, new byte[] { 2, 0x41, 0x42, 0x43, 128 })] // 3 literal bytes
    [InlineData(new byte[] { 0x41, 0x41, 0x41 }, new byte[] { 254, 0x41, 128 })] // 3 repeated bytes (257-3=254)
    [InlineData(new byte[] { 0x41 }, new byte[] { 0, 0x41, 128 })] // Single literal byte
    [InlineData(new byte[] { }, new byte[] { 128 })] // Empty input
    public void Test_Encode_BasicCases(byte[] input, byte[] expected)
    {
        var filter = new RunLengthDecode();

        var result = filter.Encode(input, null);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Encode_LongRepeatedRun()
    {
        var filter = new RunLengthDecode();
        var input = new byte[100];
        Array.Fill(input, (byte)0x42);

        var result = filter.Encode(input, null);

        // Should encode as 257-100=157, 0x42, EOD
        var expected = new byte[] { 157, 0x42, 128 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Encode_MaximumRepeatedRun()
    {
        var filter = new RunLengthDecode();
        var input = new byte[128];
        Array.Fill(input, (byte)0xFF);

        var result = filter.Encode(input, null);

        // Should encode as 257-128=129, 0xFF, EOD
        var expected = new byte[] { 129, 0xFF, 128 };
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Encode_LongLiteralRun()
    {
        var filter = new RunLengthDecode();
        var input = new byte[100];
        for (int i = 0; i < 100; i++)
            input[i] = (byte)(i % 256);

        var result = filter.Encode(input, null);

        // Should encode as 99 (length-1), followed by 100 bytes, then EOD
        Assert.Equal(99, result[0]); // 100-1=99
        Assert.Equal(128, result[result.Length - 1]); // EOD marker
        Assert.Equal(102, result.Length); // 1 + 100 + 1
        
        // Verify the literal data is preserved
        for (int i = 0; i < 100; i++)
            Assert.Equal(input[i], result[i + 1]);
    }

    [Fact]
    public void Test_Encode_MixedLiteralAndRepeated()
    {
        var filter = new RunLengthDecode();
        // ABC followed by 5 D's
        var input = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x44, 0x44, 0x44, 0x44 };

        var result = filter.Encode(input, null);

        // Should encode as: 2 (literal length-1), A, B, C, 252 (257-5), D, EOD
        var expected = new byte[] { 2, 0x41, 0x42, 0x43, 252, 0x44, 128 };
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x41, 0x41 })] // 2 repeated bytes
    [InlineData(new byte[] { 0x42, 0x42, 0x42, 0x42, 0x42 })] // 5 repeated bytes
    [InlineData(new byte[] { 0x41, 0x42, 0x43, 0x44 })] // 4 literal bytes
    [InlineData(new byte[] { 0x41, 0x41, 0x42, 0x43 })] // Mixed: 2 repeated + 2 literal
    public void Test_Encode_Then_Decode_RoundTrip(byte[] original)
    {
        var filter = new RunLengthDecode();

        var encoded = filter.Encode(original, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Test_Filter_Name()
    {
        var filter = new RunLengthDecode();
        
        Assert.Equal("RunLengthDecode", filter.Name.Display);
    }

    [Theory]
    [InlineData(new byte[] { 255, 0x20 }, new byte[] { 0x20, 0x20 })] // Repeat 0x20 two times (257-255=2)
    [InlineData(new byte[] { 250, 0x30 }, new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 })] // Repeat 0x30 seven times (257-250=7)
    public void Test_Decode_VariousRepeatCounts(byte[] input, byte[] expected)
    {
        var filter = new RunLengthDecode();

        var result = filter.Decode(input, null, null!);

        Assert.Equal(expected, result);
    }
}
