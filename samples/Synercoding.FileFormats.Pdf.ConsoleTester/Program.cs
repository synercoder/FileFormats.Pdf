using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System.IO;

namespace Synercoding.FileFormats.Pdf.ConsoleTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fileName = "out.pdf";

            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var fs = File.OpenWrite(fileName))
            using (var writer = new PdfWriter(fs))
            {
                var bleed = new Spacing(3, Unit.Millimeters);
                var mediaBox = Sizes.A4.Expand(bleed).AsRectangle();
                var trimBox = mediaBox.Contract(bleed);

                writer
                    // Set document info
                    .SetDocumentInfo(info =>
                    {
                        info.Author = "Gerard Gunnewijk";
                        info.Title = "Example 1";
                    })
                    // Test placement using rectangle
                    .AddPage(page =>
                    {
                        page.MediaBox = mediaBox;
                        page.TrimBox = trimBox;

                        using (var barrenStream = File.OpenRead("Pexels_com/arid-barren-desert-1975514.jpg"))
                        using (var barrenImage = SixLabors.ImageSharp.Image.Load(barrenStream))
                        {
                            var scale = (double)barrenImage.Width / barrenImage.Height;

                            page.AddImage(barrenImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
                        }

                        using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
                        {
                            var scale = 3456d / 5184;

                            var width = 100;
                            var height = 100 * scale;

                            var offSet = 6;
                            page.AddImage(eyeStream, new Rectangle(offSet, offSet, width + offSet, height + offSet, Unit.Millimeters));
                        }
                    })
                    // Test shape graphics
                    .AddPage(page =>
                    {
                        page.AddShapes(ctx =>
                        {
                            ctx.DefaultState(g =>
                            {
                                g.LineWidth = 1;
                                g.Fill = null;
                                g.Stroke = null;
                                g.Dash = new Dash()
                                {
                                    Array = new double[0],
                                    Phase = 0
                                };
                                g.MiterLimit = 10;
                                g.LineCap = LineCapStyle.ButtCap;
                                g.LineJoin = LineJoinStyle.MiterJoin;
                            });

                            ctx.NewPath(g => { g.Fill = Colors.Red; g.Stroke = Colors.Black; g.LineWidth = 5; })
                                .Move(100, 100)
                                .LineTo(200, 100)
                                .LineTo(200, 200)
                                .LineTo(100, 200);
                            ctx.NewPath(g => { g.Fill = Colors.Blue; g.Stroke = null; })
                                .Move(50, 50)
                                .LineTo(150, 50)
                                .LineTo(150, 150)
                                .LineTo(50, 150)
                                .Close();
                            ctx.NewPath(g => { g.Fill = null; g.Stroke = Colors.Yellow; g.LineWidth = 3; g.Dash = new Dash() { Array = new[] { 5d } }; })
                                .Move(150, 150)
                                .LineTo(250, 150)
                                .LineTo(250, 250)
                                .LineTo(150, 250)
                                .Close();
                        });
                    })
                    // Test placement using matrix
                    .AddPage(page =>
                    {
                        page.MediaBox = mediaBox;
                        page.TrimBox = trimBox;

                        using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
                        using (var forestImage = SixLabors.ImageSharp.Image.Load(forestStream))
                        {
                            var scale = (double)forestImage.Width / forestImage.Height;

                            var matrix = Matrix.CreateScaleMatrix(new Value(scale * 303, Unit.Millimeters), new Value(303, Unit.Millimeters))
                                .Translate(new Value(-100, Unit.Millimeters), new Value(0, Unit.Millimeters));

                            page.AddImage(forestImage, matrix);
                        }
                    });

                using (var blurStream = File.OpenRead("Pexels_com/4k-wallpaper-blur-bokeh-1484253.jpg"))
                using (var blurImage = SixLabors.ImageSharp.Image.Load(blurStream))
                {
                    var reusedImage = writer.AddImage(blurImage);

                    for(int i = 0; i < 4; i++)
                    {
                        writer.AddPage(page =>
                        {
                            page.MediaBox = mediaBox;
                            page.TrimBox = trimBox;

                            var scale = (double)blurImage.Width / blurImage.Height;

                            page.AddImage(reusedImage, new Rectangle(0, 0, scale * 303, 303, Unit.Millimeters));
                        });
                    }
                }
            }
        }
    }
}
