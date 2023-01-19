using Synercoding.Primitives;
using System;
using System.IO;

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
        }

        private static int _toOctal(int number)
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

        internal static PdfStream IndirectStream<TPdfObject>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, params StreamFilter[] streamFilters)
            where TPdfObject : IPdfObject
            => contentStream.IndirectStream(pdfObject, stream, _ => { }, streamFilters);

        internal static PdfStream IndirectStream<TPdfObject>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, Action<PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
            where TPdfObject : IPdfObject
            => contentStream.IndirectStream(pdfObject, stream, dictionaryAction, static (action, dict) => action(dict), streamFilters);

        internal static PdfStream IndirectStream<TPdfObject, TData>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, TData data, Action<TData, PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
            where TPdfObject : IPdfObject
            where TData : notnull
        {
            contentStream
                .StartObject(pdfObject.Reference)
                .Dictionary((stream, data, dictionaryAction, streamFilters), static (tuple, dictionary) =>
                {
                    dictionary.Write(PdfName.Get("Length"), tuple.stream.Length);
                    if (tuple.streamFilters != null && tuple.streamFilters.Length > 0)
                    {
                        if (tuple.streamFilters.Length == 1)
                        {
                            dictionary.Write(PdfName.Get("Filter"), tuple.streamFilters[0].ToPdfName());
                        }
                        else
                        {
                            dictionary.Write(PdfName.Get("Filter"), tuple.streamFilters, static (filters, streamPart) =>
                            {
                                streamPart
                                    .WriteByte(0x5B) // [
                                    .Space();

                                foreach (var filter in filters)
                                {
                                    streamPart
                                        .Write(filter.ToPdfName())
                                        .Space();
                                }

                                streamPart
                                    .WriteByte(0x5D); // ]
                            });
                        }
                    }

                    tuple.dictionaryAction(tuple.data, dictionary);
                })
                .Write("stream")
                .NewLine()
                .CopyFrom(stream)
                .NewLine()
                .Write("endstream")
                .NewLine()
                .EndObject()
                .NewLine();

            return contentStream;
        }

        internal static PdfStream CopyFrom(this PdfStream stream, Stream data)
        {
            data.CopyTo(stream.InnerStream);
            return stream;
        }
    }
}
