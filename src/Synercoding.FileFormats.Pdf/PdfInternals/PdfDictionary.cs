using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.Primitives;
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
            string nameValue = null;
            switch (subType)
            {
                case XObjectSubType.Image:
                    nameValue = "/Image";
                    break;
                default:
                    throw new NotImplementedException("Unknown XObjectSubType: " + subType);
            }

            _stream
                .Write("/Subtype")
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        public PdfDictionary Type(ObjectType objectType)
        {
            string nameValue = null;
            switch (objectType)
            {
                case ObjectType.Catalog:
                    nameValue = "/Catalog";
                    break;
                case ObjectType.Page:
                    nameValue = "/Page";
                    break;
                case ObjectType.Pages:
                    nameValue = "/Pages";
                    break;
                case ObjectType.XObject:
                    nameValue = "/XObject";
                    break;
                default:
                    throw new NotImplementedException("Unknown objectType: " + objectType);
            }

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
