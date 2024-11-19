using Synercoding.FileFormats.Pdf.Parsing.Filters;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;
public class ASCIIHexDecodeTests
{
    [Theory]
    [InlineData("A0   B3 0F9A>", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A })]
    [InlineData("A0   B3 0F9AA>", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9AA    >", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9A", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A })]
    [InlineData("A0   B3 0F9AA", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9AA    ", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    public void Test_If_Decode_Correct(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();

        var inputBytes = Encoding.UTF8.GetBytes(input);

        var decode = filter.Decode(inputBytes, null);

        Assert.Equal(expected, decode);
    }

    [Theory]
    [InlineData(new byte[] { 0xA0, 0xB3, 0x0F, 0x9A }, "A0B30F9A>")]
    [InlineData(new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 }, "A0B30F9AA0>")]
    public void Test_If_Encode_Correct(byte[] input, string expected)
    {
        var filter = new ASCIIHexDecode();

        var decode = filter.Encode(input, null);

        Assert.Equal(expected, Encoding.ASCII.GetString(decode));
    }
}
