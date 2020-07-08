using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.PdfInternals.XRef;
using Synercoding.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    internal class Page : IPdfObject
    {
        private readonly PdfReference _pageTreeNode;
        private readonly PdfPage _page;
        private readonly TableBuilder _tableBuilder;

        public Page(TableBuilder tableBuilder, PdfPage page, PdfReference pageTreeNode)
        {
            _tableBuilder = tableBuilder;
            _page = page;
            _pageTreeNode = pageTreeNode;

            Reference = tableBuilder.ReserveId();
        }

        public PdfReference Reference { get; private set; }

        public bool IsWritten { get; private set; }

        public void Dispose()
        {
            _page.ContentStream.Dispose();
            foreach (var image in _page.Images)
            {
                image.Value.Dispose();
            }
        }

        public uint WriteToStream(Stream stream)
        {
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }

            var position = (uint)stream.Position;

            var dependencies = new List<IPdfObject>();

            stream.IndirectDictionary(Reference, dictionary =>
            {
                dictionary
                    .Type(ObjectType.Page)
                    .Write("/Parent", _pageTreeNode);

                // Boxes
                dictionary
                    .Write("/MediaBox", _page.MediaBox)
                    .Write("/CropBox", _page.CropBox)
                    .Write("/BleedBox", _page.BleedBox)
                    .Write("/TrimBox", _page.TrimBox);

                // Resources
                if (_page.Images.Count == 0)
                {
                    dictionary.Write("/Resources", x => x.EmptyDictionary());
                }
                else
                {
                    dictionary.Write("/Resources", x => x.Dictionary(resources =>
                    {
                        resources.Write("/XObject", y => y.Dictionary(xobject =>
                        {
                            foreach (var image in _page.Images)
                            {
                                xobject.Write("/" + image.Key, image.Value.Reference);
                                dependencies.Add(image.Value);
                            }
                        }));
                    }));
                }

                // Content stream
                dictionary.Write("/Contents", _page.ContentStream.Reference);
                dependencies.Add(_page.ContentStream);
            });
            IsWritten = true;

            _tableBuilder.SetPosition(Reference, position);

            foreach (var dependency in dependencies)
            {
                if (!dependency.IsWritten)
                {
                    var dependentPosition = dependency.WriteToStream(stream);
                    _tableBuilder.SetPosition(dependency.Reference, dependentPosition);
                }
            }

            return position;
        }
    }
}
