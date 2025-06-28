using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Encryption.Internal;

internal class AesOrRC4Decryptor : IDecryptor
{
    private readonly IDecryptor _stringDecryptor;
    private readonly IDecryptor _streamDecryptor;

    public AesOrRC4Decryptor(byte[] decryptionKey, StandardEncryptionDictionary encryptionDictionary)
    {
        _stringDecryptor = _getDecryptor(decryptionKey, encryptionDictionary.StrF, encryptionDictionary.CF);
        _streamDecryptor = _getDecryptor(decryptionKey, encryptionDictionary.StmF, encryptionDictionary.CF);
    }

    public PdfString Decrypt(PdfString rawValue, PdfObjectId id)
        => _stringDecryptor.Decrypt(rawValue, id);

    public IPdfStreamObject Decrypt(IPdfStreamObject stream, PdfObjectId id)
        => _streamDecryptor.Decrypt(stream, id);

    private IDecryptor _getDecryptor(byte[] decryptionKey, PdfName? cfEntry, IDictionary<PdfName, IPdfDictionary>? cf)
    {
        if(cf is null)
            return new RC4PasswordDecryptor(decryptionKey);

        cfEntry ??= PdfNames.Identity;

        if(!cf.TryGetValue(cfEntry, out var filterDictionary))
            return new RC4PasswordDecryptor(decryptionKey);

        if(!filterDictionary.TryGetValue<PdfName>(PdfNames.CFM, out var cfm))
            return new RC4PasswordDecryptor(decryptionKey);

        if (cfm == PdfNames.AESV2)
            return new AesPasswordDecryptor(decryptionKey);

        if (cfm == PdfNames.AESV3)
            throw new NotImplementedException();

        return new RC4PasswordDecryptor(decryptionKey);
    }
}
