using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.Primitives.Matrices;
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
                var bleed = _mmToPts(3);
                var mediaBox = new Primitives.Rectangle(0, 0, _mmToPts(216), _mmToPts(303));
                var trimBox = new Primitives.Rectangle(
                    mediaBox.LLX + bleed,
                    mediaBox.LLY + bleed,
                    mediaBox.URX - bleed,
                    mediaBox.URY - bleed
                );

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
                        using (var barrenImage = Image.Load(barrenStream))
                        {
                            var scale = (double)barrenImage.Width / barrenImage.Height;

                            page.AddImage(barrenImage, new Primitives.Rectangle(0, 0, _mmToPts(scale * 303), _mmToPts(303)));
                        }

                        using (var eyeStream = File.OpenRead("Pexels_com/adult-blue-blue-eyes-865711.jpg"))
                        {
                            var scale = 3456d / 5184;

                            var width = _mmToPts(100);
                            var height = _mmToPts(100 * scale);

                            var offSet = _mmToPts(6);
                            page.AddImage(eyeStream, new Primitives.Rectangle(offSet, offSet, width + offSet, height + offSet));
                        }
                    })
                    // Test placement using matrix
                    .AddPage(page =>
                    {
                        page.MediaBox = mediaBox;
                        page.TrimBox = trimBox;

                        using (var forestStream = File.OpenRead("Pexels_com/android-wallpaper-art-backlit-1114897.jpg"))
                        using (var forestImage = Image.Load(forestStream))
                        {
                            var scale = (double)forestImage.Width / forestImage.Height;

                            var matrix = Matrix.CreateScaleMatrix(_mmToPts(scale * 303), _mmToPts(303))
                                .Translate(_mmToPts(-100), 0);

                            page.AddImage(forestImage, matrix);
                        }
                    });
            }
        }

        private static double _mmToPts(double mm) => mm / 25.4d * 72;
    }
}
