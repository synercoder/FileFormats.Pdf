namespace Synercoding.FileFormats.Pdf.IO;

/// <summary>
/// Provides utility methods for calculating the number of bytes needed to represent numeric values as strings.
/// </summary>
internal static class ByteSizes
{
    /// <summary>
    /// Calculates the number of bytes required to represent a signed 32-bit integer as a decimal string.
    /// </summary>
    /// <param name="integer">The integer value to calculate the size for.</param>
    /// <returns>The number of bytes needed to represent the integer as a decimal string, including the negative sign if applicable.</returns>
    public static int Size(int integer)
    {
        if (integer == 0)
            return 1;

        if (integer < 0)
        {
            if (integer == int.MinValue)
                return 11; // "-2147483648" has 11 characters
            return Size(-integer) + 1;
        }

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }

    /// <summary>
    /// Calculates the number of bytes required to represent an unsigned 32-bit integer as a decimal string.
    /// </summary>
    /// <param name="integer">The unsigned integer value to calculate the size for.</param>
    /// <returns>The number of bytes needed to represent the unsigned integer as a decimal string.</returns>
    public static int Size(uint integer)
    {
        if (integer == 0)
            return 1;

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }

    /// <summary>
    /// Calculates the number of bytes required to represent a signed 64-bit integer as a decimal string.
    /// </summary>
    /// <param name="integer">The long integer value to calculate the size for.</param>
    /// <returns>The number of bytes needed to represent the long integer as a decimal string, including the negative sign if applicable.</returns>
    public static int Size(long integer)
    {
        if (integer == 0)
            return 1;

        if (integer < 0)
        {
            if (integer == long.MinValue)
                return 20; // "-9223372036854775808" has 20 characters
            return Size(-integer) + 1;
        }

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }

    /// <summary>
    /// Calculates the number of bytes required to represent an unsigned 64-bit integer as a decimal string.
    /// </summary>
    /// <param name="integer">The unsigned long integer value to calculate the size for.</param>
    /// <returns>The number of bytes needed to represent the unsigned long integer as a decimal string.</returns>
    public static int Size(ulong integer)
    {
        if (integer == 0)
            return 1;

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }
}
