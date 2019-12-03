namespace Synercoding.FileFormats.Pdf.PdfInternals
{
    public struct PdfReference
    {
        public PdfReference(int objectId)
            : this(objectId, 0)
        { }

        public PdfReference(int objectId, int generation)
        {
            ObjectId = objectId;
            Generation = generation;
        }

        public int ObjectId { get; }
        public int Generation { get; }
    }
}
