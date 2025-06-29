using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Encryption;

public class DecryptionResultTests
{
    [Fact]
    public void Test_NotEncrypted_Result_Properties()
    {
        var result = DecryptionResult.NotEncrypted();

        Assert.Equal(AccessLevel.NotEncrypted, result.AccessLevel);
        Assert.Equal(0, result.KeyLength);
        Assert.Equal((UserAccessPermissions)0b1111_1111_1100, result.Permissions);
        Assert.Equal(EncryptionMethod.None, result.TextEncryptionMethod);
        Assert.Equal(EncryptionMethod.None, result.StreamEncryptionMethod);
    }

    [Fact]
    public void Test_NotEncrypted_GetDecryptor_ThrowsEncryptionException()
    {
        var result = DecryptionResult.NotEncrypted();

        var exception = Assert.Throws<EncryptionException>(() => result.GetDecryptor());
        Assert.Contains("not encrypted", exception.Message);
    }

    [Fact]
    public void Test_Encrypted_GetDecryptor_ThrowsEncryptionException()
    {
        var result = DecryptionResult.Fail(null!, 128);

        var exception = Assert.Throws<EncryptionException>(() => result.GetDecryptor());
        Assert.Contains("not encrypted", exception.Message);
    }

    [Fact]
    public void Test_AES128_Dictionary_EncryptionMethods()
    {
        var (dictionary, fileId, _, userPassword) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);
        
        var result = handler.AuthenticateUserPassword(userPassword);

        Assert.Equal(EncryptionMethod.AES, result.TextEncryptionMethod);
        Assert.Equal(EncryptionMethod.AES, result.StreamEncryptionMethod);
    }

    [Fact]
    public void Test_RC4_Dictionary_EncryptionMethods()
    {
        var (dictionary, fileId, _, userPassword) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);
        
        var result = handler.AuthenticateUserPassword(userPassword);

        Assert.Equal(EncryptionMethod.RC4, result.TextEncryptionMethod);
        Assert.Equal(EncryptionMethod.RC4, result.StreamEncryptionMethod);
    }

    [Fact]
    public void Test_Permissions_Default_ForNotEncrypted()
    {
        var result = DecryptionResult.NotEncrypted();

        var expectedPermissions = (UserAccessPermissions)0b1111_1111_1100;
        Assert.Equal(expectedPermissions, result.Permissions);
    }

    [Fact]
    public void Test_AES128_Success_GetDecryptor_ReturnsDecryptor()
    {
        var (dictionary, fileId, _, userPassword) = _createAes128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);
        
        var result = handler.AuthenticateUserPassword(userPassword);
        
        if (result.AccessLevel == AccessLevel.UserAccess)
        {
            var decryptor = result.GetDecryptor();
            Assert.NotNull(decryptor);
            Assert.IsType<PasswordDecryptor>(decryptor);
        }
    }

    [Fact]
    public void Test_RC4_Success_GetDecryptor_ReturnsDecryptor()
    {
        var (dictionary, fileId, _, userPassword) = _createRc4128BitDictionary();
        var handler = new StandardSecurityHandler(dictionary, fileId);
        
        var result = handler.AuthenticateUserPassword(userPassword);
        
        if (result.AccessLevel == AccessLevel.UserAccess)
        {
            var decryptor = result.GetDecryptor();
            Assert.NotNull(decryptor);
            Assert.IsType<PasswordDecryptor>(decryptor);
        }
    }

    [Fact]
    public void Test_Fail_Result_Properties()
    {
        var result = DecryptionResult.Fail(null!, 256);
        
        Assert.Equal(AccessLevel.Encrypted, result.AccessLevel);
        Assert.Equal(256, result.KeyLength);
    }

    private static (StandardEncryptionDictionary, byte[], string, string) _createAes128BitDictionary()
    {
        var ownerPassword = "ChangePW";
        var userPassword = "OpenPW";
        var fileId = new byte[] { 
            0x3e, 0xdc, 0x52, 0xa4, 0x0e, 0x97, 0x05, 0x4b, 
            0x4f, 0x9a, 0x9f, 0x86, 0x8e, 0xa1, 0x20, 0x2f 
        };

        var oBytes = new byte[] { 
            0x2b, 0x9d, 0x2d, 0x5c, 0x59, 0xed, 0x53, 0x47, 
            0x71, 0x49, 0xd9, 0x21, 0x27, 0xcd, 0x59, 0xa7, 
            0x7c, 0xe1, 0xd0, 0x29, 0x56, 0xc3, 0xfc, 0x82, 
            0xd8, 0x36, 0x19, 0x63, 0x8c, 0x6b, 0x1d, 0x2c 
        };
        var uBytes = new byte[] { 
            0x28, 0xbf, 0x4e, 0x5e, 0x4e, 0x75, 0x8a, 0x41, 
            0x64, 0x00, 0x4e, 0x56, 0xff, 0xfa, 0x01, 0x08, 
            0x2e, 0x2e, 0x00, 0xb6, 0xd0, 0x68, 0x37, 0x80, 
            0x7d, 0x01, 0xe2, 0xd2, 0x67, 0xc6, 0xfe, 0xb6 
        };

        var cryptFilter = new PdfDictionary()
        {
            [PdfNames.CFM] = PdfNames.AESV2,
            [PdfNames.Length] = new PdfNumber(128)
        };

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(4),
            [PdfNames.R] = new PdfNumber(4),
            [PdfNames.Length] = new PdfNumber(128),
            [PdfNames.O] = new PdfString(oBytes, true),
            [PdfNames.U] = new PdfString(uBytes, true),
            [PdfNames.P] = new PdfNumber(-1852),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true),
            [PdfNames.CF] = new PdfDictionary()
            {
                [PdfName.Get("StdCF")] = cryptFilter
            },
            [PdfNames.StmF] = PdfName.Get("StdCF"),
            [PdfNames.StrF] = PdfName.Get("StdCF")
        };

        var dictionary = new StandardEncryptionDictionary(pdfDictionary, null!);
        return (dictionary, fileId, ownerPassword, userPassword);
    }

    private static (StandardEncryptionDictionary, byte[], string, string) _createRc4128BitDictionary()
    {
        var ownerPassword = "ChangePW";
        var userPassword = "OpenPW";
        var fileId = new byte[] { 
            0x3e, 0xdc, 0x52, 0xa4, 0x0e, 0x97, 0x05, 0x4b, 
            0x4f, 0x9a, 0x9f, 0x86, 0x8e, 0xa1, 0x20, 0x2f 
        };

        var oBytes = new byte[] { 
            0x2b, 0x9d, 0x2d, 0x5c, 0x59, 0xed, 0x53, 0x47, 
            0x71, 0x49, 0xd9, 0x21, 0x27, 0xcd, 0x59, 0xa7, 
            0x7c, 0xe1, 0xd0, 0x29, 0x56, 0xc3, 0xfc, 0x82, 
            0xd8, 0x36, 0x19, 0x63, 0x8c, 0x6b, 0x1d, 0x2c 
        };
        var uBytes = new byte[] { 
            0x28, 0xbf, 0x4e, 0x5e, 0x4e, 0x75, 0x8a, 0x41, 
            0x64, 0x00, 0x4e, 0x56, 0xff, 0xfa, 0x01, 0x08, 
            0x2e, 0x2e, 0x00, 0xb6, 0xd0, 0x68, 0x37, 0x80, 
            0x7d, 0x01, 0xe2, 0xd2, 0x67, 0xc6, 0xfe, 0xb6 
        };

        var cryptFilter = new PdfDictionary()
        {
            [PdfNames.CFM] = PdfNames.V2,
            [PdfNames.Length] = new PdfNumber(128)
        };

        var pdfDictionary = new PdfDictionary()
        {
            [PdfNames.Filter] = PdfNames.Standard,
            [PdfNames.V] = new PdfNumber(4),
            [PdfNames.R] = new PdfNumber(4),
            [PdfNames.Length] = new PdfNumber(128),
            [PdfNames.O] = new PdfString(oBytes, true),
            [PdfNames.U] = new PdfString(uBytes, true),
            [PdfNames.P] = new PdfNumber(-1852),
            [PdfNames.EncryptMetadata] = new PdfBoolean(true),
            [PdfNames.CF] = new PdfDictionary()
            {
                [PdfName.Get("StdCF")] = cryptFilter
            },
            [PdfNames.StmF] = PdfName.Get("StdCF"),
            [PdfNames.StrF] = PdfName.Get("StdCF")
        };

        var dictionary = new StandardEncryptionDictionary(pdfDictionary, null!);
        return (dictionary, fileId, ownerPassword, userPassword);
    }
}
