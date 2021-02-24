using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.Primitives;
using System;
using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    public class PdfDictionary
    {
        private readonly PdfStream _stream;

        public PdfDictionary(PdfStream stream)
        {
            _stream = stream;
        }

        public PdfDictionary SubType(XObjectSubType subType)
        {
            var nameValue = subType switch
            {
                XObjectSubType.Image => PdfName.Get("Image"),
                _ => throw new NotImplementedException("Unknown XObjectSubType: " + subType)
            };

            _stream
                .Write(PdfName.Get("Subtype"))
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        public PdfDictionary Type(ObjectType objectType)
        {
            var nameValue = objectType switch
            {
                ObjectType.Catalog => PdfName.Get("Catalog"),
                ObjectType.Page => PdfName.Get("Page"),
                ObjectType.Pages => PdfName.Get("Pages"),
                ObjectType.XObject => PdfName.Get("XObject"),
                _ => throw new NotImplementedException("Unknown objectType: " + objectType)
            };

            _stream
                .Write(PdfName.Get("Type"))
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, IEnumerable<PdfReference> objectReferences)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReferences)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value1, double value2)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value1, double value2, double value3)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4, double value5)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4, double value5, double value6)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5, value6)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, long value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value1, int value2)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value1, int value2, int value3)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4, int value5)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4, int value5, int value6)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5, value6)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, string value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }
        public PdfDictionary Write(PdfName key, PdfName value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, PdfReference objectReference)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReference)
                .NewLine();

            return this;
        }

        public PdfDictionary Write<T>(PdfName key, T data, Action<T, PdfStream> rawActions)
        {
            _stream
                .Write(key)
                .Space();
            rawActions(data, _stream);
            _stream.NewLine();

            return this;
        }

        public PdfDictionary Write(PdfName key, Action<PdfStream> rawActions)
            => Write(key, rawActions, static (action, stream) => action(stream));

        public PdfDictionary Write(PdfName key, Rectangle rectangle)
        {
            _stream
                .Write(key)
                .Space()
                .Write(rectangle)
                .NewLine();

            return this;
        }

        public PdfDictionary WriteIfNotNull(PdfName key, Rectangle? rectangle)
        {
            if (!rectangle.HasValue)
                return this;

            return Write(key, rectangle.Value);
        }

        public PdfDictionary WriteIfNotNull(PdfName key, int? value)
        {
            if (!value.HasValue)
                return this;

            return Write(key, value.Value);
        }
    }
}