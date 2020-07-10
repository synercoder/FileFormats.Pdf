using Synercoding.FileFormats.Pdf.PdfInternals;
using Synercoding.FileFormats.Pdf.PdfInternals.Objects;
using Synercoding.FileFormats.Pdf.PdfInternals.XRef;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// Main class for writing PDF files to streams
    /// </summary>
    public class PdfWriter : IDisposable
    {
        private readonly bool _ownsStream;
        private readonly Stream _stream;
        private readonly TableBuilder _tableBuilder = new TableBuilder();

        private readonly PdfReference _documentInfo;
        private readonly PdfReference _pageTreeNode;
        private readonly PdfReference _catalog;

        private readonly IList<IPdfObject> _pageReferences = new List<IPdfObject>();

        /// <summary>
        /// Constructor for <see cref="PdfWriter"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the PDF file</param>
        public PdfWriter(Stream stream)
            : this(stream, true)
        { }

        /// <summary>
        /// Constructor for <see cref="PdfWriter"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the PDF file</param>
        /// <param name="ownsStream">If the stream is owned, then when this <see cref="PdfWriter"/> is disposed, the stream is also disposed.</param>
        public PdfWriter(Stream stream, bool ownsStream)
        {
            _stream = stream;
            new Header().WriteToStream(stream);

            _pageTreeNode = _tableBuilder.ReserveId();
            _catalog = _tableBuilder.ReserveId();
            _documentInfo = _tableBuilder.ReserveId();

            _ownsStream = ownsStream;
        }

        /// <summary>
        /// Document information, such as the author and title
        /// </summary>
        public DocumentInformation DocumentInformation { get; } = new DocumentInformation()
        {
            Producer = $"Synercoding.FileFormats.Pdf {typeof(PdfWriter).GetTypeInfo().Assembly.GetName().Version}",
            CreationDate = DateTime.Now
        };

        /// <summary>
        /// Add a page to the pdf file
        /// </summary>
        /// <param name="pageAction">Action used to setup the page</param>
        /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
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

        /// <summary>
        /// Add an <seealso cref="SixLabors.ImageSharp.Image"/> to the pdf file and get the <seealso cref="Image"/> reference returned
        /// </summary>
        /// <param name="image">The image that needs to be added.</param>
        /// <returns>The image reference that can be used in pages</returns>
        public Image AddImage(SixLabors.ImageSharp.Image image)
        {
            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, image);

            var position = pdfImage.WriteToStream(_stream);

            _tableBuilder.SetPosition(id, position);

            return pdfImage;
        }

        /// <summary>
        /// Set meta information for this document
        /// </summary>
        /// <param name="infoAction">Action used to set meta data</param>
        /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
        public PdfWriter SetDocumentInfo(Action<DocumentInformation> infoAction)
        {
            infoAction(DocumentInformation);

            return this;
        }

        /// <summary>
        /// Close the PDF document by writing the pagetree, catalog, xref table and trailer to the <see cref="Stream"/>
        /// </summary>
        public void Dispose()
        {
            _writePageTree();

            _writeCatalog();

            _writeDocumentInformation();

            _writePdfEnding();

            _stream.Flush();

            if (_ownsStream)
            {
                _stream.Dispose();
            }
        }

        private void _writeDocumentInformation()
        {
            _tableBuilder.SetPosition(_documentInfo, (uint)_stream.Position);

            var info = new DocumentInformationDictionary(_documentInfo, DocumentInformation);
            info.WriteToStream(_stream);
        }

        private void _writePageTree()
        {
            _tableBuilder.SetPosition(_pageTreeNode, (uint)_stream.Position);

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
            if (!_tableBuilder.Validate())
            {
                throw new InvalidOperationException("XRef table is invalid.");
            }

            var xRefTable = _tableBuilder.GetXRefTable();
            uint xRefPosition = xRefTable.WriteToStream(_stream);

            var trailer = new Trailer(xRefPosition, xRefTable.Section.ObjectCount, _catalog, _documentInfo);
            trailer.WriteToStream(_stream);
        }
    }
}