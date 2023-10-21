using HtmlAgilityPack;

namespace Vereyon.Web;

public interface IHtmlElementSanitizer
{
	/// <summary>
	/// Checks the passed element.
	/// </summary>
	/// <param name="element"></param>
	/// <param name="tagRule"></param>
	/// <returns></returns>
	SanitizerOperation SanitizeElement(HtmlNode element, HtmlSanitizerTagRule tagRule);
}