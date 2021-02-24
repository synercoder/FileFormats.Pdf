using Synercoding.Primitives;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions
{
    public static class PdfStreamExtensions
    {
        public static PdfStream Space(this PdfStream stream)
            => stream.Write(' ');

        public static PdfStream NewLine(this PdfStream stream)
            => stream.Write('\r').Write('\n');

        public static PdfStream Write(this PdfStream stream, PdfName name)
            => stream.Write(name.ToString());

        public static PdfStream IndirectStream<TPdfObject>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, params StreamFilter[] streamFilters)
            where TPdfObject : IPdfObject
            => contentStream.IndirectStream(pdfObject, stream, _ => { }, streamFilters);

        public static PdfStream IndirectStream<TPdfObject>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, Action<PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
            where TPdfObject : IPdfObject
            => contentStream.IndirectStream(pdfObject, stream, dictionaryAction, static (action, dict) => action(dict), streamFilters);

        public static PdfStream IndirectStream<TPdfObject, TData>(this PdfStream contentStream, TPdfObject pdfObject, Stream stream, TData data, Action<TData, PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
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

        public static PdfStream CopyFrom(this PdfStream stream, Stream data)
        {
            data.CopyTo(stream.InnerStream);
            return stream;
        }

        public static PdfStream IndirectDictionary<TPdfObject>(this PdfStream stream, TPdfObject pdfObject, Action<TPdfObject, PdfDictionary> dictionaryAction)
            where TPdfObject : IPdfObject
        {
            return stream
                .StartObject(pdfObject.Reference)
                .Dictionary(pdfObject, dictionaryAction)
                .EndObject()
                .NewLine();
        }

        public static PdfStream Dictionary<T>(this PdfStream stream, T data, Action<T, PdfDictionary> streamAction)
        {
            stream
                .Write('<')
                .Write('<')
                .NewLine();

            streamAction(data, new PdfDictionary(stream));

            stream
                .Write('>')
                .Write('>')
                .NewLine();

            return stream;
        }

        public static PdfStream Dictionary(this PdfStream stream, Action<PdfDictionary> streamAction)
            => stream.Dictionary(streamAction, static (action, dict) => action(dict));

        public static PdfStream EmptyDictionary(this PdfStream stream)
            => stream.Dictionary(_ => { });

        public static PdfStream Write(this PdfStream stream, PdfReference objectReference)
        {
            return stream
                .Write(objectReference.ObjectId)
                .Space()
                .Write(objectReference.Generation)
                .Space()
                .Write('R');
        }

        public static PdfStream Write(this PdfStream stream, Rectangle rectangle)
        {
            rectangle = rectangle.ConvertTo(Unit.Points);

            return stream.Write(rectangle.LLX.Raw, rectangle.LLY.Raw, rectangle.URX.Raw, rectangle.URY.Raw);
        }
        public static PdfStream StartObject(this PdfStream stream, PdfReference objectReference)
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

        public static PdfStream EndObject(this PdfStream stream)
        {
            return stream
                .Write('e')
                .Write('n')
                .Write('d')
                .Write('o')
                .Write('b')
                .Write('j');
        }
    }
}
