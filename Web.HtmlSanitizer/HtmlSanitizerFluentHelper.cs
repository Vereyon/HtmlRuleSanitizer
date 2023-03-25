using System;

namespace Vereyon.Web;

/// <summary>
/// The HtmlSanitizerFluentHelper class implements fluent style extension methods for configuring the HtmlSanitizer.
/// </summary>
public static class HtmlSanitizerFluentHelper
{

	/// <summary>
	/// Specifies that the specified space seperated list of attributes are allowed on this tag.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="attributes"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule AllowAttributes(this HtmlSanitizerTagRule rule, string attributes)
	{
		_ = rule.SanitizeAttributes(attributes, WhiteListingAttributeSanitizer.Default);
		return rule;
	}

	/// <summary>
	/// White lists the specified CSS class names.
	/// </summary>
	/// <param name="sanitizer"></param>
	/// <param name="classNames"></param>
	/// <returns></returns>
	public static HtmlSanitizer AllowCss(this HtmlSanitizer sanitizer, params string[] classNames)
	{
		foreach (string className in classNames)
		{

			// Make sure no empty class names are added.
			string cleanedClassName = className.Trim();
			if (string.IsNullOrEmpty(cleanedClassName))
				continue;
			sanitizer.AllowedCssClasses.Add(cleanedClassName);
		}

		return sanitizer;
	}

	/// <summary>White lists the specified space seperated CSS class names.</summary>
	/// <param name="sanitizer">The sanitizer.</param>
	/// <param name="classNames">The class names.</param>
	/// <returns></returns>
	public static HtmlSanitizer AllowCss(this HtmlSanitizer sanitizer, string classNames) => sanitizer.AllowCss(classNames.Split(' '));

	/// <summary>
	/// Specifies that the given check is be performed on any attribute with the given name.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="attribute"></param>
	/// <param name="check"></param>
	/// <returns></returns>
	[Obsolete]
	public static HtmlSanitizerTagRule CheckAttribute(this HtmlSanitizerTagRule rule, string attribute, HtmlSanitizerCheckType check)
	{
		switch (check)
		{
			case HtmlSanitizerCheckType.Url:
				_ = rule.CheckAttributeUrl(attribute);
				break;
		}
		return rule;
	}

	/// <summary>
	/// Removes the tag but preserves it's contents in place of the tag matched by this rule.
	/// </summary>
	/// <param name="rule"></param>
	public static void Flatten(this HtmlSanitizerTagRule rule) => rule.Operation = SanitizerOperation.FlattenTag;

	/// <summary>Specifies the operation to perform if this node does not have any attributes set.</summary>
	/// <param name="rule">The rule.</param>
	/// <param name="operation">The operation.</param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule NoAttributes(this HtmlSanitizerTagRule rule, SanitizerOperation operation)
	{
		rule.NoAttributesOperation = operation;
		return rule;
	}

	/// <summary>
	/// Applies the specified global operation to a tag matching this rule.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="operation"></param>
	public static HtmlSanitizerTagRule Operation(this HtmlSanitizerTagRule rule, SanitizerOperation operation)
	{
		rule.Operation = operation;
		return rule;
	}

	/// <summary>
	/// Removes the tag and it's contents of the tag matched by this rule.
	/// </summary>
	/// <param name="rule"></param>
	public static void Remove(this HtmlSanitizerTagRule rule) => rule.Operation = SanitizerOperation.RemoveTag;

	/// <summary>
	/// Specifies that empty tags matching this rule should be removed.
	/// </summary>
	/// <param name="rule"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule RemoveEmpty(this HtmlSanitizerTagRule rule)
	{
		rule.RemoveEmpty = true;
		return rule;
	}

	/// <summary>
	/// Renames the tag to the specified tag name. Usefull for preserving content in unwanted HTML tags.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="newName"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule Rename(this HtmlSanitizerTagRule rule, string newName)
	{
		rule.RenameTag = newName;
		return rule;
	}

	/// <summary>
	/// Specifies the passed attribute sanitizer is to be applied to the space seperated list of attributes on this tag.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="attributes"></param>
	/// <param name="attributeSanitizer"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule SanitizeAttributes(this HtmlSanitizerTagRule rule, string attributes, IHtmlAttributeSanitizer attributeSanitizer)
	{
		foreach (string attribute in attributes.Split(' '))
		{
			string trimmed = attribute.Trim();
			if (string.IsNullOrEmpty(trimmed))
				continue;

			rule.AttributeChecks[trimmed] = attributeSanitizer;
		}

		return rule;
	}

	/// <summary>
	/// Specifies that the value of any attribute with the given name is to be set to the specified value.
	/// </summary>
	/// <param name="rule"></param>
	/// <param name="attribute"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule SetAttribute(this HtmlSanitizerTagRule rule, string attribute, string value)
	{
		rule.SetAttributes[attribute] = value;
		return rule;
	}

	/// <summary>
	/// White lists the specified HTML tag, creating a rule for it which allows further specification of what is to be done
	/// with the tag.
	/// </summary>
	/// <param name="sanitizer"></param>
	/// <param name="tagName"></param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule Tag(this HtmlSanitizer sanitizer, string tagName) => sanitizer.Tag(tagName, true);

	/// <summary>
	/// White lists the specified HTML tag, creating a rule for it which allows further specification of what is to be done
	/// with the tag.
	/// </summary>
	/// <param name="sanitizer">The sanitizer.</param>
	/// <param name="tagName">Name of the tag.</param>
	/// <param name="replace">if set to <c>true</c> replace.</param>
	/// <returns></returns>
	public static HtmlSanitizerTagRule Tag(this HtmlSanitizer sanitizer, string tagName, bool replace)
	{


		if (sanitizer.Rules.TryGetValue(tagName, out HtmlSanitizerTagRule rule))
		{
			if (replace)
				_ = sanitizer.Rules.Remove(tagName);
			else
				return rule;
		}

		rule = new HtmlSanitizerTagRule(tagName);
		sanitizer.Rules.Add(tagName, rule);
		return rule;
	}
}