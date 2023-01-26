using System;
using System.Globalization;

namespace Synercoding.FileFormats.Pdf.Internals;

internal static class ByteSizes
{
    public static int Size(double value)
    {
        return value.ToString("G", CultureInfo.InvariantCulture).Length;
    }

    public static int Size(int integer)
    {
        if (integer == 0)
            return 1;

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }

    public static int Size(uint integer)
    {
        if (integer == 0)
            return 1;

        return (int)Math.Floor(Math.Log10(integer)) + 1;
    }
}
