using System.Buffers;

namespace Synercoding.FileFormats.Pdf.Encryption;

internal static class RC4
{
    public static byte[] Process(byte[] key, byte[] data)
    {
        _ = key ?? throw new ArgumentNullException(nameof(key));
        _ = data ?? throw new ArgumentNullException(nameof(data));

        // Rent a byte array
        var sBox = ArrayPool<byte>.Shared.Rent(256);

        _initializeSBoxArray(key, sBox);
        int i = 0;
        int j = 0;

        byte[] result = new byte[data.Length];

        for (int k = 0; k < data.Length; k++)
        {
            // Pseudo-random generation algorithm (PRGA)
            i = ( i + 1 ) % 256;
            j = ( j + sBox[i] ) % 256;
            _swap(ref sBox[i], ref sBox[j]);

            byte keyStreamByte = sBox[( sBox[i] + sBox[j] ) % 256];
            result[k] = (byte)( data[k] ^ keyStreamByte );
        }

        // Return the byte array
        ArrayPool<byte>.Shared.Return(sBox);

        return result;
    }

    private static void _initializeSBoxArray(byte[] key, byte[] sBox)
    {
        if (sBox.Length < 256)
            throw new ArgumentException("", nameof(sBox));

        // Initialize S-box
        for (int i = 0; i < 256; i++)
            sBox[i] = (byte)i;

        // Key scheduling algorithm (KSA)
        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = ( j + sBox[i] + key[i % key.Length] ) % 256;
            _swap(ref sBox[i], ref sBox[j]);
        }
    }

    private static void _swap(ref byte a, ref byte b)
    {
        byte temp = a;
        a = b;
        b = temp;
    }
}
