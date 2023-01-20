using Synercoding.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions
{
    /// <summary>
    /// Extension method for <see cref="PdfStream"/>
    /// </summary>
    public static class PdfStreamExtensions
    {
        /// <summary>
        /// Write a space ' ' character to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="PdfStream"/> to write to.</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Space(this PdfStream stream)
            => stream.Write(' ');

        /// <summary>
        /// Write a \r\n newline to the stream.
        /// </summary>
        /// <param name="stream">The <see cref="PdfStream"/> to write to.</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream NewLine(this PdfStream stream)
            => stream.Write('\r').Write('\n');

        /// <summary>
        /// Write a <see cref="PdfName"/> to the stream
        /// </summary>
        /// <param name="stream">The <see cref="PdfStream"/> to write to.</param>
        /// <param name="name">The pdf name to write.</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, PdfName name)
            => stream.Write(name.ToString());

        /// <summary>
        /// Write a <typeparamref name="TPdfObject"/> to the stream as a dictionary using <paramref name="dictionaryAction"/>
        /// </summary>
        /// <typeparam name="TPdfObject">The type with data to write to the stream</typeparam>
        /// <param name="stream">The stream to write the data to</param>
        /// <param name="pdfObject">The data to write</param>
        /// <param name="dictionaryAction">The instructions for writing the type</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream IndirectDictionary<TPdfObject>(this PdfStream stream, TPdfObject pdfObject, Action<TPdfObject, PdfDictionary> dictionaryAction)
            where TPdfObject : IPdfObject
        {
            return stream
                .StartObject(pdfObject.Reference)
                .Dictionary(pdfObject, dictionaryAction)
                .EndObject()
                .NewLine();
        }

        internal static PdfStream IndirectDictionary<T>(this PdfStream stream, PdfReference reference, T data, Action<T, PdfDictionary> dictionaryAction)
        {
            return stream
                .StartObject(reference)
                .Dictionary(data, dictionaryAction)
                .EndObject()
                .NewLine();
        }

        /// <summary>
        /// Write a dictionary to the <see cref="PdfStream"/>
        /// </summary>
        /// <typeparam name="T">Type of data to use in the <paramref name="streamAction"/></typeparam>
        /// <param name="stream">The stream to write to</param>
        /// <param name="data">The data to use in the <paramref name="streamAction"/></param>
        /// <param name="streamAction">Action to fill the dictionary</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Dictionary<T>(this PdfStream stream, T data, Action<T, PdfDictionary> streamAction)
        {
            stream
                .Write('<')
                .Write('<');

            streamAction(data, new PdfDictionary(stream));

            stream
                .Write('>')
                .Write('>');

            return stream;
        }

        /// <summary>
        /// Write a dictionary to the <see cref="PdfStream"/>
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="streamAction">Action to fill the dictionary</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Dictionary(this PdfStream stream, Action<PdfDictionary> streamAction)
            => stream.Dictionary(streamAction, static (action, dict) => action(dict));

        /// <summary>
        /// Write an empty dictionary to the <see cref="PdfStream"/>
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream EmptyDictionary(this PdfStream stream)
            => stream.Dictionary(_ => { });

        /// <summary>
        /// Write an object reference to the stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="objectReference">The <see cref="PdfReference"/> to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, PdfReference objectReference)
        {
            return stream
                .Write(objectReference.ObjectId)
                .Space()
                .Write(objectReference.Generation)
                .Space()
                .Write('R');
        }

        /// <summary>
        /// Write a rectangle to the stream as an array of [ LLX LLY URX URY ]
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);

            return stream.Write(rectangle.LLX.Raw, rectangle.LLY.Raw, rectangle.URX.Raw, rectangle.URY.Raw);
        }

        public static PdfStream Write(this PdfStream stream, DateTimeOffset dateTimeOffset)
        {
            stream
                .Write('(')
                .Write('D')
                .Write(':');

            stream.Write(dateTimeOffset.Year);

            if (dateTimeOffset.Month < 10)
                stream.Write('0');
            stream.Write(dateTimeOffset.Month);

            if (dateTimeOffset.Day < 10)
                stream.Write('0');
            stream.Write(dateTimeOffset.Day);

            if (dateTimeOffset.Hour < 10)
                stream.Write('0');
            stream.Write(dateTimeOffset.Hour);

            if (dateTimeOffset.Minute < 10)
                stream.Write('0');
            stream.Write(dateTimeOffset.Minute);

            if (dateTimeOffset.Second < 10)
                stream.Write('0');
            stream.Write(dateTimeOffset.Second);

            var hours = dateTimeOffset.Offset.Hours;
            var minutes = dateTimeOffset.Offset.Minutes;
            if (hours == 0 && minutes == 0)
            {
                stream
                    .Write('Z')
                    .Write('0')
                    .Write('0')
                    .Write('\'')
                    .Write('0')
                    .Write('0');
            }
            else
            {
                if (hours > 0 || ( hours == 0 && minutes > 0 ))
                    stream.Write('+');
                else
                    stream.Write('-');

                if (hours < 10)
                    stream.Write('0');
                stream.Write(hours);

                stream.Write('\'');

                if (minutes < 10)
                    stream.Write('0');
                stream.Write(minutes);
            }

            stream
                .Write(')');

            return stream;
        }

        public static PdfStream WriteHexadecimalString(this PdfStream stream, string value)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(value);

            stream.Write('<');
            foreach (var b in bytes)
            {
                var (c1, c2) = _getByteAsHex(b);

                stream
                    .Write(c1)
                    .Write(c2);
            }
            stream.Write('>');

            return stream;

            static (char C1, char C2) _getByteAsHex(byte b)
            {
                var c1 = _getHexCharForNumber(b % 16);
                var c2 = _getHexCharForNumber(b / 16 % 16);

                return (c1, c2);

                static char _getHexCharForNumber(int number)
                {
                    return number switch
                    {
                        0 => '0',
                        1 => '1',
                        2 => '2',
                        3 => '3',
                        4 => '4',
                        5 => '5',
                        6 => '6',
                        7 => '7',
                        8 => '8',
                        9 => '9',
                        10 => 'A',
                        11 => 'B',
                        12 => 'C',
                        13 => 'D',
                        14 => 'E',
                        15 => 'F',
                        _ => throw new InvalidOperationException()
                    };
                }
            }
        }

        /// <summary>
        /// Write a text to the stream as a string literal
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="value">The string to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        /// <exception cref="InvalidOperationException">Throws when the string contains characters outside of the StandardEncoding range.</exception>
        public static PdfStream WriteStringLiteral(this PdfStream stream, string value)
        {
            stream.WriteByte(0x28); // (

            foreach (var c in value)
            {
                if (c == '\n')
                    stream.Write('\\').Write('n');
                else if (c == '\r')
                    stream.Write('\\').Write('r');
                else if (c == '\t')
                    stream.Write('\\').Write('t');
                else if (c == '\b')
                    stream.Write('\\').Write('b');
                else if (c == '\f')
                    stream.Write('\\').Write('f');
                else if (c == '(')
                    stream.Write('\\').Write('(');
                else if (c == ')')
                    stream.Write('\\').Write(')');
                else if (c == '\\')
                    stream.Write('\\').Write('\\');
                else if (c > 0x7F && c < 0x200)
                    stream.Write('\\').Write(_toOctal(c));
                else if (c <= 0x7F)
                    stream.Write(c);
                else
                    throw new InvalidOperationException("Character is outside of the StandardEncoding range");
            }

            stream.WriteByte(0x29); // )

            return stream;

            static int _toOctal(int number)
            {
                if (number > 511)
                    throw new ArgumentOutOfRangeException(nameof(number), "Number is higher than octal 777 (dec 511).");
                if (number < 0)
                    throw new ArgumentOutOfRangeException(nameof(number), "Only positive numbers can be converted.");

                int resultNumber = 0;

                int quotient = number;
                int multiplier = 1;

                do
                {
                    int remainder = quotient % 8;
                    quotient = quotient / 8;

                    resultNumber += multiplier * remainder;
                    multiplier *= 10;
                }
                while (quotient != 0);

                return resultNumber;
            }
        }

        internal static PdfStream StartObject(this PdfStream stream, PdfReference objectReference)
        {
            return stream
                .Write(objectReference.ObjectId)
                .Space()
                .Write(objectReference.Generation)
                .Space()
                .Write('o')
                .Write('b')
                .Write('j')
                .NewLine();
        }

        internal static PdfStream EndObject(this PdfStream stream)
        {
            return stream
                .NewLine()
                .Write('e')
                .Write('n')
                .Write('d')
                .Write('o')
                .Write('b')
                .Write('j')
                .NewLine();
        }

        internal static PdfStream CopyFrom(this PdfStream stream, Stream data)
        {
            data.Position = 0;
            data.CopyTo(stream.InnerStream);
            return stream;
        }
    }
}
