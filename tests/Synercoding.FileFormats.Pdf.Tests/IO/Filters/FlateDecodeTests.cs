using Synercoding.FileFormats.Pdf.IO.Filters;
using System.Buffers.Binary;
using System.IO.Compression;

namespace Synercoding.FileFormats.Pdf.Tests.IO.Filters;

public class FlateDecodeTests
{
    [Fact]
    public void Test_Encode_WritesZlibHeaderAndAdler32Trailer()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("Wikipedia");

        var encoded = new FlateDecode().Encode(input, null);

        // zlib header: deflate method with 32K window, and a header checksum that is a multiple of 31
        Assert.Equal(0x78, encoded[0]);
        Assert.Equal(0, ( ( encoded[0] << 8 ) | encoded[1] ) % 31);

        // Adler-32 of "Wikipedia" is 0x11E60398, stored big-endian at the end of the stream
        Assert.Equal(0x11E60398u, BinaryPrimitives.ReadUInt32BigEndian(encoded.AsSpan(encoded.Length - 4)));
    }

    [Fact]
    public void Test_Encode_RoundTripsThroughZlib()
    {
        var input = new byte[10_000];
        new Random(42).NextBytes(input);

        var encoded = new FlateDecode().Encode(input, null);

        using var decompressed = new MemoryStream();
        using (var zlib = new ZLibStream(new MemoryStream(encoded), CompressionMode.Decompress))
            zlib.CopyTo(decompressed);

        Assert.Equal(input, decompressed.ToArray());
    }
}
