using Synercoding.FileFormats.Pdf.Primitives;
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
            }
        }
    }
}
