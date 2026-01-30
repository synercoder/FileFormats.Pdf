namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Provides static access to commonly used PDF name objects defined in the PDF specification.
/// </summary>
public static class PdfNames
{
    // To match naming in pdf reference, also otherwise there are clashes OP != op.
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static PdfName AESV2
        => PdfName.Get(nameof(AESV2));
    public static PdfName AESV3
        => PdfName.Get(nameof(AESV3));
    public static PdfName ASCII85Decode
        => PdfName.Get(nameof(ASCII85Decode));
    public static PdfName ASCIIHexDecode
        => PdfName.Get(nameof(ASCIIHexDecode));
    public static PdfName ArtBox
        => PdfName.Get(nameof(ArtBox));
    public static PdfName Ascent
        => PdfName.Get(nameof(Ascent));
    public static PdfName Author
        => PdfName.Get(nameof(Author));
    public static PdfName BaseFont
        => PdfName.Get(nameof(BaseFont));
    public static PdfName BitsPerComponent
        => PdfName.Get(nameof(BitsPerComponent));
    public static PdfName Black
        => PdfName.Get(nameof(Black));
    public static PdfName BleedBox
        => PdfName.Get(nameof(BleedBox));
    public static PdfName CapHeight
        => PdfName.Get(nameof(CapHeight));
    public static PdfName Catalog
        => PdfName.Get(nameof(Catalog));
    public static PdfName CCITTFaxDecode
        => PdfName.Get(nameof(CCITTFaxDecode));
    public static PdfName CF
        => PdfName.Get(nameof(CF));
    public static PdfName CFM
        => PdfName.Get(nameof(CFM));
    public static PdfName CIDFontType2
        => PdfName.Get(nameof(CIDFontType2));
    public static PdfName CIDSystemInfo
        => PdfName.Get(nameof(CIDSystemInfo));
    public static PdfName CIDToGIDMap
        => PdfName.Get(nameof(CIDToGIDMap));
    public static PdfName ColorSpace
        => PdfName.Get(nameof(ColorSpace));
    public static PdfName Colors
        => PdfName.Get(nameof(Colors));
    public static PdfName Columns
        => PdfName.Get(nameof(Columns));
    public static PdfName Contents
        => PdfName.Get(nameof(Contents));
    public static PdfName Count
        => PdfName.Get(nameof(Count));
    public static PdfName CreationDate
        => PdfName.Get(nameof(CreationDate));
    public static PdfName Creator
        => PdfName.Get(nameof(Creator));
    public static PdfName CropBox
        => PdfName.Get(nameof(CropBox));
    public static PdfName Crypt
        => PdfName.Get(nameof(Crypt));
    public static PdfName Cyan
        => PdfName.Get(nameof(Cyan));
    public static PdfName DCTDecode
        => PdfName.Get(nameof(DCTDecode));
    public static PdfName Decode
        => PdfName.Get(nameof(Decode));
    public static PdfName DecodeParms
        => PdfName.Get(nameof(DecodeParms));
    public static PdfName DescendantFonts
        => PdfName.Get(nameof(DescendantFonts));
    public static PdfName Descent
        => PdfName.Get(nameof(Descent));
    public static PdfName DeviceCMYK
        => PdfName.Get(nameof(DeviceCMYK));
    public static PdfName DeviceGray
        => PdfName.Get(nameof(DeviceGray));
    public static PdfName DeviceRGB
        => PdfName.Get(nameof(DeviceRGB));
    public static PdfName DW
        => PdfName.Get(nameof(DW));
    public static PdfName EarlyChange
        => PdfName.Get(nameof(EarlyChange));
    public static PdfName EFF
        => PdfName.Get(nameof(EFF));
    public static PdfName Encoding
        => PdfName.Get(nameof(Encoding));
    public static PdfName Encrypt
        => PdfName.Get(nameof(Encrypt));
    public static PdfName EncryptMetadata
        => PdfName.Get(nameof(EncryptMetadata));
    public static PdfName ExtGState
        => PdfName.Get(nameof(ExtGState));
    public static PdfName Filter
        => PdfName.Get(nameof(Filter));
    public static PdfName First
        => PdfName.Get(nameof(First));
    public static PdfName Flags
        => PdfName.Get(nameof(Flags));
    public static PdfName FlateDecode
        => PdfName.Get(nameof(FlateDecode));
    public static PdfName Font
        => PdfName.Get(nameof(Font));
    public static PdfName FontBBox
        => PdfName.Get(nameof(FontBBox));
    public static PdfName FontDescriptor
        => PdfName.Get(nameof(FontDescriptor));
    public static PdfName FontFile2
        => PdfName.Get(nameof(FontFile2));
    public static PdfName FontName
        => PdfName.Get(nameof(FontName));
    public static PdfName FullScreen
        => PdfName.Get(nameof(FullScreen));
    public static PdfName Height
        => PdfName.Get(nameof(Height));
    public static PdfName ID
        => PdfName.Get(nameof(ID));
    public static PdfName Identity
        => PdfName.Get(nameof(Identity));
    public static PdfName IdentityH
        => PdfName.Get("Identity-H");
    public static PdfName Image
        => PdfName.Get(nameof(Image));
    public static PdfName Index
        => PdfName.Get(nameof(Index));
    public static PdfName Info
        => PdfName.Get(nameof(Info));
    public static PdfName ItalicAngle
        => PdfName.Get(nameof(ItalicAngle));
    public static PdfName JBIG2Decode
        => PdfName.Get(nameof(JBIG2Decode));
    public static PdfName JPXDecode
        => PdfName.Get(nameof(JPXDecode));
    public static PdfName Keywords
        => PdfName.Get(nameof(Keywords));
    public static PdfName Kids
        => PdfName.Get(nameof(Kids));
    public static PdfName Length
        => PdfName.Get(nameof(Length));
    public static PdfName Length1
        => PdfName.Get(nameof(Length1));
    public static PdfName LZWDecode
        => PdfName.Get(nameof(LZWDecode));
    public static PdfName Magenta
        => PdfName.Get(nameof(Magenta));
    public static PdfName MediaBox
        => PdfName.Get(nameof(MediaBox));
    public static PdfName Metadata
        => PdfName.Get(nameof(Metadata));
    public static PdfName ModDate
        => PdfName.Get(nameof(ModDate));
    public static PdfName N
        => PdfName.Get(nameof(N));
    public static PdfName None
        => PdfName.Get(nameof(None));
    public static PdfName O
        => PdfName.Get(nameof(O));
    public static PdfName ObjStm
        => PdfName.Get(nameof(ObjStm));
    public static PdfName OE
        => PdfName.Get(nameof(OE));
    public static PdfName OneColumn
        => PdfName.Get(nameof(OneColumn));
    public static PdfName OP
        => PdfName.Get(nameof(OP));
    public static PdfName op
        => PdfName.Get(nameof(op));
    public static PdfName Ordering
        => PdfName.Get(nameof(Ordering));
    public static PdfName P
        => PdfName.Get(nameof(P));
    public static PdfName Page
        => PdfName.Get(nameof(Page));
    public static PdfName PageLayout
        => PdfName.Get(nameof(PageLayout));
    public static PdfName PageMode
        => PdfName.Get(nameof(PageMode));
    public static PdfName Pages
        => PdfName.Get(nameof(Pages));
    public static PdfName Parent
        => PdfName.Get(nameof(Parent));
    public static PdfName Pattern
        => PdfName.Get(nameof(Pattern));
    public static PdfName Perms
        => PdfName.Get(nameof(Perms));
    public static PdfName Predictor
        => PdfName.Get(nameof(Predictor));
    public static PdfName Prev
        => PdfName.Get(nameof(Prev));
    public static PdfName ProcSet
        => PdfName.Get(nameof(ProcSet));
    public static PdfName Producer
        => PdfName.Get(nameof(Producer));
    public static PdfName Properties
        => PdfName.Get(nameof(Properties));
    public static PdfName R
        => PdfName.Get(nameof(R));
    public static PdfName Registry
        => PdfName.Get(nameof(Registry));
    public static PdfName Resources
        => PdfName.Get(nameof(Resources));
    public static PdfName Root
        => PdfName.Get(nameof(Root));
    public static PdfName Rotate
        => PdfName.Get(nameof(Rotate));
    public static PdfName RunLengthDecode
        => PdfName.Get(nameof(RunLengthDecode));
    public static PdfName Separation
        => PdfName.Get(nameof(Separation));
    public static PdfName Shading
        => PdfName.Get(nameof(Shading));
    public static PdfName SinglePage
        => PdfName.Get(nameof(SinglePage));
    public static PdfName Size
        => PdfName.Get(nameof(Size));
    public static PdfName SMask
        => PdfName.Get(nameof(SMask));
    public static PdfName Standard
        => PdfName.Get(nameof(Standard));
    public static PdfName StemV
        => PdfName.Get(nameof(StemV));
    public static PdfName StmF
        => PdfName.Get(nameof(StmF));
    public static PdfName StrF
        => PdfName.Get(nameof(StrF));
    public static PdfName SubFilter
        => PdfName.Get(nameof(SubFilter));
    public static PdfName Subject
        => PdfName.Get(nameof(Subject));
    public static PdfName Subtype
        => PdfName.Get(nameof(Subtype));
    public static PdfName Supplement
        => PdfName.Get(nameof(Supplement));
    public static PdfName Title
        => PdfName.Get(nameof(Title));
    public static PdfName ToUnicode
        => PdfName.Get(nameof(ToUnicode));
    public static PdfName TrimBox
        => PdfName.Get(nameof(TrimBox));
    public static PdfName TwoColumnLeft
        => PdfName.Get(nameof(TwoColumnLeft));
    public static PdfName TwoColumnRight
        => PdfName.Get(nameof(TwoColumnRight));
    public static PdfName TwoPageLeft
        => PdfName.Get(nameof(TwoPageLeft));
    public static PdfName TwoPageRight
        => PdfName.Get(nameof(TwoPageRight));
    public static PdfName Type
        => PdfName.Get(nameof(Type));
    public static PdfName Type0
        => PdfName.Get(nameof(Type0));
    public static PdfName Type1
        => PdfName.Get(nameof(Type1));
    public static PdfName U
        => PdfName.Get(nameof(U));
    public static PdfName UE
        => PdfName.Get(nameof(UE));
    public static PdfName UseAttachments
        => PdfName.Get(nameof(UseAttachments));
    public static PdfName UseNone
        => PdfName.Get(nameof(UseNone));
    public static PdfName UseOC
        => PdfName.Get(nameof(UseOC));
    public static PdfName UseOutlines
        => PdfName.Get(nameof(UseOutlines));
    public static PdfName UserUnit
        => PdfName.Get(nameof(UserUnit));
    public static PdfName UseThumbs
        => PdfName.Get(nameof(UseThumbs));
    public static PdfName V
        => PdfName.Get(nameof(V));
    public static PdfName V2
        => PdfName.Get(nameof(V2));
    public static PdfName V4
        => PdfName.Get(nameof(V4));
    public static PdfName V5
        => PdfName.Get(nameof(V5));
    public static PdfName Version
        => PdfName.Get(nameof(Version));
    public static PdfName W
        => PdfName.Get(nameof(W));
    public static PdfName Width
        => PdfName.Get(nameof(Width));
    public static PdfName XObject
        => PdfName.Get(nameof(XObject));
    public static PdfName XRef
        => PdfName.Get(nameof(XRef));
    public static PdfName XRefStm
        => PdfName.Get(nameof(XRefStm));
    public static PdfName Yellow
        => PdfName.Get(nameof(Yellow));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore IDE1006 // Naming Styles
}
