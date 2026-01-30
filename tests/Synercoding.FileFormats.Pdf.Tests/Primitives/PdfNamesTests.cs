using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfNamesTests
{
    [Fact]
    public void Test_EnsureAllNameProperties_EqualDisplayValue()
    {
        string[] testExclusions = [
            "IdentityH"
        ];

        var properties = typeof(PdfNames)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(PdfName))
            .Select(p => new
            {
                PdfName = p.GetValue(null) as PdfName,
                Propertyname = p.Name
            })
            .Where(v => v.PdfName is not null)
            .Where(v => testExclusions.Contains(v.Propertyname) == false)
            .Select(v => new
            {
                PdfName = v.PdfName!.Display,
                Propertyname = v.Propertyname
            })
            .ToArray();

        foreach (var property in properties)
            Assert.Equal(property.Propertyname, property.PdfName);
    }
}
