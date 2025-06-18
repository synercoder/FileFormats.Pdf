using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public interface IReadOnlyResources
{
    /// <summary>
    /// A dictionary that maps each resource name to either the
    /// name of a colour space with no additional parameters
    /// (DeviceGray, DeviceRGB, DEviceCMYK, or Pattern) or an
    /// array describing a colour space.
    /// </summary>
    IPdfDictionary? ColorSpace { get; }

    /// <summary>
    /// A dictionary that maps resource names to graphics state
    /// parameter dictionaries
    /// </summary>
    IPdfDictionary? ExtGState { get; }

    /// <summary>
    /// A dictionary that maps resource names to font dictionaries.
    /// </summary>
    IPdfDictionary? Font { get; }

    /// <summary>
    ///  A dictionary that maps resource names to pattern objects.
    /// </summary>
    IPdfDictionary? Pattern { get; }

    /// <summary>
    /// An array of predefined procedure set names.
    /// </summary>
    /// <remarks>Deprecated in PDF 2.0.</remarks>
    IPdfArray? ProcSet { get; }

    /// <summary>
    ///  A dictionary that maps resource names to property list
    ///  dictionaries for marked-content.
    /// </summary>
    IPdfDictionary? Properties { get; }

    /// <summary>
    /// A dictionary that maps resource names to shading dictionaries.
    /// </summary>
    IPdfDictionary? Shading { get; }

    /// <summary>
    /// A dictionary that maps resource names to external objects.
    /// </summary>
    IPdfDictionary? XObject { get; }
}
