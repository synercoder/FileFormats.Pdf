using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.Helpers;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals.Objects
{
    internal class DocumentInformationDictionary : IPdfObject
    {
        private DocumentInformation _documentInformation;

        public DocumentInformationDictionary(PdfReference id)
        {
            Reference = id;
        }

        public DocumentInformationDictionary(PdfReference id, DocumentInformation data)
            : this(id)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public PdfReference Reference { get; }

        public bool IsWritten { get; private set; }

        public DocumentInformation Data
        {
            get => _documentInformation;
            set => _documentInformation = value ?? throw new ArgumentNullException(nameof(Data));
        }

        public void Dispose()
        { }

        public uint WriteToStream(Stream stream)
        {
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            stream.IndirectDictionary(Reference, dictionary =>
            {
                if (!string.IsNullOrWhiteSpace(Data.Title))
                    dictionary.Write("/Title", PdfTypeHelper.ToPdfHexadecimalString(Data.Title));
                if (!string.IsNullOrWhiteSpace(Data.Author))
                    dictionary.Write("/Author", PdfTypeHelper.ToPdfHexadecimalString(Data.Author));
                if (!string.IsNullOrWhiteSpace(Data.Subject))
                    dictionary.Write("/Subject", PdfTypeHelper.ToPdfHexadecimalString(Data.Subject));
                if (!string.IsNullOrWhiteSpace(Data.Keywords))
                    dictionary.Write("/Keywords", PdfTypeHelper.ToPdfHexadecimalString(Data.Keywords));
                if (!string.IsNullOrWhiteSpace(Data.Creator))
                    dictionary.Write("/Creator", PdfTypeHelper.ToPdfHexadecimalString(Data.Creator));
                if (!string.IsNullOrWhiteSpace(Data.Producer))
                    dictionary.Write("/Producer", PdfTypeHelper.ToPdfHexadecimalString(Data.Producer));
                if (Data.CreationDate != null)
                    dictionary.Write("/CreationDate", PdfTypeHelper.ToPdfDate(Data.CreationDate.Value));
                if (Data.ModDate != null)
                    dictionary.Write("/ModDate", PdfTypeHelper.ToPdfDate(Data.ModDate.Value));
            });

            IsWritten = true;

            return position;
        }
    }
}
