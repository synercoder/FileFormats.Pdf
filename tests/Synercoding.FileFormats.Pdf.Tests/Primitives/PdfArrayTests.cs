using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfArrayTests
{
    [Fact]
    public void Test_Constructor_Empty_CreatesEmptyArray()
    {
        var array = new PdfArray();
        
        Assert.Empty(array);
    }

    [Fact]
    public void Test_Constructor_WithItems_CreatesArrayWithItems()
    {
        var items = new IPdfPrimitive[] { new PdfNumber(1), new PdfNumber(2), new PdfNumber(3) };
        var array = new PdfArray(items);
        
        Assert.Equal(3, array.Count);
        Assert.Equal(1, ((PdfNumber)array[0]).Value);
        Assert.Equal(2, ((PdfNumber)array[1]).Value);
        Assert.Equal(3, ((PdfNumber)array[2]).Value);
    }

    [Fact]
    public void Test_Indexer_Get_ReturnsCorrectItem()
    {
        var items = new IPdfPrimitive[] { new PdfNumber(42) };
        var array = new PdfArray(items);
        
        var result = array[0];
        
        Assert.IsType<PdfNumber>(result);
        Assert.Equal(42, ((PdfNumber)result).Value);
    }

    [Fact]
    public void Test_Indexer_Set_UpdatesItem()
    {
        var array = new PdfArray(new IPdfPrimitive[] { new PdfNumber(1) });
        var newItem = new PdfNumber(42);
        
        array[0] = newItem;
        
        Assert.Equal(42, ((PdfNumber)array[0]).Value);
    }

    [Fact]
    public void Test_Add_IncreasesCount()
    {
        var array = new PdfArray();
        var item = new PdfNumber(42);
        
        array.Add(item);
        
        Assert.Single(array);
        Assert.Equal(42, ((PdfNumber)array[0]).Value);
    }

    [Fact]
    public void Test_Insert_InsertsAtCorrectPosition()
    {
        var array = new PdfArray(new IPdfPrimitive[] { new PdfNumber(1), new PdfNumber(3) });
        var item = new PdfNumber(2);
        
        array.Insert(1, item);
        
        Assert.Equal(3, array.Count);
        Assert.Equal(1, ((PdfNumber)array[0]).Value);
        Assert.Equal(2, ((PdfNumber)array[1]).Value);
        Assert.Equal(3, ((PdfNumber)array[2]).Value);
    }

    [Fact]
    public void Test_Remove_RemovesItem()
    {
        var item1 = new PdfNumber(1);
        var item2 = new PdfNumber(2);
        var array = new PdfArray(new IPdfPrimitive[] { item1, item2 });
        
        bool removed = array.Remove(item1);
        
        Assert.True(removed);
        Assert.Single(array);
        Assert.Equal(2, ((PdfNumber)array[0]).Value);
    }

    [Fact]
    public void Test_Remove_NonExistentItem_ReturnsFalse()
    {
        var array = new PdfArray(new IPdfPrimitive[] { new PdfNumber(1) });
        var nonExistentItem = new PdfNumber(2);
        
        bool removed = array.Remove(nonExistentItem);
        
        Assert.False(removed);
        Assert.Single(array);
    }

    [Fact]
    public void Test_RemoveAt_RemovesItemAtIndex()
    {
        var array = new PdfArray(new IPdfPrimitive[] { new PdfNumber(1), new PdfNumber(2), new PdfNumber(3) });
        
        array.RemoveAt(1);
        
        Assert.Equal(2, array.Count);
        Assert.Equal(1, ((PdfNumber)array[0]).Value);
        Assert.Equal(3, ((PdfNumber)array[1]).Value);
    }

    [Fact]
    public void Test_Clear_RemovesAllItems()
    {
        var array = new PdfArray(new IPdfPrimitive[] { new PdfNumber(1), new PdfNumber(2) });
        
        array.Clear();
        
        Assert.Empty(array);
    }

    [Fact]
    public void Test_GetEnumerator_IteratesAllItems()
    {
        var items = new IPdfPrimitive[] { new PdfNumber(1), new PdfNumber(2), new PdfNumber(3) };
        var array = new PdfArray(items);
        var results = new List<long>();
        
        foreach (var item in array)
        {
            results.Add((PdfNumber)item);
        }
        
        Assert.Equal(new[] { 1L, 2L, 3L }, results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Test_Count_ReturnsCorrectCount(int itemCount)
    {
        var items = Enumerable.Range(1, itemCount).Select(i => new PdfNumber(i)).Cast<IPdfPrimitive>().ToArray();
        var array = new PdfArray(items);
        
        Assert.Equal(itemCount, array.Count);
    }

    [Fact]
    public void Test_Array_WithMixedTypes_HandlesCorrectly()
    {
        var items = new IPdfPrimitive[] 
        { 
            new PdfNumber(42), 
            new PdfNumber(3.14),
            PdfBoolean.True,
            new PdfString("test", PdfStringEncoding.PdfDocEncoding, false)
        };
        var array = new PdfArray(items);
        
        Assert.Equal(4, array.Count);
        Assert.IsType<PdfNumber>(array[0]);
        Assert.IsType<PdfNumber>(array[1]);
        Assert.IsType<PdfBoolean>(array[2]);
        Assert.IsType<PdfString>(array[3]);
    }

    [Fact]
    public void Test_Array_LargeData_HandlesCorrectly()
    {
        var items = Enumerable.Range(1, 10000).Select(i => new PdfNumber(i)).Cast<IPdfPrimitive>().ToArray();
        var array = new PdfArray(items);
        
        Assert.Equal(10000, array.Count);
        Assert.Equal(1, ((PdfNumber)array[0]).Value);
        Assert.Equal(10000, ((PdfNumber)array[9999]).Value);
    }
}
