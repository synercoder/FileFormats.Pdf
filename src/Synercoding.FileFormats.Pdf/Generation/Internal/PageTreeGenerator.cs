using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal static class PageTreeGenerator
{
    public static (PdfObject<PdfDictionary>[] Nodes, PdfReference Root) Create(TableBuilder tableBuilder, IEnumerable<PdfDictionary> pages)
    {
        var objectsToWrite = new List<PdfObject<PdfDictionary>>();

        foreach (var page in pages)
        {
            objectsToWrite.Add(new PdfObject<PdfDictionary>()
            {
                Id = tableBuilder.ReserveId(),
                Value = page
            });
        }

        var pageTreeNodes = objectsToWrite.ToArray();
        while (pageTreeNodes.Length > 15)
        {
            var chunked = pageTreeNodes
                .Chunk(10)
                .Select(c => new
                {
                    NodeId = tableBuilder.ReserveId(),
                    Kids = c.ToArray(),
                    Count = c.Sum(n => n.Value.TryGetValue<PdfNumber>(PdfNames.Count, out var count) ? count.LongValue : 1)
                })
                .ToArray();
            foreach (var chunk in chunked)
            {
                foreach (var kid in chunk.Kids)
                {
                    kid.Value[PdfNames.Parent] = chunk.NodeId.GetReference();
                }
            }

            pageTreeNodes = chunked.Select(c => new PdfObject<PdfDictionary>()
            {
                Id = c.NodeId,
                Value = new PdfDictionary()
                {
                    [PdfNames.Type] = PdfNames.Pages,
                    [PdfNames.Kids] = new PdfArray(c.Kids.Select(k => k.Id.GetReference()).Cast<IPdfPrimitive>().ToArray()),
                    [PdfNames.Count] = new PdfNumber(c.Count)
                }
            })
                .ToArray();

            objectsToWrite.AddRange(pageTreeNodes);
        }

        var pageCount = pageTreeNodes.Sum(n => n.Value.TryGetValue<PdfNumber>(PdfNames.Count, out var count) ? count.LongValue : 1);
        var kids = pageTreeNodes
            .Select(n => n.Id.GetReference() as IPdfPrimitive)
            .ToArray();
        var rootNode = new PdfObject<PdfDictionary>()
        {
            Id = tableBuilder.ReserveId(),
            Value = new PdfDictionary()
            {
                [PdfNames.Type] = PdfNames.Pages,
                [PdfNames.Kids] = new PdfArray(kids),
                [PdfNames.Count] = new PdfNumber(pageCount)
            }
        };
        foreach (var node in pageTreeNodes)
            node.Value[PdfNames.Parent] = rootNode.Id.GetReference();

        objectsToWrite.Add(rootNode);

        return (objectsToWrite.ToArray(), new PdfReference(rootNode.Id));
    }
}
