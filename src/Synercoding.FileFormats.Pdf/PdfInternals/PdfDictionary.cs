using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals
{
    internal class PdfDictionary
    {
        private readonly Stream _stream;

        public PdfDictionary(Stream stream)
        {
            _stream = stream;
        }

        public PdfDictionary SubType(XObjectSubType subType)
        {
            var nameValue = subType switch
            {
                XObjectSubType.Image => "/Image",
                _ => throw new NotImplementedException("Unknown XObjectSubType: " + subType)
            };

            _stream
                .Write("/Subtype")
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        public PdfDictionary Type(ObjectType objectType)
        {
            var nameValue = objectType switch
            {
                ObjectType.Catalog => "/Catalog",
                ObjectType.Page => "/Page",
                ObjectType.Pages => "/Pages",
                ObjectType.XObject => "/XObject",
                _ => throw new NotImplementedException("Unknown objectType: " + objectType)
            };

            _stream
                .Write("/Type")
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, IEnumerable<PdfReference> objectReferences)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReferences)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, int value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, long value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, string value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, PdfReference objectReference)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReference)
                .NewLine();

            return this;
        }

        public PdfDictionary Write(string key, Action<Stream> rawActions)
        {
            _stream
                .Write(key)
                .Space();
            rawActions(_stream);
            _stream.NewLine();

            return this;
        }

        public PdfDictionary Write(string key, Rectangle rectangle)
        {
            _stream
                .Write(key)
                .Space()
                .Write(rectangle)
                .NewLine();

            return this;
        }
    }
}
