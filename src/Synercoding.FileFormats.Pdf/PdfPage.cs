using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// This class represents a page in a pdf
    /// </summary>
    public sealed class PdfPage : IPdfObject, IDisposable
    {
        private int _imageCounter = 0;
        private readonly Map<PdfName, Image> _images = new Map<PdfName, Image>();
        private readonly TableBuilder _tableBuilder;
        private readonly PageTree _parent;

        private int? _rotation;
        private bool _isWritten;

        internal PdfPage(TableBuilder tableBuilder, PageTree parent)
        {
            _tableBuilder = tableBuilder;
            _parent = parent;
            _parent.AddPage(this);

            Reference = tableBuilder.ReserveId();
            ContentStream = new ContentStream(tableBuilder.ReserveId());
        }

        /// <summary>
        /// The content stream of this page
        /// </summary>
        public ContentStream ContentStream { get; }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        /// <summary>
        /// The rotation of how the page is displayed, must be in increments of 90
        /// </summary>
        public int? Rotation
        {
            get => _rotation;
            set
            {
                if (value is not null && value % 90 != 0)
                    throw new ArgumentOutOfRangeException(nameof(Rotation), value, "The provided value can only be increments of 90.");

                _rotation = value;
            }
        }

        /// <summary>
        /// The media box of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle MediaBox { get; set; } = Sizes.A4.AsRectangle();

        /// <summary>
        /// The cropbox of the <see cref="PdfPage"/>, defaults to <see cref="MediaBox"/>
        /// </summary>
        public Rectangle? CropBox { get; set; }

        /// <summary>
        /// The bleedbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? BleedBox { get; set; }

        /// <summary>
        /// The trimbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? TrimBox { get; set; }

        /// <summary>
        /// The artbox of the <see cref="PdfPage"/>
        /// </summary>
        public Rectangle? Art { get; set; }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="image">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(SixLabors.ImageSharp.Image image)
        {
            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, image);

            return _addImageToResources(pdfImage);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="jpgStream">The image to add</param>
        /// <param name="width">The width of the image in the <paramref name="jpgStream"/></param>
        /// <param name="height">The height of the image in the <paramref name="jpgStream"/></param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(System.IO.Stream jpgStream, int width, int height)
        {
            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, jpgStream, width, height);

            return _addImageToResources(pdfImage);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="imageStream">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(System.IO.Stream imageStream)
        {
            using var image = SixLabors.ImageSharp.Image.Load(imageStream);
            return AddImageToResources(image);
        }

        /// <summary>
        /// Add an image to the resources of this page
        /// </summary>
        /// <param name="image">The image to add</param>
        /// <returns>The <see cref="PdfName"/> that can be used to reference this image in the <see cref="ContentStream"/></returns>
        public PdfName AddImageToResources(Image image)
        {
            return _addImageToResources(image);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var kv in _images)
                kv.Value.Dispose();

            ContentStream.Dispose();
        }

        private PdfName _addImageToResources(Image image)
        {
            if (_images.Reverse.Contains(image))
                return _images.Reverse[image];

            var key = "Im" + System.Threading.Interlocked.Increment(ref _imageCounter).ToString().PadLeft(6, '0');

            var pdfName = PdfName.Get(key);

            _images.Add(pdfName, image);

            return pdfName;
        }

        internal uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }

            var position = (uint)stream.Position;
            _tableBuilder.SetPosition(Reference, position);

            stream.IndirectDictionary(this, static (page, dictionary) =>
            {
                dictionary
                    .Type(ObjectType.Page)
                    .Write(PdfName.Get("Parent"), page._parent.Reference);

                // Boxes
                dictionary
                    .Write(PdfName.Get("MediaBox"), page.MediaBox)
                    .WriteIfNotNull(PdfName.Get("CropBox"), page.CropBox)
                    .WriteIfNotNull(PdfName.Get("BleedBox"), page.BleedBox)
                    .WriteIfNotNull(PdfName.Get("TrimBox"), page.TrimBox)
                    .WriteIfNotNull(PdfName.Get("Rotate"), page.Rotation);

                // Resources
                if (page._images.Count == 0)
                {
                    dictionary.Write(PdfName.Get("Resources"), static x => x.EmptyDictionary());
                }
                else
                {
                    dictionary.Write(PdfName.Get("Resources"), page._images, static (images, stream) => stream.Dictionary(images, static (images, resources) =>
                    {
                        resources.Write(PdfName.Get("XObject"), images, static (images, stream) => stream.Dictionary(images, static (images, xobject) =>
                        {
                            foreach (var image in images)
                            {
                                xobject.Write(image.Key, image.Value.Reference);
                            }
                        }));
                    }));
                }

                // Content stream
                dictionary.Write(PdfName.Get("Contents"), page.ContentStream.Reference);
            });

            _isWritten = true;

            foreach (var kv in _images)
            {
                if (kv.Value.TryWriteToStream(stream, out uint dependentPosition))
                {
                    _tableBuilder.SetPosition(kv.Value.Reference, dependentPosition);
                }
            }
            if (!ContentStream.IsWritten)
            {
                var dependentPosition = ContentStream.WriteToStream(stream);
                _tableBuilder.SetPosition(ContentStream.Reference, dependentPosition);
            }

            return position;
        }

        private sealed class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
            where T1 : notnull
            where T2 : notnull
        {
            private readonly IDictionary<T1, T2> _forward = new Dictionary<T1, T2>();
            private readonly IDictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

            public Map()
            {
                Forward = new Indexer<T1, T2>(_forward);
                Reverse = new Indexer<T2, T1>(_reverse);
            }

            public int Count => _forward.Count;

            public void Add(T1 t1, T2 t2)
            {
                _forward.Add(t1, t2);
                _reverse.Add(t2, t1);
            }

            public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
            {
                return _forward.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Indexer<T1, T2> Forward { get; }
            public Indexer<T2, T1> Reverse { get; }

            public sealed class Indexer<T3, T4>
                where T3 : notnull
                where T4 : notnull
            {
                private readonly IDictionary<T3, T4> _dictionary;

                public Indexer(IDictionary<T3, T4> dictionary)
                {
                    _dictionary = dictionary;
                }

                public T4 this[T3 index]
                {
                    get => _dictionary[index];
                    set => _dictionary[index] = value;
                }

                public bool Contains(T3 value)
                    => _dictionary.ContainsKey(value);
            }
        }
    }
}
