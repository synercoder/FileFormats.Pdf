using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// This class is the start point for creating a pdf file
    /// </summary>
    public sealed class PdfWriter : IDisposable
    {
        private readonly bool _ownsStream;
        private readonly PdfStream _stream;
        private readonly TableBuilder _tableBuilder = new TableBuilder();

        private readonly PageTree _pageTree;
        private readonly Catalog _catalog;

        private bool _endingWritten = false;

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
            _stream = new PdfStream(stream);
            _writeHeader(_stream);

            _pageTree = new PageTree(_tableBuilder.ReserveId(), _tableBuilder);
            _catalog = new Catalog(_tableBuilder.ReserveId(), _pageTree);

            DocumentInformation = new DocumentInformation(_tableBuilder.ReserveId())
            {
                Producer = $"Synercoding.FileFormats.Pdf {typeof(PdfWriter).GetTypeInfo().Assembly.GetName().Version}",
                CreationDate = DateTime.Now
            };

            _ownsStream = ownsStream;
        }

        /// <summary>
        /// Document information, such as the author and title
        /// </summary>
        public DocumentInformation DocumentInformation { get; }

        /// <summary>
        /// Returns the number of pages already added to the writer
        /// </summary>
        public int PageCount
            => _pageTree.PageCount;

        /// <summary>
        /// Set meta information for this document
        /// </summary>
        /// <param name="infoAction">Action used to set meta data</param>
        /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
        public PdfWriter SetDocumentInfo(Action<DocumentInformation> infoAction)
        {
            _throwWhenEndingWritten()';'

            infoAction(DocumentInformation);

            return this;
        }

        /// <summary>
        /// Add a page to the pdf file
        /// </summary>
        /// <param name="pageAction">Action used to setup the page</param>
        /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
        public PdfWriter AddPage(Action<PdfPage> pageAction)
            => AddPage(pageAction, static (action, page) => action(page));

        /// <summary>
        /// Add a page to the pdf file
        /// </summary>
        /// <param name="data">Data passed into the action</param>
        /// <param name="pageAction">Action used to setup the page</param>
        /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
        public PdfWriter AddPage<T>(T data, Action<T, PdfPage> pageAction)
        {
            _throwWhenEndingWritten();

            using (var page = new PdfPage(_tableBuilder, _pageTree))
            {
                pageAction(data, page);

                page.WriteToStream(_stream);
            }

            return this;
        }

        /// <summary>
        /// Add a page to the pdf file
        /// </summary>
        /// <param name="pageAction">Action used to setup the page</param>
        /// <returns>Returns an awaitable task that resolves into this <see cref="PdfWriter"/> to chain calls</returns>
        public async Task<PdfWriter> AddPageAsync(Func<PdfPage, Task> pageAction)
            => await AddPageAsync(pageAction, static async (action, page) => await action(page));

        /// <summary>
        /// Add a page to the pdf file
        /// </summary>
        /// <param name="data">Data passed into the action</param>
        /// <param name="pageAction">Action used to setup the page</param>
        /// <returns>Returns an awaitable task that resolves into this <see cref="PdfWriter"/> to chain calls</returns>
        public async Task<PdfWriter> AddPageAsync<T>(T data, Func<T, PdfPage, Task> pageAction)
        {
            _throwWhenEndingWritten();

            using (var page = new PdfPage(_tableBuilder, _pageTree))
            {
                await pageAction(data, page);

                page.WriteToStream(_stream);
            }

            return this;
        }

        /// <summary>
        /// Add an <see cref="SixLabors.ImageSharp.Image"/> to the pdf file and get the <see cref="Image"/> reference returned
        /// </summary>
        /// <param name="image">The image that needs to be added.</param>
        /// <returns>The image reference that can be used in pages</returns>
        public Image AddImage(SixLabors.ImageSharp.Image image)
        {
            _throwWhenEndingWritten();

            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, image);

            if (!pdfImage.TryWriteToStream(_stream, out uint position))
                throw new InvalidOperationException("Image was just created but could not be written to stream.");

            _tableBuilder.SetPosition(id, position);

            return pdfImage;
        }

        /// <summary>
        /// Add an jpg <see cref="Stream"/> to the pdf file and get the <see cref="Image"/> reference returned
        /// </summary>
        /// <remarks>
        /// The <paramref name="jpgStream"/> is not checked, and is used as is. Make sure only streams that represent a JPG are used.
        /// </remarks>
        /// <param name="jpgStream">The <see cref="Stream"/> of the jpg image that needs to be added.</param>
        /// <param name="originalWidth">The width of the image in the <paramref name="jpgStream"/>.</param>
        /// <param name="originalHeight">The height of the image in the <paramref name="jpgStream"/>.</param>
        /// <returns>The image reference that can be used in pages</returns>
        public Image AddJpgImageUnsafe(Stream jpgStream, int originalWidth, int originalHeight)
        {
            _throwWhenEndingWritten();

            var id = _tableBuilder.ReserveId();

            var pdfImage = new Image(id, jpgStream, originalWidth, originalHeight);

            if (!pdfImage.TryWriteToStream(_stream, out uint position))
                throw new InvalidOperationException("Image was just created but could not be written to stream.");

            _tableBuilder.SetPosition(id, position);

            return pdfImage;
        }

        /// <summary>
        /// Write the PDF trailer; indicates that the PDF is done.
        /// </summary>
        /// <remarks>
        /// Other calls to this <see cref="PdfWriter"/> will throw or have no effect after call this.
        /// </remarks>
        public void WriteTrailer()
        {
            if (_endingWritten)
                return;

            _writePageTree();

            _writeCatalog();

            _writeDocumentInformation();

            _writePdfEnding();

            _stream.Flush();

            _endingWritten = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            WriteTrailer();

            _stream.Flush();

            if (_ownsStream)
            {
                _stream.Dispose();
            }
        }

        private void _throwWhenEndingWritten()
        {
            if (_endingWritten) throw new InvalidOperationException("Can't change document information when PDF trailer is written to the stream.");
        }

        private static void _writeHeader(PdfStream stream)
        {
            stream.WriteByte(0x25); // %
            stream.WriteByte(0x50); // P
            stream.WriteByte(0x44); // D
            stream.WriteByte(0x46); // F
            stream.WriteByte(0x2D); // -
            stream.WriteByte(0x31); // 1
            stream.WriteByte(0x2E); // .
            stream.WriteByte(0x37); // 7
            stream.WriteByte(0x0D); // CR
            stream.WriteByte(0x0A); // LF
            stream.WriteByte(0x25); // %
            stream.WriteByte(0x81); // binary indicator > 128
            stream.WriteByte(0x82); // binary indicator > 128
            stream.WriteByte(0x83); // binary indicator > 128
            stream.WriteByte(0x84); // binary indicator > 128
            stream.WriteByte(0x0D); // CR
            stream.WriteByte(0x0A); // LF
        }

        private void _writeDocumentInformation()
        {
            _tableBuilder.SetPosition(DocumentInformation.Reference, (uint)_stream.Position);

            DocumentInformation.WriteToStream(_stream);
        }

        private void _writePageTree()
        {
            _tableBuilder.SetPosition(_pageTree.Reference, (uint)_stream.Position);

            _pageTree.WriteToStream(_stream);
        }

        private void _writeCatalog()
        {
            _tableBuilder.SetPosition(_catalog.Reference, (uint)_stream.Position);

            _catalog.WriteToStream(_stream);
        }

        private void _writePdfEnding()
        {
            if (!_tableBuilder.Validate())
                throw new InvalidOperationException("XRef table is invalid.");

            var xRefTable = _tableBuilder.GetXRefTable();
            uint xRefPosition = xRefTable.WriteToStream(_stream);

            _writeTrailer(_stream, xRefPosition, xRefTable.Section.ObjectCount, _catalog.Reference, DocumentInformation.Reference);
        }

        private void _writeTrailer(PdfStream stream, uint startXRef, int size, PdfReference root, PdfReference documentInfo)
        {
            stream
                .Write("trailer")
                .NewLine()
                .Dictionary((size, root, documentInfo), static (triple, dictionary) =>
                {
                    var (size, root, documentInfo) = triple;
                    dictionary
                        .Write(PdfName.Get("Size"), size)
                        .Write(PdfName.Get("Root"), root)
                        .Write(PdfName.Get("Info"), documentInfo);
                })
                .Write("startxref")
                .NewLine()
                .Write(startXRef)
                .NewLine()
                .Write("%%EOF");
        }
    }
}
