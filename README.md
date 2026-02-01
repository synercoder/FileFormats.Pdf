# FileFormats.Pdf
[![NuGet][nuget-badge]][nuget] 

[nuget]: https://www.nuget.org/packages/Synercoding.FileFormats.Pdf/
[nuget-badge]: https://img.shields.io/nuget/v/Synercoding.FileFormats.Pdf.svg?label=Synercoding.FileFormats.Pdf


This project was created to enable PDF creation on .NET. Multiple libraries did not suit my purpose of working on .NET Core & .NET Framework the same way. Some alternatives supported settings the different boxes (Media, Crop, Bleed & Trim) but did not fully support images on all platforms. Others supported images but not the different boxes, and again others did not work at all on .NET Core.

Because of those reasons this library was created. Since then this project has evolved to keep current with the latest versions. Currently targeting .NET 8.0 and .NET 10.0.

## License
This project is licensed under a dual license. See [LICENSE](LICENSE) for full information. It comes down to: Apache License 2.0 for small and non-profit companies, or contact me for other options.

## Specifications used
This library was created using the specifications lay out in ["ISO 32000-2:2020 (PDF 2.0)"](https://pdfa.org/resource/iso-32000-2/).

## Remarks
Unlike most PDF libraries this library does not create the entire PDF model in memory before writing the PDF to a (file)stream.

To place the images this library makes use of [SixLabors/ImageSharp](https://github.com/SixLabors/ImageSharp). All images that are placed in the PDF will be saved internally as a JPG file. Except for transparent images, where the color data is saved in jpg format, and the alpha channel is saved as color data compressed with flate encoding.

### Sample program images
The sample project called *Synercoding.FileFormats.Pdf.ConsoleTester* uses multiple images. 
Those images were taken from:
- [Pexels.com](https://www.pexels.com/royalty-free-images/) and are licensed under the [Pexels License](https://www.pexels.com/photo-license/)
- [FreePngImg.com](https://freepngimg.com/png/59872-jaguar-panther-royalty-free-cougar-black-cheetah) and are licensed under [Creative Commons (CC BY-NC 4.0)](https://creativecommons.org/licenses/by-nc/4.0/)
