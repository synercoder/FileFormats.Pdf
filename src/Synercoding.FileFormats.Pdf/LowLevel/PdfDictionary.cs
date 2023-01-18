using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.Primitives;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    /// <summary>
    /// Class represents a dictionary in a pdf
    /// </summary>
    public class PdfDictionary
    {
        private readonly PdfStream _stream;

        /// <summary>
        /// Constructor for <see cref="PdfDictionary"/>
        /// </summary>
        /// <param name="stream">The stream to </param>
        public PdfDictionary(PdfStream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Write an array of <see cref="PdfReference"/>s to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="objectReferences">The array to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, PdfReference[] objectReferences)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReferences)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a number to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The number to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value1, double value2)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value1, double value2, double value3)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <param name="value5">The fifth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4, double value5)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <param name="value5">The fifth number to write in the array</param>
        /// <param name="value6">The sixth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, double value1, double value2, double value3, double value4, double value5, double value6)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5, value6)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a number to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The number to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, long value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a number to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The number to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value1, int value2)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value1, int value2, int value3)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <param name="value5">The fifth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4, int value5)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write an array of numbers to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value1">The first number to write in the array</param>
        /// <param name="value2">The second number to write in the array</param>
        /// <param name="value3">The third number to write in the array</param>
        /// <param name="value4">The fourth number to write in the array</param>
        /// <param name="value5">The fifth number to write in the array</param>
        /// <param name="value6">The sixth number to write in the array</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, int value1, int value2, int value3, int value4, int value5, int value6)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value1, value2, value3, value4, value5, value6)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a text to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The text to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, string value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a <see cref="PdfName"/> to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The <see cref="PdfName"/> to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, PdfName value)
        {
            _stream
                .Write(key)
                .Space()
                .Write(value)
                .NewLine();

            return this;
        }


        /// <summary>
        /// Write a <see cref="PdfReference"/> to the dictionary
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="objectReference">The <see cref="PdfReference"/> to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, PdfReference objectReference)
        {
            _stream
                .Write(key)
                .Space()
                .Write(objectReference)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Use an action to write things to the stream
        /// </summary>
        /// <typeparam name="T">The type of data to pass to <paramref name="rawActions"/></typeparam>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="data">Data to use in the <paramref name="rawActions"/></param>
        /// <param name="rawActions">The action to use to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write<T>(PdfName key, T data, Action<T, PdfStream> rawActions)
        {
            _stream
                .Write(key)
                .Space();

            rawActions(data, _stream);

            _stream.NewLine();

            return this;
        }

        /// <summary>
        /// Use an action to write things to the stream
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="rawActions">The action to use to write</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, Action<PdfStream> rawActions)
            => Write(key, rawActions, static (action, stream) => action(stream));

        /// <summary>
        /// Write a <see cref="Rectangle"/> to the stream
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> to write.</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary Write(PdfName key, Rectangle rectangle)
        {
            _stream
                .Write(key)
                .Space()
                .Write(rectangle)
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a rectangle to the stream if it is not null
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="rectangle">The <see cref="Rectangle"/> to write.</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary WriteIfNotNull(PdfName key, Rectangle? rectangle)
            => rectangle.HasValue
            ? Write(key, rectangle.Value)
            : this;

        /// <summary>
        /// Write a number to the stream if it is not null
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The number to write.</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary WriteIfNotNull(PdfName key, int? value)
            => value.HasValue
            ? Write(key, value.Value)
            : this;

        /// <summary>
        /// Write a number to the stream if it is not null
        /// </summary>
        /// <param name="key">The key of the item in the dictionary</param>
        /// <param name="value">The number to write.</param>
        /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
        public PdfDictionary WriteIfNotNull(PdfName key, double? value)
            => value.HasValue
            ? Write(key, value.Value)
            : this;

        internal PdfDictionary Type(ObjectType objectType)
        {
            var nameValue = objectType switch
            {
                ObjectType.Catalog => PdfName.Get("Catalog"),
                ObjectType.Page => PdfName.Get("Page"),
                ObjectType.Pages => PdfName.Get("Pages"),
                ObjectType.XObject => PdfName.Get("XObject"),
                ObjectType.Font => PdfName.Get("Font"),
                _ => throw new NotImplementedException("Unknown objectType: " + objectType)
            };

            _stream
                .Write(PdfName.Get("Type"))
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }

        internal PdfDictionary SubType(XObjectSubType subType)
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

        internal PdfDictionary SubType(FontSubType subType)
        {
            var nameValue = subType switch
            {
                FontSubType.Type1 => PdfName.Get("Type1"),
                _ => throw new NotImplementedException("Unknown FontSubType: " + subType)
            };

            _stream
                .Write(PdfName.Get("Subtype"))
                .Space()
                .Write(nameValue)
                .NewLine();

            return this;
        }
    }
}