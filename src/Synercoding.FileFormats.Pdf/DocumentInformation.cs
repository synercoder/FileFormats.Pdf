using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using System;
using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// This class contains information about the document
    /// </summary>
    public class DocumentInformation : IPdfObject
    {
        private bool _isWritten;

        internal DocumentInformation(PdfReference id)
        {
            Reference = id;
        }

        /// <summary>
        /// The document's title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The name of the person who created the document
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// The subject of the document
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Keywords associated with the document
        /// </summary>
        public string? Keywords { get; set; }

        /// <summary>
        /// If the document was converted to PDF from another format, the name of the conforming product that created the original document from which it was converted. Otherwise the name of the application that created the document.
        /// </summary>
        public string? Creator { get; set; }

        /// <summary>
        /// If the document was converted to PDF from another format, the name of the conforming product that converted it to PDF.
        /// </summary>
        public string? Producer { get; set; }

        /// <summary>
        /// The date and time the document was created, in human-readable form.
        /// </summary>
        public DateTime? CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The date and time the document was most recently modified, in human-readable form.
        /// </summary>
        public DateTime? ModDate { get; set; }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        /// <summary>
        /// Extra information that will be added to the PDF meta data
        /// </summary>
        public IDictionary<string, string> ExtraInfo { get; } = new Dictionary<string, string>();

        internal uint WriteToStream(PdfStream stream)
        {
            if (_isWritten)
                throw new InvalidOperationException("Object is already written to stream.");

            var position = (uint)stream.Position;

            stream.IndirectDictionary(this, static (did, dictionary) =>
            {
                if (!string.IsNullOrWhiteSpace(did.Title))
                    dictionary.Write(PdfName.Get("Title"), _toPdfHexadecimalString(did.Title!));
                if (!string.IsNullOrWhiteSpace(did.Author))
                    dictionary.Write(PdfName.Get("Author"), _toPdfHexadecimalString(did.Author!));
                if (!string.IsNullOrWhiteSpace(did.Subject))
                    dictionary.Write(PdfName.Get("Subject"), _toPdfHexadecimalString(did.Subject!));
                if (!string.IsNullOrWhiteSpace(did.Keywords))
                    dictionary.Write(PdfName.Get("Keywords"), _toPdfHexadecimalString(did.Keywords!));
                if (!string.IsNullOrWhiteSpace(did.Creator))
                    dictionary.Write(PdfName.Get("Creator"), _toPdfHexadecimalString(did.Creator!));
                if (!string.IsNullOrWhiteSpace(did.Producer))
                    dictionary.Write(PdfName.Get("Producer"), _toPdfHexadecimalString(did.Producer!));
                if (did.CreationDate != null)
                    dictionary.Write(PdfName.Get("CreationDate"), _toPdfDate(did.CreationDate.Value));
                if (did.ModDate != null)
                    dictionary.Write(PdfName.Get("ModDate"), _toPdfDate(did.ModDate.Value));

                if(did.ExtraInfo.Count != 0)
                    foreach(var kv in did.ExtraInfo)
                        dictionary.Write(PdfName.Get(kv.Key), _toPdfHexadecimalString(kv.Value));
            });

            _isWritten = true;

            return position;
        }
        
        private static string _toPdfHexadecimalString(string input)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            var builder = new System.Text.StringBuilder((bytes.Length * 2) + 2);
            builder.Append('<');
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("X2"));
            }
            builder.Append('>');
            return builder.ToString();
        }

        private static string _toPdfDate(DateTimeOffset input)
        {
            var datePart = input.ToString("yyyyMMddHHmmss");

            var builder = new System.Text.StringBuilder(22);
            builder.Append("(D:");
            builder.Append(datePart);

            var hours = input.Offset.Hours;
            var minutes = input.Offset.Minutes;

            if (hours == 0 && minutes == 0)
            {
                builder.Append("Z00'00");
            }
            else
            {
                if (hours > 0 || (hours == 0 && minutes > 0))
                {
                    builder.Append('+');
                }
                else
                {
                    builder.Append('-');
                }
                builder.Append(Math.Abs(hours).ToString().PadLeft(2, '0'));
                builder.Append('\'');
                builder.Append(minutes.ToString().PadLeft(2, '0'));
            }
            builder.Append(')');

            return builder.ToString();
        }
    }
}
