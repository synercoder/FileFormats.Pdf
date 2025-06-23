namespace Synercoding.FileFormats.Pdf.Encryption;

[Flags]
public enum UserAccessPermissions
{
    Print                  = 0b0000_0000_0000_0100,
    Modify                 = 0b0000_0000_0000_1000,
    CopyAndExtract         = 0b0000_0000_0001_0000,
    Annotations            = 0b0000_0000_0010_0000,
    InteractiveFormFields  = 0b0000_0001_0000_0000,
    ExtractTextAndGraphics = 0b0000_0010_0000_0000,
    AssembleDocument       = 0b0000_0100_0000_0000,
    PrintHighQuality       = 0b0000_1000_0000_0000,
}
