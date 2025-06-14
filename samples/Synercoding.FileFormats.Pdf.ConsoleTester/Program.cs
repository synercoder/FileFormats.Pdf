using Synercoding.FileFormats.Pdf.Logging;

namespace Synercoding.FileFormats.Pdf.ConsoleTester;

public class Program
{
    public static void Main(string[] args)
    {
        var settings = new ReaderSettings()
        {
            Logger = new MultiLogger(new ConsoleLogger(), new DebugLogger())
        };
    }
}
