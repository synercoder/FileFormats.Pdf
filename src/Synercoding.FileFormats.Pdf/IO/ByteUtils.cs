namespace Synercoding.FileFormats.Pdf.IO;

public static class ByteUtils
{
    public const byte PARENTHESIS_OPEN = (byte)'(';
    public const byte PARENTHESIS_CLOSED = (byte)')';
    public const byte LESS_THAN_SIGN = (byte)'<';
    public const byte GREATER_THAN_SIGN = (byte)'>';
    public const byte LEFT_SQUARE_BRACKET = (byte)'[';
    public const byte RIGHT_SQUARE_BRACKET = (byte)']';
    public const byte LEFT_CURLY_BRACKET = (byte)'{';
    public const byte RIGHT_CURLY_BRACKET = (byte)'}';
    public const byte SOLIDUS = (byte)'/';
    public const byte REVERSE_SOLIDUS = (byte)'\\';
    public const byte PERCENT_SIGN = (byte)'%';

    public const byte NULL = 0x00;
    public const byte HORIZONTAL_TAB = 0x09;
    public const byte LINE_FEED = 0x0A;
    public const byte FORM_FEED = 0x0C;
    public const byte CARRIAGE_RETURN = 0x0D;
    public const byte SPACE = 0x20;

    public static bool IsDelimiter(byte b, bool insidePostScriptCalculator = false)
    {
        return (b, insidePostScriptCalculator) switch
        {
            (LEFT_CURLY_BRACKET, true) => true,
            (RIGHT_CURLY_BRACKET, true) => true,
            (PARENTHESIS_OPEN, _) => true,
            (PARENTHESIS_CLOSED, _) => true,
            (LESS_THAN_SIGN, _) => true,
            (GREATER_THAN_SIGN, _) => true,
            (LEFT_SQUARE_BRACKET, _) => true,
            (RIGHT_SQUARE_BRACKET, _) => true,
            (SOLIDUS, _) => true,
            (PERCENT_SIGN, _) => true,
            _ => false
        };
    }

    public static bool IsDelimiterOrWhiteSpace(byte b, bool insidePostScriptCalculator = false)
        => IsDelimiter(b, insidePostScriptCalculator) || IsWhiteSpace(b);

    public static bool IsWhiteSpace(byte b)
    {
        return b switch
        {
            NULL => true,
            HORIZONTAL_TAB => true,
            LINE_FEED => true,
            FORM_FEED => true,
            CARRIAGE_RETURN => true,
            SPACE => true,
            _ => false
        };
    }

    public static bool IsCRLF(byte b1, byte b2)
        => b1 == CARRIAGE_RETURN && b2 == LINE_FEED;

    public static bool IsOctal(byte b)
        => b >= '0' && b <= '7';

    public static bool IsHex(byte b)
        => ( b >= '0' && b <= '9' )
        || ( b >= 'a' && b <= 'f' )
        || ( b >= 'A' && b <= 'F' );

    public static bool IsChar(byte b)
        => ( b >= 'a' && b <= 'z' )
        || ( b >= 'A' && b <= 'Z' );
}
