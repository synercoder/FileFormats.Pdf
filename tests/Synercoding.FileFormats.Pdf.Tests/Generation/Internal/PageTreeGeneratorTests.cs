using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class PageTreeGeneratorTests
{
    [Fact]
    public void Test_Create_SinglePage()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var page = _createTestPage();
        var pages = new[] { page };

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        Assert.Equal(2, nodes.Length); // 1 page + 1 root

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Single(rootKids);
        Assert.Equal(new PdfNumber(1), rootNode.Value[PdfNames.Count]);

        // Verify page node
        var pageNode = nodes.First();
        Assert.Same(page, pageNode.Value);
        Assert.Equal(rootNode.Id.GetReference(), pageNode.Value[PdfNames.Parent]);

        // Verify root reference
        Assert.Equal(rootNode.Id, root.Id);
    }

    [Fact]
    public void Test_Create_MultiplePagesUnder15()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pageCount = 10;
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        Assert.Equal(pageCount + 1, nodes.Length); // 10 pages + 1 root

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Equal(pageCount, rootKids.Count);
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);

        // Verify all page nodes have parent reference to root
        for (int i = 0; i < pageCount; i++)
        {
            var pageNode = nodes[i];
            Assert.Equal(rootNode.Id.GetReference(), pageNode.Value[PdfNames.Parent]);
        }
    }

    [Fact]
    public void Test_Create_ExactlyAt15Pages()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pageCount = 15;
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        Assert.Equal(pageCount + 1, nodes.Length); // 15 pages + 1 root

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Equal(pageCount, rootKids.Count);
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);
    }

    [Fact]
    public void Test_Create_MultiLevelTreeWith16Pages()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pageCount = 16;
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        // 16 pages will be chunked into 2 intermediate nodes (10 + 6)
        // Total: 16 pages + 2 intermediate + 1 root = 19
        Assert.Equal(19, nodes.Length);

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Equal(2, rootKids.Count); // 2 intermediate nodes
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);

        // Verify intermediate nodes
        var intermediateNodes = nodes.Skip(pageCount).Take(2).ToArray();
        Assert.Equal(new PdfNumber(10), intermediateNodes[0].Value[PdfNames.Count]);
        Assert.Equal(new PdfNumber(6), intermediateNodes[1].Value[PdfNames.Count]);

        // Verify all intermediate nodes have parent reference to root
        foreach (var intermediateNode in intermediateNodes)
        {
            Assert.Equal(PdfNames.Pages, intermediateNode.Value[PdfNames.Type]);
            Assert.Equal(rootNode.Id.GetReference(), intermediateNode.Value[PdfNames.Parent]);
        }
    }

    [Fact]
    public void Test_Create_LargePageTree()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pageCount = 100;
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        // 100 pages will be chunked into 10 intermediate nodes (10 pages each)
        // Total: 100 pages + 10 intermediate + 1 root = 111
        Assert.Equal(111, nodes.Length);

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Equal(10, rootKids.Count); // 10 intermediate nodes
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);
    }

    [Fact]
    public void Test_Create_VeryLargePageTreeRequiresMultipleLevels()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pageCount = 200; // This will require multiple levels of intermediate nodes
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        // 200 pages -> 20 intermediate nodes -> 2 second-level intermediate nodes -> 1 root
        // Total: 200 + 20 + 2 + 1 = 223
        Assert.Equal(223, nodes.Length);

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);

        // Verify hierarchical structure
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Equal(2, rootKids.Count); // 2 second-level intermediate nodes
    }

    [Fact]
    public void Test_Create_EmptyPageList()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pages = Array.Empty<PdfDictionary>();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        Assert.Single(nodes); // Only root node

        var rootNode = nodes[0];
        Assert.Equal(PdfNames.Pages, rootNode.Value[PdfNames.Type]);
        var rootKids = (PdfArray)rootNode.Value[PdfNames.Kids]!;
        Assert.Empty(rootKids);
        Assert.Equal(new PdfNumber(0), rootNode.Value[PdfNames.Count]);
    }

    [Fact]
    public void Test_Create_IntermediateNodesWithExistingCount()
    {
        // Arrange
        var tableBuilder = new TableBuilder();

        // Create a pages node that already has children (simulating an existing page tree)
        var existingPagesNode = new PdfDictionary
        {
            [PdfNames.Type] = PdfNames.Pages,
            [PdfNames.Count] = new PdfNumber(5), // This node represents 5 pages
            [PdfNames.Kids] = new PdfArray() // Would contain references to 5 pages
        };

        var pages = new[] { existingPagesNode };

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        Assert.Equal(2, nodes.Length); // 1 existing pages node + 1 root

        // Verify root node
        var rootNode = nodes.Last();
        Assert.Equal(new PdfNumber(5), rootNode.Value[PdfNames.Count]); // Should sum the count from child
    }

    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(150)]
    public void Test_Create_CorrectPageCountPropagation(int pageCount)
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pages = Enumerable.Range(0, pageCount).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        var rootNode = nodes.Last();
        Assert.Equal(new PdfNumber(pageCount), rootNode.Value[PdfNames.Count]);
    }

    [Fact]
    public void Test_Create_AllNodesHaveUniqueIds()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var pages = Enumerable.Range(0, 20).Select(_ => _createTestPage()).ToArray();

        // Act
        var (nodes, root) = PageTreeGenerator.Create(tableBuilder, pages);

        // Assert
        var allIds = nodes.Select(n => n.Id).ToHashSet();
        Assert.Equal(nodes.Length, allIds.Count); // All IDs should be unique
    }

    private static PdfDictionary _createTestPage()
    {
        return new PdfDictionary
        {
            [PdfNames.Type] = PdfNames.Page,
            [PdfNames.MediaBox] = new PdfArray(
                new PdfNumber(0),
                new PdfNumber(0),
                new PdfNumber(612),
                new PdfNumber(792)
            )
        };
    }
}
