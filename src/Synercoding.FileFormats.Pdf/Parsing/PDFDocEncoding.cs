using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Parsing;

/// <summary>
/// Contains methods to encode and decode PDFDocEncoding.
/// </summary>
public static class PDFDocEncoding
{
    // Characters / unicode points to supported byte values.
    private static readonly FrozenDictionary<char, byte> _encodeLookup = new Dictionary<char, byte>()
    {
        { (char)0x0009, 0x09 }, // (CHARACTER TABULATION)
        { (char)0x000A, 0x0A }, // (LINE FEED)
        { (char)0x000D, 0x0D }, // (CARRIAGE RETURN)
        { (char)0x02D8, 0x18 }, // BREVE
        { (char)0x02C7, 0x19 }, // CARON
        { (char)0x02C6, 0x1A }, // MODIFIER LETTER CIRCUMFLEX ACCENT
        { (char)0x02D9, 0x1B }, // DOT ABOVE
        { (char)0x02DD, 0x1C }, // DOUBLE ACUTE ACCENT
        { (char)0x02DB, 0x1D }, // OGONEK
        { (char)0x02DA, 0x1E }, // RING ABOVE
        { (char)0x02DC, 0x1F }, // SMALL TILDE
        { ' ',          0x20 }, // SPACE
        { '!',          0x21 }, // EXCLAMATION MARK
        { '"',          0x22 }, // QUOTATION MARK
        { '#',          0x23 }, // NUMBER SIGN
        { '$',          0x24 }, // DOLLAR SIGN
        { '%',          0x25 }, // PERCENT SIGN
        { '&',          0x26 }, // AMBERSAND
        { '\'',         0x27 }, // APOSTROPHE
        { '(',          0x28 }, // LEFT PARENTHESIS
        { ')',          0x29 }, // RIGHT PARENTHESIS
        { '*',          0x2A }, // ASTERISK
        { '+',          0x2B }, // PLUS SIGN
        { ',',          0x2C }, // COMMA
        { '-',          0x2D }, // HYPHEN-MINUS
        { '.',          0x2E }, // FULL STOP
        { '/',          0x2F }, // SOLIDUS
        { '0',          0x30 }, // DIGIT ZERO
        { '1',          0x31 }, // DIGIT ONE
        { '2',          0x32 }, // DIGIT TWO
        { '3',          0x33 }, // DIGIT THREE
        { '4',          0x34 }, // DIGIT FOUR
        { '5',          0x35 }, // DIGIT FIVE
        { '6',          0x36 }, // DIGIT SIX
        { '7',          0x37 }, // DIGIT SEVEN
        { '8',          0x38 }, // DIGIT EIGHT
        { '9',          0x39 }, // DIGIT NINE
        { ':',          0x3A }, // COLON
        { ';',          0x3B }, // SEMICOLON
        { '<',          0x3C }, // LESS THAN SIGN
        { '=',          0x3D }, // EQUALS SIGN
        { '>',          0x3E }, // GREATER THAN SIGN
        { '?',          0x3F }, // QUESTION MARK
        { '@',          0x40 }, // COMMERCIAL AT
        { 'A',          0x41 }, // LATIN CAPITAL LETTER A
        { 'B',          0x42 }, // LATIN CAPITAL LETTER B
        { 'C',          0x43 }, // LATIN CAPITAL LETTER C
        { 'D',          0x44 }, // LATIN CAPITAL LETTER D
        { 'E',          0x45 }, // LATIN CAPITAL LETTER E
        { 'F',          0x46 }, // LATIN CAPITAL LETTER F
        { 'G',          0x47 }, // LATIN CAPITAL LETTER G
        { 'H',          0x48 }, // LATIN CAPITAL LETTER H
        { 'I',          0x49 }, // LATIN CAPITAL LETTER I
        { 'J',          0x4A }, // LATIN CAPITAL LETTER J
        { 'K',          0x4B }, // LATIN CAPITAL LETTER K
        { 'L',          0x4C }, // LATIN CAPITAL LETTER L
        { 'M',          0x4D }, // LATIN CAPITAL LETTER M
        { 'N',          0x4E }, // LATIN CAPITAL LETTER N
        { 'O',          0x4F }, // LATIN CAPITAL LETTER O
        { 'P',          0x50 }, // LATIN CAPITAL LETTER P
        { 'Q',          0x51 }, // LATIN CAPITAL LETTER Q
        { 'R',          0x52 }, // LATIN CAPITAL LETTER R
        { 'S',          0x53 }, // LATIN CAPITAL LETTER S
        { 'T',          0x54 }, // LATIN CAPITAL LETTER T
        { 'U',          0x55 }, // LATIN CAPITAL LETTER U
        { 'V',          0x56 }, // LATIN CAPITAL LETTER V
        { 'W',          0x57 }, // LATIN CAPITAL LETTER W
        { 'X',          0x58 }, // LATIN CAPITAL LETTER X
        { 'Y',          0x59 }, // LATIN CAPITAL LETTER Y
        { 'Z',          0x5A }, // LATIN CAPITAL LETTER Z
        { '[',          0x5B }, // LEFT SQUARE BRACKET
        { '\\',         0x5C }, // REVERSE SOLIDUS
        { ']',          0x5D }, // RIGHT SQUARE BRACKET
        { '^',          0x5E }, // CIRCUMFLEX ACCENT
        { '_',          0x5F }, // LOW LINE
        { '`',          0x60 }, // GRAVE ACCENT
        { 'a',          0x61 }, // LATIN SMALL LETTER A
        { 'b',          0x62 }, // LATIN SMALL LETTER B
        { 'c',          0x63 }, // LATIN SMALL LETTER C
        { 'd',          0x64 }, // LATIN SMALL LETTER D
        { 'e',          0x65 }, // LATIN SMALL LETTER E
        { 'f',          0x66 }, // LATIN SMALL LETTER F
        { 'g',          0x67 }, // LATIN SMALL LETTER G
        { 'h',          0x68 }, // LATIN SMALL LETTER H
        { 'i',          0x69 }, // LATIN SMALL LETTER I
        { 'j',          0x6A }, // LATIN SMALL LETTER J
        { 'k',          0x6B }, // LATIN SMALL LETTER K
        { 'l',          0x6C }, // LATIN SMALL LETTER L
        { 'm',          0x6D }, // LATIN SMALL LETTER M
        { 'n',          0x6E }, // LATIN SMALL LETTER N
        { 'o',          0x6F }, // LATIN SMALL LETTER O
        { 'p',          0x70 }, // LATIN SMALL LETTER P
        { 'q',          0x71 }, // LATIN SMALL LETTER Q
        { 'r',          0x72 }, // LATIN SMALL LETTER R
        { 's',          0x73 }, // LATIN SMALL LETTER S
        { 't',          0x74 }, // LATIN SMALL LETTER T
        { 'u',          0x75 }, // LATIN SMALL LETTER U
        { 'v',          0x76 }, // LATIN SMALL LETTER V
        { 'w',          0x77 }, // LATIN SMALL LETTER W
        { 'x',          0x78 }, // LATIN SMALL LETTER X
        { 'y',          0x79 }, // LATIN SMALL LETTER Y
        { 'z',          0x7A }, // LATIN SMALL LETTER Z
        { '{',          0x7B }, // LEFT CURLY BRACKET
        { '|',          0x7C }, // VERTICAL LINE
        { '}',          0x7D }, // RIGHT CURLY BRACKET
        { '~',          0x7E }, // TILDE
        { (char)0x2022, 0x80 }, // BULLET
        { (char)0x2020, 0x81 }, // DAGGER
        { (char)0x2021, 0x82 }, // DOUBLE DAGGER
        { (char)0x2026, 0x83 }, // HORIZONTAL ELLIPSIS
        { (char)0x2014, 0x84 }, // EM DASH
        { (char)0x2013, 0x85 }, // EN DASH
        { (char)0x0192, 0x86 }, // LATIN SMALL LETTER F WITH HOOK
        { (char)0x2044, 0x87 }, // FRACTION SLASH
        { (char)0x2039, 0x88 }, // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
        { (char)0x203A, 0x89 }, // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
        { (char)0x2212, 0x8A }, // MINUS SIGN
        { (char)0x2030, 0x8B }, // PER MILLE SIGN
        { (char)0x201E, 0x8C }, // DOUBLE LOW-9 QUOTATION MARK
        { (char)0x201C, 0x8D }, // LEFT DOUBLE QUOTATION MARK
        { (char)0x201D, 0x8E }, // RIGHT DOUBLE QUOTATION MARK
        { (char)0x2018, 0x8F }, // LEFT SINGLE QUOTATION MARK
        { (char)0x2019, 0x90 }, // RIGHT SINGLE QUOTATION MARK
        { (char)0x201A, 0x91 }, // SINGLE LOW-9 QUOTATION MARK
        { (char)0x2122, 0x92 }, // SINGLE LOW-9 QUOTATION MARK
        { (char)0xFB01, 0x93 }, // LATIN SMALL LIGATURE FI
        { (char)0xFB02, 0x94 }, // LATIN SMALL LIGATURE FL
        { (char)0x0141, 0x95 }, // LATIN CAPITAL LETTER L WITH STROKE
        { (char)0x0152, 0x96 }, // LATIN CAPITAL LIGATURE OE
        { (char)0x0160, 0x97 }, // LATIN CAPITAL LETTER S WITH CARON
        { (char)0x0178, 0x98 }, // LATIN CAPITAL LETTER Y WITH DIAERESIS
        { (char)0x017D, 0x99 }, // LATIN CAPITAL LETTER Z WITH CARON
        { (char)0x0131, 0x9A }, // LATIN SMALL LETTER DOTLESS I
        { (char)0x0142, 0x9B }, // LATIN SMALL LETTER L WITH STROKE
        { (char)0x0153, 0x9C }, // LATIN SMALL LIGATURE OE
        { (char)0x0161, 0x9D }, // LATIN SMALL LETTER S WITH CARON
        { (char)0x017E, 0x9E }, // LATIN SMALL LETTER Z WITH CARON
        { (char)0x20AC, 0xA0 }, // EURO SIGN
        { (char)0x00A1, 0xA1 }, // INVERTED EXCLAMATION MARK
        { (char)0x00A2, 0xA2 }, // CENT SIGN
        { (char)0x00A3, 0xA3 }, // POUND SIGN
        { (char)0x00A4, 0xA4 }, // CURRENCY SIGN
        { (char)0x00A5, 0xA5 }, // YEN SIGN
        { (char)0x00A6, 0xA6 }, // BROKEN BAR
        { (char)0x00A7, 0xA7 }, // SECTION SIGN
        { (char)0x00A8, 0xA8 }, // DIAERESIS
        { (char)0x00A9, 0xA9 }, // COPYRIGHT SIGN
        { (char)0x00AA, 0xAA }, // FEMININE ORDINAL INDICATOR
        { (char)0x00AB, 0xAB }, // LEFT-POINTING DOUBLE ANGLE QUOTATION MARK
        { (char)0x00AC, 0xAC }, // NOT SIGN
        { (char)0x00AE, 0xAE }, // REGISTERED SIGN
        { (char)0x00AF, 0xAF }, // MACRON
        { (char)0x00B0, 0xB0 }, // DEGREE SIGN
        { (char)0x00B1, 0xB1 }, // PLUS-MINUS SIGN
        { (char)0x00B2, 0xB2 }, // SUPERSCRIPT TWO
        { (char)0x00B3, 0xB3 }, // SUPERSCRIPT THREE
        { (char)0x00B4, 0xB4 }, // ACUTE ACCENT
        { (char)0x00B5, 0xB5 }, // MICRO SIGN
        { (char)0x00B6, 0xB6 }, // PILCROW SIGN
        { (char)0x00B7, 0xB7 }, // MIDDLE DOT
        { (char)0x00B8, 0xB8 }, // CEDILLA
        { (char)0x00B9, 0xB9 }, // SUPERSCRIPT ONE
        { (char)0x00BA, 0xBA }, // MASCULINE ORDINAL INDICATOR
        { (char)0x00BB, 0xBB }, // RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK
        { (char)0x00BC, 0xBC }, // VULGAR FRACTION ONE QUARTER
        { (char)0x00BD, 0xBD }, // VULGAR FRACTION ONE HALF
        { (char)0x00BE, 0xBE }, // VULGAR FRACTION THREE QUARTERS
        { (char)0x00BF, 0xBF }, // INVERTED QUESTION MARK
        { (char)0x00C0, 0xC0 }, // LATIN CAPITAL LETTER A WITH GRAVE
        { (char)0x00C1, 0xC1 }, // LATIN CAPITAL LETTER A WITH ACUTE
        { (char)0x00C2, 0xC2 }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX
        { (char)0x00C3, 0xC3 }, // LATIN CAPITAL LETTER A WITH TILDE
        { (char)0x00C4, 0xC4 }, // LATIN CAPITAL LETTER A WITH DIAERESIS
        { (char)0x00C5, 0xC5 }, // LATIN CAPITAL LETTER A WITH RING ABOVE
        { (char)0x00C6, 0xC6 }, // LATIN CAPITAL LETTER AE
        { (char)0x00C7, 0xC7 }, // LATIN CAPITAL LETTER C WITH CEDILLA
        { (char)0x00C8, 0xC8 }, // LATIN CAPITAL LETTER E WITH GRAVE
        { (char)0x00C9, 0xC9 }, // LATIN CAPITAL LETTER E WITH ACUTE
        { (char)0x00CA, 0xCA }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX
        { (char)0x00CB, 0xCB }, // LATIN CAPITAL LETTER E WITH DIAERESIS
        { (char)0x00CC, 0xCC }, // LATIN CAPITAL LETTER I WITH GRAVE
        { (char)0x00CD, 0xCD }, // LATIN CAPITAL LETTER I WITH ACUTE
        { (char)0x00CE, 0xCE }, // LATIN CAPITAL LETTER I WITH CIRCUMFLEX
        { (char)0x00CF, 0xCF }, // LATIN CAPITAL LETTER I WITH DIAERESIS
        { (char)0x00D0, 0xD0 }, // LATIN CAPITAL LETTER ETH
        { (char)0x00D1, 0xD1 }, // LATIN CAPITAL LETTER N WITH TILDE
        { (char)0x00D2, 0xD2 }, // LATIN CAPITAL LETTER O WITH GRAVE
        { (char)0x00D3, 0xD3 }, // LATIN CAPITAL LETTER O WITH ACUTE
        { (char)0x00D4, 0xD4 }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX
        { (char)0x00D5, 0xD5 }, // LATIN CAPITAL LETTER O WITH TILDE
        { (char)0x00D6, 0xD6 }, // LATIN CAPITAL LETTER O WITH DIAERESIS
        { (char)0x00D7, 0xD7 }, // MULTIPLICATION SIGN
        { (char)0x00D8, 0xD8 }, // LATIN CAPITAL LETTER O WITH STROKE
        { (char)0x00D9, 0xD9 }, // LATIN CAPITAL LETTER U WITH GRAVE
        { (char)0x00DA, 0xDA }, // LATIN CAPITAL LETTER U WITH ACUTE
        { (char)0x00DB, 0xDB }, // LATIN CAPITAL LETTER U WITH CIRCUMFLEX
        { (char)0x00DC, 0xDC }, // LATIN CAPITAL LETTER U WITH DIAERESIS
        { (char)0x00DD, 0xDD }, // LATIN CAPITAL LETTER Y WITH ACUTE
        { (char)0x00DE, 0xDE }, // LATIN CAPITAL LETTER THORN
        { (char)0x00DF, 0xDF }, // LATIN SMALL LETTER SHARP S
        { (char)0x00E0, 0xE0 }, // LATIN SMALL LETTER A WITH GRAVE
        { (char)0x00E1, 0xE1 }, // LATIN SMALL LETTER A WITH ACUTE
        { (char)0x00E2, 0xE2 }, // LATIN SMALL LETTER A WITH CIRCUMFLEX
        { (char)0x00E3, 0xE3 }, // LATIN SMALL LETTER A WITH TILDE
        { (char)0x00E4, 0xE4 }, // LATIN SMALL LETTER A WITH DIAERESIS
        { (char)0x00E5, 0xE5 }, // LATIN SMALL LETTER A WITH RING ABOVE
        { (char)0x00E6, 0xE6 }, // LATIN SMALL LETTER AE
        { (char)0x00E7, 0xE7 }, // LATIN SMALL LETTER C WITH CEDILLA
        { (char)0x00E8, 0xE8 }, // LATIN SMALL LETTER E WITH GRAVE
        { (char)0x00E9, 0xE9 }, // LATIN SMALL LETTER E WITH ACUTE
        { (char)0x00EA, 0xEA }, // LATIN SMALL LETTER E WITH CIRCUMFLEX
        { (char)0x00EB, 0xEB }, // LATIN SMALL LETTER E WITH DIAERESIS
        { (char)0x00EC, 0xEC }, // LATIN SMALL LETTER I WITH GRAVE
        { (char)0x00ED, 0xED }, // LATIN SMALL LETTER I WITH ACUTE
        { (char)0x00EE, 0xEE }, // LATIN SMALL LETTER I WITH CIRCUMFLEX
        { (char)0x00EF, 0xEF }, // LATIN SMALL LETTER I WITH DIAERESIS
        { (char)0x00F0, 0xF0 }, // LATIN SMALL LETTER ETH
        { (char)0x00F1, 0xF1 }, // LATIN SMALL LETTER N WITH TILDE
        { (char)0x00F2, 0xF2 }, // LATIN SMALL LETTER O WITH GRAVE
        { (char)0x00F3, 0xF3 }, // LATIN SMALL LETTER O WITH ACUTE
        { (char)0x00F4, 0xF4 }, // LATIN SMALL LETTER O WITH CIRCUMFLEX
        { (char)0x00F5, 0xF5 }, // LATIN SMALL LETTER O WITH TILDE
        { (char)0x00F6, 0xF6 }, // LATIN SMALL LETTER O WITH DIAERESIS
        { (char)0x00F7, 0xF7 }, // DIVISION SIGN
        { (char)0x00F8, 0xF8 }, // LATIN SMALL LETTER O WITH STROKE
        { (char)0x00F9, 0xF9 }, // LATIN SMALL LETTER U WITH GRAVE
        { (char)0x00FA, 0xFA }, // LATIN SMALL LETTER U WITH ACUTE
        { (char)0x00FB, 0xFB }, // LATIN SMALL LETTER U WITH CIRCUMFLEX
        { (char)0x00FC, 0xFC }, // LATIN SMALL LETTER U WITH DIAERESIS
        { (char)0x00FD, 0xFD }, // LATIN SMALL LETTER Y WITH ACUTE
        { (char)0x00FE, 0xFE }, // LATIN SMALL LETTER THORN
        { (char)0x00FF, 0xFF }, // LATIN SMALL LETTER Y WITH DIAERESIS
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<byte, char> _decodeLookup = _encodeLookup
        .ToDictionary(kv => kv.Value, kv => kv.Key) // Switch key and value
        .ToFrozenDictionary();

    /// <summary>
    /// Check if the provided <paramref name="bytes"/> can be decoded with <see cref="PDFDocEncoding"/>.
    /// </summary>
    /// <param name="bytes">The byte array to check.</param>
    /// <returns>True when all bytes are supported code points, otherwise false.</returns>
    public static bool CanDecode(byte[] bytes)
    {
        foreach(var b in bytes)
            if(!IsSupportedCodePoint(b))
                return false;

        return true;
    }

    /// <summary>
    /// Check if provided character can be encoded using <see cref="PDFDocEncoding"/>.
    /// </summary>
    /// <param name="character">The <see cref="char"/> to check.</param>
    /// <returns>True if provided character can be encoded.</returns>
    public static bool IsSupportedCharacter(char character)
        => _encodeLookup.ContainsKey(character);

    /// <summary>
    /// Check if provided byte can be decoded using <see cref="PDFDocEncoding"/>.
    /// </summary>
    /// <param name="codePoint">The <see cref="byte"/> to check.</param>
    /// <returns>True if provided byte can be decoded.</returns>
    public static bool IsSupportedCodePoint(byte codePoint)
        => _decodeLookup.ContainsKey(codePoint);

    /// <summary>
    /// Encode <paramref name="value"/> using <see cref="PDFDocEncoding"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A byte array containing the encoded <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> contains a character not supported by <see cref="PDFDocEncoding"/>.
    /// </exception>
    /// <remarks>
    /// This method will only encode the data, the resulting data will not be escaped.
    /// </remarks>
    public static byte[] Encode(string value)
    {
        var bytes = new byte[value.Length];

        for (int index = 0; index < value.Length; index++)
        {
            if (!_encodeLookup.TryGetValue(value[index], out byte b))
                throw new ArgumentOutOfRangeException(nameof(value), "Provided value contains characters not supported by PDFDocEncoding.");

            bytes[index] = b;
        }

        return bytes;
    }

    /// <summary>
    /// Decode <paramref name="bytes"/> into a string using <see cref="PDFDocEncoding"/>.
    /// </summary>
    /// <param name="bytes">The bytes to decode into a <see cref="string"/>.</param>
    /// <returns>The decoded <see cref="string"/>.</returns>
    /// <exception cref="DecoderFallbackException">
    /// Thrown when <paramref name="bytes"/> contains code points not supported by <see cref="PDFDocEncoding"/>.
    /// </exception>
    /// <remarks>
    /// This method will only decode the data, the input should already be unescaped.
    /// </remarks>
    public static string Decode(byte[] bytes)
    {
        var builder = new StringBuilder(bytes.Length);

        int index = 0;
        while (index < bytes.Length)
        {
            byte b1 = bytes[index];

            if (_decodeLookup.TryGetValue(b1, out char c))
            {
                builder.Append(c);
            }
            else
            {
                _throwUndefinedCodePoint(b1, index);
            }
            index++;
        }

        return builder.ToString();
    }

    [DoesNotReturn]
    private static void _throwUndefinedCodePoint(byte b, int index)
        => throw new DecoderFallbackException($"Byte 0x{b:X2} is an undefined codepoint in PDFDocEncoding.", [b], index);
}
