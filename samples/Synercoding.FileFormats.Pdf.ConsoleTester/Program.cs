using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Colors;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Content.Extensions;
using Synercoding.FileFormats.Pdf.Content.Text;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Generation.Extensions;
using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.ConsoleTester;

public class Program
{
    public static void Main(string[] args)
    {
        _writePdf("test.pdf");
        _writePdf("test-no-subset.pdf", enableSubsetting: false);
        _writeTextOnlyPdf("text-only.pdf");
        _writeTextOnlyPdf("text-only-no-subset.pdf", enableSubsetting: false);
    }

    private static void _writePdf(string fileName, bool enableSubsetting = true)
    {
        File.Delete(fileName);
        using (var fs = File.OpenWrite(fileName))
        using (var writer = new PdfWriter(fs, new WriterSettings()
        {
            ContentStreamFilters = Array.Empty<IStreamFilter>(),
            EnableSubsetting = enableSubsetting
        }))
        {
            var bleed = Mm(3);
            var mediaBox = Sizes.A4.Expand(bleed).AsRectangle();
            var trimBox = mediaBox.Contract(bleed);

            writer.PageLayout = PageLayout.SinglePage;

            var font = Font.Load("Fonts/JuliettRegular-7OXnA.ttf");

            writer
                // Set document info
                .SetDocumentInfo(info =>
                {
                    info.Author = "Gerard Gunnewijk";
                    info.Title = "Example 1";
                    info.Creator = "Synercoding.FileFormats.Pdf";
                    info.ExtraInfo.Add("CutContourProgramId", "cloud-shape");
                })
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    using (page.Content.WrapInState())
                    {
                        string text = "Hello world!";
                        double fontSize = 80;

                        var textSize = font.GetBoundingBox(text, fontSize);
                        var matrix = Matrix.Identity
                            .Translate(
                                x: ( ( mediaBox.Width / 2 ) - ( textSize.BoundingBox.Width / 2 ) ).AsRaw(Unit.Points),
                                y: ( ( mediaBox.Height / 2 ) - ( textSize.BoundingBox.Height / 2 ) ).AsRaw(Unit.Points)
                            );
                        page.Content.ConcatenateMatrix(matrix);
                        page.Content.AddText(text, font, fontSize);
                    }

                    using (page.Content.WrapInState())
                    {
                        string[] lines = [
                            "This text is centered.",
                            "Because text bounding box calculations are now supported by the font implementation."
                        ];
                        double fontSize = 14;

                        var yPos = ( mediaBox.Height / 2 ) - Mm(20);
                        foreach (var line in lines)
                        {
                            using (page.Content.WrapInState())
                            {
                                var textSize = font.GetBoundingBox(line, fontSize);
                                var matrix = Matrix.Identity
                                    .Translate(
                                        x: ( ( mediaBox.Width / 2 ) - ( textSize.BoundingBox.Width / 2 ) ).AsRaw(Unit.Points),
                                        y: yPos.AsRaw(Unit.Points)
                                    );
                                page.Content.ConcatenateMatrix(matrix);
                                page.Content.AddText(line, font, fontSize);
                                yPos -= Pts(( font.Ascent + font.LineGap ) / font.UnitsPerEm * fontSize);
                            }
                        }
                    }
                })
                // Add image to writer directly and then use that image in the page
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    using (var blurStream = File.OpenRead("Pexels_com/4k-wallpaper-blur-bokeh-1484253.jpg"))
                    {
                        var addedImage = writer.AddJpgUnsafe(blurStream, 7000, 4672, DeviceRGB.Instance);
                        var scale = (double)addedImage.Width / addedImage.Height;
                        page.Content.AddImage(addedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
                    }
                })
                // Add text to page and use it as the clipping path
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    page.Content.WrapInState(content =>
                    {
                        content.AddText(textOp =>
                        {
                            textOp.SetTextRenderingMode(TextRenderingMode.AddClippingPath)
                                .SetFontAndSize(font, 160)
                                .SetTextLeading(425)
                                .MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(170).AsRaw(Unit.Points))
                                .ShowText("Clipped")
                                .SetFontAndSize(font, 650)
                                .ShowTextOnNextLine("it!");
                        });

                        using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
                        using (var forestImage = SixLabors.ImageSharp.Image.Load<Rgba32>(forestStream))
                        {
                            var scale = (double)forestImage.Width / forestImage.Height;

                            var matrix = Matrix.CreateScaleMatrix(new Value(scale * 303, Unit.Millimeters).AsRaw(Unit.Points), new Value(303, Unit.Millimeters).AsRaw(Unit.Points))
                                .Translate(new Value(-100, Unit.Millimeters).AsRaw(Unit.Points), new Value(0, Unit.Millimeters).AsRaw(Unit.Points));

                            page.Content.AddImage(forestImage, matrix);
                        }
                    });
                })
                // Test placement using rectangle
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    using (var barrenStream = File.OpenRead("Pexels_com/arid-barren-desert-1975514.jpg"))
                    using (var barrenImage = SixLabors.ImageSharp.Image.Load<Rgba32>(barrenStream))
                    {
                        var scale = (double)barrenImage.Width / barrenImage.Height;

                        page.Content.AddImage(barrenImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
                    }

                    using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
                    {
                        var scale = 3456d / 5184;

                        var width = 100;
                        var height = 100 * scale;

                        var offSet = 6;
                        page.Content.AddImage(eyeStream, new Rectangle(offSet, offSet, width + offSet, height + offSet, Unit.Millimeters));
                    }
                })
                // Test shape graphics
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    page.Content.AddShapes(ctx =>
                    {
                        ctx.SetMiterLimit(10)
                            .SetLineCap(LineCapStyle.ButtCap)
                            .SetLineJoin(LineJoinStyle.MiterJoin);

                        ctx.Move(100, 100)
                            .LineTo(200, 100)
                            .LineTo(200, 200)
                            .LineTo(100, 200)
                            .SetLineWidth(5)
                            .SetStroke(PredefinedColors.Black)
                            .SetFill(PredefinedColors.Red)
                            .FillThenStroke(FillRule.NonZeroWindingNumber);

                        ctx.Move(50, 50)
                            .LineTo(150, 50)
                            .LineTo(150, 150)
                            .LineTo(50, 150)
                            .SetLineWidth(1)
                            .SetFill(PredefinedColors.Blue)
                            .CloseSubPath()
                            .Fill(FillRule.NonZeroWindingNumber);

                        ctx.Move(150, 150)
                            .LineTo(250, 150)
                            .LineTo(250, 250)
                            .LineTo(150, 250)
                            .SetLineWidth(3)
                            .SetStroke(PredefinedColors.Yellow)
                            .SetDashPattern(new Dash() { Array = new[] { 5d } })
                            .CloseSubPath()
                            .Stroke();
                    });
                })
                // Test pages with text
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    page.Content.AddText(ops =>
                    {
                        ops.MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(10).AsRaw(Unit.Points))
                           .SetFontAndSize(font, 12)
                           .SetFill(PredefinedColors.Blue)
                           .ShowText("The quick brown fox jumps over the lazy dog.");
                    });

                    page.Content.AddText("Text with a newline" + Environment.NewLine + "in it.", font, 12, new Point(Mm(10), Mm(20)));
                })
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    page.Content.AddText("This page also has text in it.", font, 32, textContext =>
                    {
                        textContext.MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(10).AsRaw(Unit.Points))
                            .SetTextRenderingMode(TextRenderingMode.Stroke)
                            .SetStroke(PredefinedColors.Red);
                    });
                })
                // Test placement using matrix
                .AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
                    using (var forestImage = SixLabors.ImageSharp.Image.Load<Rgba32>(forestStream))
                    {
                        var scale = (double)forestImage.Width / forestImage.Height;

                        var matrix = Matrix.CreateScaleMatrix(new Value(scale * 303, Unit.Millimeters).AsRaw(Unit.Points), new Value(303, Unit.Millimeters).AsRaw(Unit.Points))
                            .Translate(new Value(-100, Unit.Millimeters).AsRaw(Unit.Points), new Value(0, Unit.Millimeters).AsRaw(Unit.Points));

                        page.Content.AddImage(forestImage, matrix);
                    }
                });

            using (var blurStream = File.OpenRead("Pexels_com/4k-wallpaper-blur-bokeh-1484253.jpg"))
            using (var blurImage = SixLabors.ImageSharp.Image.Load<Rgba32>(blurStream))
            {
                var reusedImage = writer.AddImage(blurImage);

                for (int i = 0; i < 4; i++)
                {
                    writer.AddPage(page =>
                    {
                        page.MediaBox = mediaBox;
                        page.TrimBox = trimBox;

                        var scale = (double)blurImage.Width / blurImage.Height;

                        page.Content.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
                    });
                }


                writer.AddPage(page =>
                {
                    page.MediaBox = mediaBox;
                    page.TrimBox = trimBox;

                    var scale = (double)blurImage.Width / blurImage.Height;

                    page.Content.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));

                    page.Content.AddShapes(trimBox, static (trim, context) =>
                    {
                        context.SetExtendedGraphicsState(new ExtendedGraphicsState()
                        {
                            Overprint = true
                        });
                        context.SetStroke(new SpotColor(new Separation(PdfName.Get("CutContour"), PredefinedColors.Magenta), 1));
                        context.Rectangle(trim);
                        context.Stroke();
                    });
                });

                using (var pantherPngStream = File.OpenRead("FreePngImage_com/59872-jaguar-panther-royalty-free-cougar-black-cheetah.png"))
                using (var pantherSixImage = SixLabors.ImageSharp.Image.Load<Rgba32>(pantherPngStream))
                {
                    var pantherImg = writer.AddImage(pantherSixImage);
                    var transparentPanther = writer.AddSeparationImage(pantherSixImage, new Separation(PdfName.Get("White"), PredefinedColors.Yellow), GrayScaleMethod.AlphaChannel, [0, 1]);

                    writer.AddPage(page =>
                    {
                        page.MediaBox = mediaBox;
                        page.TrimBox = trimBox;

                        var scale = (double)blurImage.Width / blurImage.Height;
                        page.Content.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));

                        scale = (double)transparentPanther.Width / transparentPanther.Height;
                        var pantherSize = new Rectangle(0, 0, 216, 216 / scale, Unit.Millimeters);

                        page.Content.AddImage(pantherImg, pantherSize);

                        page.Content.WrapInState(pantherSixImage, (image, content) =>
                        {
                            content.SetExtendedGraphicsState(new ExtendedGraphicsState()
                            {
                                Overprint = true
                            });
                            content.AddImage(transparentPanther, pantherSize);
                        });
                    });
                }
            }
        }
    }

    private static void _writeTextOnlyPdf(string fileName, bool enableSubsetting = true)
    {
        File.Delete(fileName);
        using (var fs = File.OpenWrite(fileName))
        using (var writer = new PdfWriter(fs, new WriterSettings()
        {
            ContentStreamFilters = Array.Empty<IStreamFilter>(),
            EnableSubsetting = enableSubsetting
        }))
        {
            var font = Font.Load("Fonts/JuliettRegular-7OXnA.ttf");

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A6.Rotated.AsRectangle();

                // Use a limited character set to make subsetting more effective
                page.Content.AddText("The quick brown fox jumps over the lazy dog.", font, 18, new Point(Mm(5), Mm(20)));
                page.Content.AddText("0123456789.,!@#$%^&*()-=[];'<>?_+", font, 18, new Point(Mm(5), Mm(10)));
            });
        }
    }
}
