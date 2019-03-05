# FileFormats.Pdf
[![Build Status](https://synercoding.visualstudio.com/Synercoding.FileFormats.Pdf/_apis/build/status/synercoder.FileFormats.Pdf?branchName=master)](https://synercoding.visualstudio.com/Synercoding.FileFormats.Pdf/_build/latest?definitionId=7&branchName=master)

This project was created to enable PDF creation on .NETStandard. This because multiple libraries did not suit my purpose of working on .NET Core & .NET Framework the same way. Some alternatives supported settings the different boxes (Media, Crop, Bleed & Trim) but did not fully supported images on all platforms. Others supported images but not the different boxes, and again others did not work at all on .NET Core.

Because of those reasons this libary was created. 

## License
This project is licensed under MIT license.

## Specifications used
This library was created using the specifications lay out in ["PDF 32000-1:2008, Document management – Portable document format – Part 1: PDF 1.7"](https://www.adobe.com/content/dam/acom/en/devnet/pdf/pdfs/PDF32000_2008.pdf).

The full specifications are not implemented. This library currently only supports placements of images and setting the different boxes.

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
    double _mmToPts(double mm) => mm / 25.4d * 72;

    writer
        .AddPage(page =&gt;
        {
            var bleed = _mmToPts(3);
            // Mediabox = A4 Portrait with 3mm bleed all around
            page.MediaBox = new Primitives.Rectangle(0, 0, _mmToPts(216), _mmToPts(303));
            page.TrimBox = new Primitives.Rectangle(
                page.MediaBox.LLX + bleed,
                page.MediaBox.LLY + bleed,
                page.MediaBox.URX - bleed,
                page.MediaBox.URY - bleed
            );

            using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
            {
                var scale = 3456d / 5184;

                var width = _mmToPts(100);
                var height = _mmToPts(100 * scale);

                var offSet = _mmToPts(6);
                page.AddImage(eyeStream, new Primitives.Rectangle(offSet, offSet, width + offSet, height + offSet));
            }
        });
}</code></pre>