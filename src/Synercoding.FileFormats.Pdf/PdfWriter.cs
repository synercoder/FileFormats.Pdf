using Synercoding.FileFormats.Pdf.PdfInternals;
using Synercoding.FileFormats.Pdf.PdfInternals.Objects;
using Synercoding.FileFormats.Pdf.PdfInternals.XRef;
using System;
using System.Collections.Generic;
using System.IO;

namespace Synercoding.FileFormats.Pdf
{
    public class PdfWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly TableBuilder _tableBuilder = new TableBuilder();

        private readonly PdfReference _pageTreeNode;
        private readonly PdfReference _catalog;

        private readonly ICollection<IPdfObject> _pageReferences = new List<IPdfObject>();

        public PdfWriter(Stream stream)
        {
            _stream = stream;
            (new Header()).WriteToStream(stream);

            _pageTreeNode = _tableBuilder.ReserveId();
            _catalog = _tableBuilder.ReserveId();
        }

        public PdfWriter AddPage(Action<PdfPage> pageAction)
        {
            var page = new PdfPage(_tableBuilder);
            pageAction(page);

            using (var obj = new Page(_tableBuilder, page, _pageTreeNode))
            {
                _pageReferences.Add(obj);

                obj.WriteToStream(_stream);
            }

            return this;
        }

        public void Dispose()
        {
            _writePageTree();

            _writeCatalog();

            _writePdfEnding();

            _stream.Flush();

            _stream.Dispose();
        }

        private void _writePageTree()
        {
            _tableBuilder.SetPosition(_pageTreeNode,  (uint)_stream.Position);

            var tree = new PageTree(_pageTreeNode, _pageReferences);
            tree.WriteToStream(_stream);
        }

        private void _writeCatalog()
        {
            _tableBuilder.SetPosition(_catalog, (uint)_stream.Position);

            var catalog = new Catalog(_catalog, _pageTreeNode);
            catalog.WriteToStream(_stream);
        }

        private void _writePdfEnding()
        {
            if(!_tableBuilder.Validate())
            {
                throw new InvalidOperationException("XRef table is invalid.");
            }

            var xRefTable = _tableBuilder.GetXRefTable();
            var xRefPosition = xRefTable.WriteToStream(_stream);

            var trailer = new Trailer(xRefPosition, xRefTable.Section.ObjectCount, _catalog.ObjectId);
            trailer.WriteToStream(_stream);
        }
    }
}