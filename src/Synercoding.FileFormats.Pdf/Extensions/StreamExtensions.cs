using Synercoding.FileFormats.Pdf.Helpers;
using Synercoding.FileFormats.Pdf.PdfInternals;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Extensions
{
    internal static class StreamExtensions
    {
        public static Stream IndirectStream(this Stream stream, PdfReference objectReference, Stream data, Action<PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
        {
            stream
                ._startObject(objectReference)
                .Dictionary(dictionary =>
                {
                    dictionary.Write("/Length", data.Length);
                    if(streamFilters != null && streamFilters.Length > 0)
                    {
                        if(streamFilters.Length == 1)
                        {
                            dictionary.Write("/Filter", streamFilters[0].ToPdfName());
                        }
                        else
                        {
                            dictionary.Write("/Filter", streamPart =>
                            {
                                streamPart
                                    .WriteSingleByte(0x5B) // [
                                    .Space();

                                foreach(var filter in streamFilters)
                                {
                                    streamPart
                                        .Write(filter.ToPdfName())
                                        .Space();
                                }

                                streamPart
                                    .WriteSingleByte(0x5D); // ]
                            });
                        }
                    }

                    dictionaryAction(dictionary);
                })
                .Write("stream")
                .NewLine()
                ._copyFrom(data)
                .NewLine()
                .Write("endstream")
                .NewLine()
                ._endObject()
                .NewLine();

            return stream;
        }

        public static Stream IndirectDictionary(this Stream stream, PdfReference objectReference, Action<PdfDictionary> dictionaryAction)
        {
            stream
                ._startObject(objectReference)
                .Dictionary(dictionaryAction)
                ._endObject()
                .NewLine();

            return stream;
        }

        public static Stream Dictionary(this Stream stream, Action<PdfDictionary> streamAction)
        {
            stream
                ._startDictionary()
                .NewLine();

            streamAction(new PdfDictionary(stream));

            stream
                ._endDictionary()
                .NewLine();

            return stream;
        }

        public static Stream WriteSingleByte(this Stream stream, byte value)
        {
            stream.WriteByte(value);
            return stream;
        }

        public static Stream Write(this Stream stream, Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);
            return stream
                .WriteSingleByte(0x5B) // [
                .Space()
                .Write(rectangle.LLX.Raw)
                .Space()
                .Write(rectangle.LLY.Raw)
                .Space()
                .Write(rectangle.URX.Raw)
                .Space()
                .Write(rectangle.URY.Raw)
                .Space()
                .WriteSingleByte(0x5D); // ]
        }

        public static Stream Write(this Stream stream, int value)
        {
            var intSize = ByteSizes.Size(value);
            for (int i = intSize - 1; i >= 0; i--)
            {
                var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
                stream.WriteByte(result);
            }

            return stream;
        }

        public static Stream Write(this Stream stream, long value)
        {
            var intSize = ByteSizes.Size(value);
            for (int i = intSize - 1; i >= 0; i--)
            {
                var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
                stream.WriteByte(result);
            }

            return stream;
        }

        public static Stream Write(this Stream stream, double value)
        {
            var stringValue = value.ToString("0.0########", CultureInfo.InvariantCulture);
            var bytes = Encoding.ASCII.GetBytes(stringValue);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        public static Stream Write(this Stream stream, Matrix matrix)
        {
            stream
                .Write(matrix.A)
                .Space()
                .Write(matrix.B)
                .Space()
                .Write(matrix.C)
                .Space()
                .Write(matrix.D)
                .Space()
                .Write(matrix.E)
                .Space()
                .Write(matrix.F)
                .Space()
                .Write("cm");

            return stream;
        }

        public static Stream Space(this Stream stream)
        {
            stream.WriteByte(0x20); // space

            return stream;
        }

        public static Stream Write(this Stream stream, PdfReference objectReference)
        {
            return stream
                .Write(objectReference.ObjectId)
                .Space()
                .Write(objectReference.Generation)
                .Space()
                .WriteSingleByte(0x52); // R
        }

        public static Stream Write(this Stream stream, IEnumerable<PdfReference> objectReferences)
        {
            stream.WriteByte(0x5B); // [

            foreach(var objectReference in objectReferences)
            {
                stream.Write(objectReference);
                stream.NewLine();
            }

            stream.WriteByte(0x5D); // ]

            return stream;
        }

        public static Stream NewLine(this Stream stream)
        {
            stream.WriteByte(0x0D);
            stream.WriteByte(0x0A);

            return stream;
        }

        public static Stream EmptyDictionary(this Stream stream)
        {
            return stream._startDictionary().Space()._endDictionary();
        }

        private static Stream _startDictionary(this Stream stream)
        {
            stream.WriteByte(0x3C);
            stream.WriteByte(0x3C);

            return stream;
        }

        private static Stream _endDictionary(this Stream stream)
        {
            stream.WriteByte(0x3E);
            stream.WriteByte(0x3E);

            return stream;
        }

        private static Stream _startObject(this Stream stream, PdfReference objectReference)
        {
            return stream
                .Write(objectReference.ObjectId)
                .Space()
                .Write(objectReference.Generation)
                .Space()
                .WriteSingleByte(0x6F) // o
                .WriteSingleByte(0x62) // b
                .WriteSingleByte(0x6A) // j
                .NewLine();
        }

        private static Stream _endObject(this Stream stream)
        {
            stream.WriteByte(0x65); // e
            stream.WriteByte(0x6E); // n
            stream.WriteByte(0x64); // d
            stream.WriteByte(0x6F); // o
            stream.WriteByte(0x62); // b
            stream.WriteByte(0x6A); // j

            return stream;
        }

        private static Stream _copyFrom(this Stream stream, Stream data)
        {
            data.CopyTo(stream);
            return stream;
        }
    }
}
