using HtmlAgilityPack;
using System;
using System.Linq;

namespace Vereyon.Web;

/// <summary>
/// Attribute sanitizer which performs a check on the url in a attribute value.
/// </summary>
public class UrlCheckerAttributeSanitizer : IHtmlAttributeSanitizer
{
	/// <summary>
	/// Collection of the allowed URI schemes.
	/// </summary>
	public string[] AllowedUriSchemes { get; internal set; }

	/// <summary>
	/// Checks if the attribute contains a valid URL.
	/// </summary>
	/// <param name="attribute"></param>
	/// <param name="tagRule"></param>
	/// <returns></returns>
	public virtual SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule) =>
		// Check the url. We assume that there's no use in keeping for example a link tag without a href, so flatten the tag on failure.
		!AttributeUrlCheck(attribute) ? SanitizerOperation.FlattenTag : SanitizerOperation.DoNothing;

	/// <summary>
	/// Checks if the passed HTML attribute contains a valid URL.
	/// </summary>
	/// <param name="attribute"></param>
	protected internal virtual bool AttributeUrlCheck(HtmlAttribute attribute)
	{

		string url = attribute.Value;

		if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri))
			return false;

		// Reject the url if it is not well formed.
		if (!uri.IsWellFormedOriginalString())
			return false;

		// Reject the url if it has invalid scheme. Only do this check if we are dealing with an absolute url.
		if (uri.IsAbsoluteUri && !AllowedUriSchemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase))
			return false;

		// Make sure to the url is well formed.
		attribute.Value = uri.ToString();

		return true;
	}
}

/// <summary>
/// Fluent extension methods for URL checking.
/// </summary>
public static class UrlCheckerAttributeSanitizerFluentHelper
{

	/// <summary>
	/// Applies the default URL check to the specified attribute.
	/// </summary>
	public static HtmlSanitizerTagRule CheckAttributeUrl(this HtmlSanitizerTagRule rule, string attribute, string[] allowedUriSchemes = null)
	{
		rule.AttributeChecks.Add(attribute, new UrlCheckerAttributeSanitizer() { AllowedUriSchemes = allowedUriSchemes ?? HtmlSanitizer.defaultAllowedUriSchemes });
		return rule;
	}
}
