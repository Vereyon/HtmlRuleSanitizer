using HtmlAgilityPack;
using System.Collections.Generic;

namespace Vereyon.Web;

/// <summary>
/// Attribute sanitizer which only allows whitelisted CSS classes, and removes the class attribute alltogether if no CSS declarations remain.
/// </summary>
public class CssWhitelistAttributeSanitizer : IHtmlAttributeSanitizer
{

	/// <summary>
	/// Gets / sets the list of allowed CSS classes
	/// </summary>
	public virtual IList<string> AllowedCssClasses { get; set; } = new List<string>();

	/// <summary>
	/// Checks the passed attribute content agains the configured list of css classes.
	/// </summary>
	public SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule) => !ApplyCssWhitelist(attribute) ? SanitizerOperation.RemoveAttribute : SanitizerOperation.DoNothing;

	/// <summary>
	/// Applies the CSS class white list to the passed attribute. Returns false if the complete attribute is removed.
	/// </summary>
	/// <param name="attribute"></param>
	/// <returns></returns>
	protected virtual bool ApplyCssWhitelist(HtmlAttribute attribute)
	{

		// Break the attribute contents on white spaces after trimming off any white spaces.
		string[] cssClasses = attribute.Value.Trim().Split(' ');
		string passedClasses = string.Empty;

		// Inspect each class.
		foreach (string cssClass in cssClasses)
		{

			// No empty or white space classes.
			if (string.IsNullOrWhiteSpace(cssClass))
				continue;

			// Only allowed classes.
			if (!AllowedCssClasses.Contains(cssClass))
				continue;

			if (passedClasses.Length > 0)
				passedClasses += " ";
			passedClasses += cssClass;
		}

		// If nothing remains, remove the attribute. Else, set the passed classes.
		if (string.IsNullOrEmpty(passedClasses))
		{
			return false;
		}

		attribute.Value = passedClasses;
		return true;
	}
}
