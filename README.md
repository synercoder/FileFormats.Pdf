# FileFormats.Pdf
[![NuGet][nuget-badge]][nuget] 

[nuget]: https://www.nuget.org/packages/Synercoding.FileFormats.Pdf/
[nuget-badge]: https://img.shields.io/nuget/v/Synercoding.FileFormats.Pdf.svg?label=Synercoding.FileFormats.Pdf


This project was created to enable PDF creation on .NETStandard. This because multiple libraries did not suit my purpose of working on .NET Core & .NET Framework the same way. Some alternatives supported settings the different boxes (Media, Crop, Bleed & Trim) but did not fully supported images on all platforms. Others supported images but not the different boxes, and again others did not work at all on .NET Core.

Because of those reasons this libary was created. 

## License
This project is licensed under MIT license.

## Specifications used
This library was created using the specifications lay out in ["PDF 32000-1:2008, Document management – Portable document format – Part 1: PDF 1.7"](https://www.adobe.com/content/dam/acom/en/devnet/pdf/pdfs/PDF32000_2008.pdf).

The full specifications are not implemented. This library currently only supports placements of images, drawing of vector shapes (CMYK, RGB & gray scale), and setting the different boxes.

## Remarks
Unlike most PDF libraries this library does not create the entire PDF model in memory before writing the PDF to a (file)stream. Most libaries support editing capabilities, because this libary only supports creating files, it was not necessary to keep the PDF model in memory. This results in less memory usage.

To place the images this library makes use of [SixLabors/ImageSharp](https://github.com/SixLabors/ImageSharp). All images that are placed in the PDF will be saved internally as a JPG file. This means that this library currently does **NOT** support transparency.

### Output pdfs
The PDF files created in this library are PDF 1.7 compliant.


### Sample program images
The sample project called *Synercoding.FileFormats.Pdf.ConsoleTester* uses multiple images. Those images were taken from [Pexels.com](https://www.pexels.com/royalty-free-images/) and are licensed under the [Pexels License](https://www.pexels.com/photo-license/).

## Sample usage

<pre><code>using (var fs = File.OpenWrite(fileName))
using (var writer = new PdfWriter(fs))
{
    writer
        .AddPage(page =&gt;
        {
            var bleed = new Spacing(3, Unit.Millimeters);
            page.MediaBox = Sizes.A4Portrait.Expand(bleed).AsRectangle();
            page.TrimBox = page.MediaBox.Contract(bleed);

            using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
            {
                var scale = 3456d / 5184;

                var width = 100;
                var height = 100 * scale;

                var offSet = 6;
                page.AddImage(eyeStream, new Rectangle(offSet, offSet, width + offSet, height + offSet, Unit.Millimeters));
            }
        });
}</code></pre>