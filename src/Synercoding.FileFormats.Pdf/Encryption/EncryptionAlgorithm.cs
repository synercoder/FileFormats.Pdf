namespace Synercoding.FileFormats.Pdf.Encryption;

public enum EncryptionAlgorithm
{
    RC4With40BitsKey = 1,
    RC4WithMoreThan40BitsKey = 2,
    UnpublishedAlgorithm = 3,
    RC4OrAESKey128Bits = 4,
    AES256BitsKey = 5
}
