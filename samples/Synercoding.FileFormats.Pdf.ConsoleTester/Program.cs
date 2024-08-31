//using Synercoding.FileFormats.Pdf.Extensions;
//using Synercoding.FileFormats.Pdf.LowLevel.Colors;
//using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
//using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf.ConsoleTester;

public class Program
{
    public static void Main(string[] args)
    {
        //var fileName = "out.pdf";

        //if (File.Exists(fileName))
        //    File.Delete(fileName);

        //using (var fs = File.OpenWrite(fileName))
        //using (var writer = new PdfWriter(fs))
        //{
        //    var bleed = Mm(3);
        //    var mediaBox = Sizes.A4.Expand(bleed).AsRectangle();
        //    var trimBox = mediaBox.Contract(bleed);

        //    writer
        //        // Set document info
        //        .SetDocumentInfo(info =>
        //        {
        //            info.Author = "Gerard Gunnewijk";
        //            info.Title = "Example 1";
        //            info.Creator = "Synercoding.FileFormats.Pdf";
        //            info.ExtraInfo.Add("CutContourProgramId", "cloud-shape");
        //        })
        //        // Add image to writer directly and then use that image in the page
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            using (var blurStream = File.OpenRead("Pexels_com/4k-wallpaper-blur-bokeh-1484253.jpg"))
        //            {
        //                var addedImage = writer.AddJpgUnsafe(blurStream, 7000, 4672, DeviceRGB.Instance);
        //                var scale = (double)addedImage.Width / addedImage.Height;
        //                page.Content.AddImage(addedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
        //            }
        //        })
        //        // Add text to page and use it as the clipping path
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            page.Content.WrapInState(content =>
        //            {
        //                content.AddText(textOp =>
        //                {
        //                    textOp.SetTextRenderingMode(TextRenderingMode.AddClippingPath)
        //                        .SetFontAndSize(StandardFonts.Helvetica, 160)
        //                        .SetTextLeading(500)
        //                        .MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(200).AsRaw(Unit.Points))
        //                        .ShowText("Clipped")
        //                        .SetFontAndSize(StandardFonts.HelveticaBold, 650)
        //                        .ShowTextOnNextLine("it!");
        //                });

        //                using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
        //                using (var forestImage = SixLabors.ImageSharp.Image.Load(forestStream))
        //                {
        //                    var scale = (double)forestImage.Width / forestImage.Height;

        //                    var matrix = Matrix.CreateScaleMatrix(new Value(scale * 303, Unit.Millimeters).AsRaw(Unit.Points), new Value(303, Unit.Millimeters).AsRaw(Unit.Points))
        //                        .Translate(new Value(-100, Unit.Millimeters).AsRaw(Unit.Points), new Value(0, Unit.Millimeters).AsRaw(Unit.Points));

        //                    page.Content.AddImage(forestImage, matrix);
        //                }
        //            });
        //        })
        //        // Test placement using rectangle
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            using (var barrenStream = File.OpenRead("Pexels_com/arid-barren-desert-1975514.jpg"))
        //            using (var barrenImage = SixLabors.ImageSharp.Image.Load(barrenStream))
        //            {
        //                var scale = (double)barrenImage.Width / barrenImage.Height;

        //                page.Content.AddImage(barrenImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
        //            }

        //            using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
        //            {
        //                var scale = 3456d / 5184;

        //                var width = 100;
        //                var height = 100 * scale;

        //                var offSet = 6;
        //                page.Content.AddImage(eyeStream, new Rectangle(offSet, offSet, width + offSet, height + offSet, Unit.Millimeters));
        //            }
        //        })
        //        // Test shape graphics
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            page.Content.AddShapes(ctx =>
        //            {
        //                ctx.SetMiterLimit(10)
        //                    .SetLineCap(LowLevel.Graphics.LineCapStyle.ButtCap)
        //                    .SetLineJoin(LowLevel.Graphics.LineJoinStyle.MiterJoin);

        //                ctx.Move(100, 100)
        //                    .LineTo(200, 100)
        //                    .LineTo(200, 200)
        //                    .LineTo(100, 200)
        //                    .SetLineWidth(5)
        //                    .SetStroke(PredefinedColors.Black)
        //                    .SetFill(PredefinedColors.Red)
        //                    .FillThenStroke(LowLevel.FillRule.NonZeroWindingNumber);

        //                ctx.Move(50, 50)
        //                    .LineTo(150, 50)
        //                    .LineTo(150, 150)
        //                    .LineTo(50, 150)
        //                    .SetLineWidth(1)
        //                    .SetFill(PredefinedColors.Blue)
        //                    .CloseSubPath()
        //                    .Fill(LowLevel.FillRule.NonZeroWindingNumber);

        //                ctx.Move(150, 150)
        //                    .LineTo(250, 150)
        //                    .LineTo(250, 250)
        //                    .LineTo(150, 250)
        //                    .SetLineWidth(3)
        //                    .SetStroke(PredefinedColors.Yellow)
        //                    .SetDashPattern(new LowLevel.Graphics.Dash() { Array = new[] { 5d } })
        //                    .CloseSubPath()
        //                    .Stroke();
        //            });
        //        })
        //        // Test pages with text
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            page.Content.AddText(ops =>
        //            {
        //                ops.MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(10).AsRaw(Unit.Points))
        //                   .SetFontAndSize(StandardFonts.Helvetica, 12)
        //                   .SetFill(PredefinedColors.Blue)
        //                   .ShowText("The quick brown fox jumps over the lazy dog.");
        //            });

        //            page.Content.AddText("Text with a newline" + Environment.NewLine + "in it.", StandardFonts.Helvetica, 12, new Point(Mm(10), Mm(20)));
        //        })
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            page.Content.AddText("This page also used Helvetica", StandardFonts.Helvetica, 32, textContext =>
        //            {
        //                textContext.MoveToStartNextLine(Mm(10).AsRaw(Unit.Points), Mm(10).AsRaw(Unit.Points))
        //                    .SetTextRenderingMode(TextRenderingMode.Stroke)
        //                    .SetStroke(PredefinedColors.Red);
        //            });
        //        })
        //        // Test placement using matrix
        //        .AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
        //            using (var forestImage = SixLabors.ImageSharp.Image.Load(forestStream))
        //            {
        //                var scale = (double)forestImage.Width / forestImage.Height;

        //                var matrix = Matrix.CreateScaleMatrix(new Value(scale * 303, Unit.Millimeters).AsRaw(Unit.Points), new Value(303, Unit.Millimeters).AsRaw(Unit.Points))
        //                    .Translate(new Value(-100, Unit.Millimeters).AsRaw(Unit.Points), new Value(0, Unit.Millimeters).AsRaw(Unit.Points));

        //                page.Content.AddImage(forestImage, matrix);
        //            }
        //        });

        //    using (var blurStream = File.OpenRead("Pexels_com/4k-wallpaper-blur-bokeh-1484253.jpg"))
        //    using (var blurImage = SixLabors.ImageSharp.Image.Load(blurStream))
        //    {
        //        var reusedImage = writer.AddImage(blurImage);

        //        for (int i = 0; i < 4; i++)
        //        {
        //            writer.AddPage(page =>
        //            {
        //                page.MediaBox = mediaBox;
        //                page.TrimBox = trimBox;

        //                var scale = (double)blurImage.Width / blurImage.Height;

        //                page.Content.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
        //            });
        //        }


        //        writer.AddPage(page =>
        //        {
        //            page.MediaBox = mediaBox;
        //            page.TrimBox = trimBox;

        //            var scale = (double)blurImage.Width / blurImage.Height;

        //            page.Content.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));

        //            page.Content.AddShapes(trimBox, static (trim, context) =>
        //            {
        //                context.SetStroke(new SpotColor(new Separation(LowLevel.PdfName.Get("CutContour"), PredefinedColors.Magenta), 1));
        //                context.Rectangle(trim);
        //                context.Stroke();
        //            });
        //        });
        //    }
        //}
    }
}
