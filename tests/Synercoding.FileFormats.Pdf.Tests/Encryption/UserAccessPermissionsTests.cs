using Synercoding.FileFormats.Pdf.Encryption;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class UserAccessPermissionsTests
{
    [Fact]
    public void Test_UserAccessPermissions_Values()
    {
        Assert.Equal(0b0000_0000_0000_0100, (int)UserAccessPermissions.Print);
        Assert.Equal(0b0000_0000_0000_1000, (int)UserAccessPermissions.Modify);
        Assert.Equal(0b0000_0000_0001_0000, (int)UserAccessPermissions.CopyAndExtract);
        Assert.Equal(0b0000_0000_0010_0000, (int)UserAccessPermissions.Annotations);
        Assert.Equal(0b0000_0001_0000_0000, (int)UserAccessPermissions.InteractiveFormFields);
        Assert.Equal(0b0000_0010_0000_0000, (int)UserAccessPermissions.ExtractTextAndGraphics);
        Assert.Equal(0b0000_0100_0000_0000, (int)UserAccessPermissions.AssembleDocument);
        Assert.Equal(0b0000_1000_0000_0000, (int)UserAccessPermissions.PrintHighQuality);
    }

    [Fact]
    public void Test_UserAccessPermissions_FlagsAttribute()
    {
        var flagsAttribute = typeof(UserAccessPermissions).GetCustomAttributes(typeof(FlagsAttribute), false);
        Assert.Single(flagsAttribute);
    }
}
