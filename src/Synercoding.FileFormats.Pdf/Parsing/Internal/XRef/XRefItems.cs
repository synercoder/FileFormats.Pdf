using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal abstract record class XRefItem(PdfObjectId Id, bool Free);
internal sealed record class FreeXRefItem(PdfObjectId Id) : XRefItem(Id, true);
internal sealed record class ClassicXRefItem(PdfObjectId Id, long Offset) : XRefItem(Id, false);
internal sealed record class CompressedXRefItem(PdfObjectId Id, PdfObjectId ObjectStreamId, int ObjectIndex) : XRefItem(Id, false);
