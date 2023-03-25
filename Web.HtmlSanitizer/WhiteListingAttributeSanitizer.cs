using HtmlAgilityPack;

namespace Vereyon.Web;

/// <summary>
/// Attribute sanitizer which solely whitelists a attribute.
/// </summary>
public class WhiteListingAttributeSanitizer : IHtmlAttributeSanitizer
{

	/// <summary>
	/// Returns the default instance.
	/// </summary>
	public static WhiteListingAttributeSanitizer Default { get; private set; } = new WhiteListingAttributeSanitizer();

	/// <summary>
	/// Returns SanitizerOperation.DoNothing for the specified attribute.
	/// </summary>
	public SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule) => SanitizerOperation.DoNothing;
}