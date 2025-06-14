using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Tests.IO;

public class PdfByteArrayProviderTests
{
    [Fact]
    public void Constructor_WithNullBytes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PdfByteArrayProvider(null!));
    }

    [Fact]
    public void Constructor_WithEmptyBytes_CreatesValidProvider()
    {
        var bytes = Array.Empty<byte>();
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Equal(0, provider.Length);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void Constructor_WithBytes_SetsLengthCorrectly()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Equal(5, provider.Length);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void Position_SetToNegative_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Position = -1);
    }

    [Fact]
    public void Position_SetToLength_IsAllowed()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        // Setting position to exactly Length should be allowed (EOF position)
        provider.Position = 3;
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void Position_SetToGreaterThanLength_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Position = 10);
    }

    [Fact]
    public void Position_SetToValidValue_UpdatesPosition()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        provider.Position = 1;
        Assert.Equal(1, provider.Position);

        provider.Position = 2;
        Assert.Equal(2, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_AtBeginning_ReturnsCorrectByte()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43 };
        var provider = new PdfByteArrayProvider(bytes);

        var success = provider.TryRead(out byte result);

        Assert.True(success);
        Assert.Equal(0x41, result);
        Assert.Equal(1, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_AtEnd_ReturnsFalse()
    {
        var bytes = new byte[] { 0x41 };
        var provider = new PdfByteArrayProvider(bytes);
        provider.Position = 1; // Set to EOF position

        var success = provider.TryRead(out byte result);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.Equal(1, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_EmptyArray_ReturnsFalse()
    {
        var bytes = Array.Empty<byte>();
        var provider = new PdfByteArrayProvider(bytes);

        var success = provider.TryRead(out byte result);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_ValidRead_ReturnsTrue()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
        var provider = new PdfByteArrayProvider(bytes);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 0, 3);

        Assert.True(success);
        Assert.Equal(new byte[] { 0x41, 0x42, 0x43, 0, 0, 0, 0, 0, 0, 0 }, buffer);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_WithOffset_ReadsCorrectly()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
        var provider = new PdfByteArrayProvider(bytes);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 2, 3);

        Assert.True(success);
        Assert.Equal(new byte[] { 0, 0, 0x41, 0x42, 0x43, 0, 0, 0, 0, 0 }, buffer);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_NotEnoughDataInSource_ReturnsFalse()
    {
        var bytes = new byte[] { 0x41, 0x42 };
        var provider = new PdfByteArrayProvider(bytes);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 0, 5);

        Assert.False(success);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_NotEnoughSpaceInBuffer_ReturnsFalse()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
        var provider = new PdfByteArrayProvider(bytes);
        var buffer = new byte[3];

        var success = provider.TryRead(buffer, 2, 3);

        Assert.False(success);
        Assert.Equal(0, provider.Position);
    }

    [Theory]
    [InlineData(SeekOrigin.Begin, 0, 0)]
    [InlineData(SeekOrigin.Begin, 2, 2)]
    [InlineData(SeekOrigin.Current, 1, 2)]
    [InlineData(SeekOrigin.Current, -1, 0)]
    [InlineData(SeekOrigin.End, -1, 4)]
    [InlineData(SeekOrigin.End, -2, 3)]
    public void Seek_ValidOperations_ReturnsCorrectPosition(SeekOrigin origin, long offset, long expectedPosition)
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var provider = new PdfByteArrayProvider(bytes);

        if (origin == SeekOrigin.Current)
            provider.Position = expectedPosition - offset;

        var result = provider.Seek(offset, origin);

        Assert.Equal(expectedPosition, result);
        Assert.Equal(expectedPosition, provider.Position);
    }

    [Fact]
    public void Seek_InvalidOrigin_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Seek(0, (SeekOrigin)999));
    }

    [Fact]
    public void Seek_BeginWithNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Seek(-1, SeekOrigin.Begin));
    }

    [Fact]
    public void Seek_BeginWithOffsetBeyondLength_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Seek(5, SeekOrigin.Begin));
    }

    [Fact]
    public void Seek_EndWithPositiveOffset_ThrowsArgumentOutOfRangeException()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var provider = new PdfByteArrayProvider(bytes);

        Assert.Throws<ArgumentOutOfRangeException>(() => provider.Seek(1, SeekOrigin.End));
    }

    [Fact]
    public void SequentialRead_ReadAllBytes_WorksCorrectly()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43 };
        var provider = new PdfByteArrayProvider(bytes);
        var results = new List<byte>();

        while (provider.TryRead(out byte b))
            results.Add(b);

        Assert.Equal(bytes, results.ToArray());
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void LargeArray_WorksCorrectly()
    {
        var bytes = new byte[10_000];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(i % 256);

        var provider = new PdfByteArrayProvider(bytes);

        Assert.Equal(10_000, provider.Length);

        provider.Position = 5000;
        Assert.True(provider.TryRead(out byte result));
        Assert.Equal((byte)(5000 % 256), result);
    }
}
