# FileFormats.Pdf
[![NuGet][nuget-badge]][nuget] 

[nuget]: https://www.nuget.org/packages/Synercoding.FileFormats.Pdf/
[nuget-badge]: https://img.shields.io/nuget/v/Synercoding.FileFormats.Pdf.svg?label=Synercoding.FileFormats.Pdf


This project was created to enable PDF creation on .NETStandard 2.1, .NET 6 & .NET 7. This because multiple libraries did not suit my purpose of working on .NET Core & .NET Framework the same way. Some alternatives supported settings the different boxes (Media, Crop, Bleed & Trim) but did not fully supported images on all platforms. Others supported images but not the different boxes, and again others did not work at all on .NET Core.

Because of those reasons this libary was created. 

## License
This project is licensed under MIT license.

## Specifications used
This library was created using the specifications lay out in ["PDF 32000-1:2008, Document management – Portable document format – Part 1: PDF 1.7"](https://www.adobe.com/content/dam/acom/en/devnet/pdf/pdfs/PDF32000_2008.pdf).

The full specifications are not implemented. 
This library currently only supports:
 - Images
   - *No transparency support at the moment*
 - Text
   - *Only the 14 standard fonts are available*
     - *See "9.6.2.2 Standard Type 1 Fonts (Standard 14 Fonts)" in the specifications.*
 - Shapes
 - Setting page boxes (Media, Crop, Bleed, Trim & Art)
 - Color model limited to:
   - DeviceGray
   - DeviceRGB
   - DeviceCMYK
   - Separations/Spotcolor (with a linearized type 2 method to portay the tint)

## Remarks
Unlike most PDF libraries this library does not create the entire PDF model in memory before writing the PDF to a (file)stream. Most libaries support editing capabilities, because this libary only supports creating files, it was not necessary to keep the PDF model in memory. This results in less memory usage.

To place the images this library makes use of [SixLabors/ImageSharp](https://github.com/SixLabors/ImageSharp). All images that are placed in the PDF will be saved internally as a JPG file. This means that this library currently does **NOT** support transparency.

### Output pdfs
The PDF files created in this library are not fully PDF 1.7 compliant, because the standard 14 fonts are partially written to the Font Dictionary, The **FirstChar**, **LastChar**. **Widths** and **FontDescriptor** values are omitted, which is was acceptable prior to PDF 1.5. This special treatment of the 14 standard fonts was deprecated in PDF 1.5. The PDF's that are generated will work in conforming readers because as the specification states: *"For backwards capability, conforming readers shall still provide the special treatment identified for the standard 14 fonts."*.

This shortcoming shall be remedied when broader font support is implemented.

### Sample program images
The sample project called *Synercoding.FileFormats.Pdf.ConsoleTester* uses multiple images. 
Those images were taken from:
- [Pexels.com](https://www.pexels.com/royalty-free-images/) and are licensed under the [Pexels License](https://www.pexels.com/photo-license/)
- [FreePngImg.com](https://freepngimg.com/png/59872-jaguar-panther-royalty-free-cougar-black-cheetah) and are licensed under [Creative Commons (CC BY-NC 4.0)](https://creativecommons.org/licenses/by-nc/4.0/)

## Sample usage

<pre><code>using (var fs = File.OpenWrite(fileName))
using (var writer = new PdfWriter(fs))
{
    writer
        .AddPage(page =>
        {
            var bleed = Mm(3);
            page.MediaBox = Sizes.A4.Expand(bleed).AsRectangle();
            page.TrimBox = page.MediaBox.Contract(bleed);
            
            using (var barrenStream = File.OpenRead("Pexels_com/arid-barren-desert-1975514.jpg"))
            using (var barrenImage = SixLabors.ImageSharp.Image.Load(barrenStream))
            {
                var scale = (double)barrenImage.Width / barrenImage.Height;

                page.Content.AddImage(barrenImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
            }
        })
}</code></pre>
