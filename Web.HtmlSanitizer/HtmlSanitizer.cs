using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Vereyon.Web;

/// <summary>
/// The HtmlSanitizer class implements a rule based HTML sanitizer.
/// </summary>
/// <remarks>
/// Inspired by: https://htmlagilitypack.codeplex.com/discussions/215674
///         and: https://github.com/xing/wysihtml5/blob/master/parser_rules/simple.js
/// </remarks>
public class HtmlSanitizer : IHtmlSanitizer
{
	internal static readonly string[] defaultAllowedUriSchemes = new[] { "http", "https", "mailto", "tel" };

	private readonly CssWhitelistAttributeSanitizer _cssAttributeSanitizer;

	/// <summary>Initializes a new instance of the <see cref="HtmlSanitizer"/> class.</summary>
	public HtmlSanitizer()
	{
		WhiteListMode = true;
		EncodeHtmlEntities = true;
		SanitizeCssClasses = true;
		AllowedCssClasses = new List<string>();
		Rules = new Dictionary<string, HtmlSanitizerTagRule>();
		AttributeCheckRegistry = new Dictionary<HtmlSanitizerCheckType, HtmlSanitizerAttributeCheckHandler>();
		MaxRecursionDepth = 75;
		Depth = 0;

		RegisterChecks();

		_cssAttributeSanitizer = new CssWhitelistAttributeSanitizer
		{
			AllowedCssClasses = AllowedCssClasses
		};
	}

	/// <summary>
	/// A attribute check handler returns false if the attribute is to be rejected and removed.
	/// </summary>
	/// <param name="attribute"></param>
	/// <returns></returns>
	public delegate SanitizerOperation HtmlSanitizerAttributeCheckHandler(HtmlAttribute attribute);

	/// <summary>Occurs after a node is processed.</summary>
	public event Action<HtmlNode> PostprocessNode;

	/// <summary>Occurs before a node is processed and will cancel further processing if return value is <see langword="false"/>.</summary>
	public event Func<HtmlNode, bool> PreprocessNode;

	/// <summary>
	/// Sets which CSS classes are allowed on any HTML tag.
	/// </summary>
	public IList<string> AllowedCssClasses { get; protected set; }

	/// <summary>
	/// Collection of the allowed URI schemes.
	/// </summary>
	public IList<string> AllowedUriSchemes { get; set; } = new List<string>(defaultAllowedUriSchemes);

	/// <summary>
	/// Contains sanitation checks supported HtmlSanitizer class instance.
	/// </summary>
	[Obsolete]
	public IDictionary<HtmlSanitizerCheckType, HtmlSanitizerAttributeCheckHandler> AttributeCheckRegistry { get; protected set; }

	/// <summary>
	/// Gets / sets if HTML entities in all text should be encoded.
	/// </summary>
	public bool EncodeHtmlEntities { get; set; }

	/// <summary>
	/// Gets / sets the maximum depth in the DOM tree which is traversed. This limit is introduced to prevent stack overflows.
	/// </summary>
	/// <remarks>
	/// Problems tend to start above a recursion depth of about 75 where the ASP.NET runtime stack nears exhaustion.
	/// </remarks>
	public int MaxRecursionDepth { get; set; }

	/// <summary>
	/// Gets / sets if any HTML comments should be removed.
	/// </summary>
	public bool RemoveComments { get; set; }

	/// <summary>
	/// Sets which HTML sanitation rules are to be applied to a tag. Tag name as key, rule as value.
	/// </summary>
	public IDictionary<string, HtmlSanitizerTagRule> Rules { get; protected set; }

	/// <summary>
	/// Determines if CSS classes are sanitized.
	/// </summary>
	public bool SanitizeCssClasses { get; set; }

	/// <summary>
	/// Gets / sets if the sanitizer operates in white list mode. If so, only tags for which rules are
	/// set and attributes for which checks are set are preserved. Defaults to true.
	/// </summary>
	public bool WhiteListMode { get; set; }

	internal int Depth { get; set; }

	/// <summary>
	/// Checks if the passed HTML attribute contains a valid URL.
	/// </summary>
	/// <param name="attribute"></param>
	[Obsolete("This method has been deprecated in favor of the UrlCheckerAttributeSanitizer.")]
	public static bool AttributeUrlCheck(HtmlAttribute attribute) => new UrlCheckerAttributeSanitizer() { AllowedUriSchemes = defaultAllowedUriSchemes }.AttributeUrlCheck(attribute);

	/// <summary>
	/// Equal to the SimpleHtml5Sanitizer but allows html and body declarations.
	/// </summary>
	/// <returns></returns>
	public static HtmlSanitizer SimpleHtml5DocumentSanitizer()
	{

		HtmlSanitizer sanitizer = SimpleHtml5Sanitizer();
		_ = sanitizer.Tag("html");
		_ = sanitizer.Tag("body");
		return sanitizer;
	}

	/// <summary>
	/// Returns an instance of the HtmlSanitizer with a HTML5 compliant rule set for documents with simple markup.
	/// </summary>
	/// <remarks>Strips all CSS and only allows simple links. Enfores nofollow.</remarks>
	/// <returns></returns>
	public static HtmlSanitizer SimpleHtml5Sanitizer()
	{

		HtmlSanitizer sanitizer = new()
		{
			WhiteListMode = true
		};
		_ = sanitizer.Tag("h1").RemoveEmpty();
		_ = sanitizer.Tag("h2").RemoveEmpty();
		_ = sanitizer.Tag("h3").RemoveEmpty();
		_ = sanitizer.Tag("h4").RemoveEmpty();
		_ = sanitizer.Tag("h5").RemoveEmpty();
		_ = sanitizer.Tag("strong").RemoveEmpty();
		_ = sanitizer.Tag("b").Rename("strong").RemoveEmpty();
		_ = sanitizer.Tag("i").RemoveEmpty();
		_ = sanitizer.Tag("em");
		_ = sanitizer.Tag("br");
		_ = sanitizer.Tag("p");
		_ = sanitizer.Tag("div").NoAttributes(SanitizerOperation.FlattenTag);
		_ = sanitizer.Tag("span").RemoveEmpty();
		_ = sanitizer.Tag("ul");
		_ = sanitizer.Tag("ol")
			.AllowAttributes("start")
			.AllowAttributes("type")
			.AllowAttributes("reversed");
		_ = sanitizer.Tag("li")
			.AllowAttributes("value");
		_ = sanitizer.Tag("a").SetAttribute("target", "_blank")
			.SetAttribute("rel", "nofollow")
			.CheckAttributeUrl("href")
			.RemoveEmpty()
			.NoAttributes(SanitizerOperation.FlattenTag);

		return sanitizer;
	}

	/// <summary>
	/// Checks if the attribute contains a valid link.
	/// </summary>
	/// <param name="attribute"></param>
	/// <returns></returns>
	[Obsolete("This method has been deprecated in favor of the UrlCheckerAttributeSanitizer.")]
	public static SanitizerOperation UrlCheckHandler(HtmlAttribute attribute) =>

		// Check the url. We assume that there's no use in keeping for example a link tag without a href, so flatten the tag on failure.
		!AttributeUrlCheck(attribute) ? SanitizerOperation.FlattenTag : SanitizerOperation.DoNothing;

	/// <summary>
	/// Sanitizes the passed HTML string and returns the sanitized HTML.
	/// </summary>
	/// <param name="html">A string containing HTML formatted text.</param>
	/// <returns>A string containing sanitized HTML formatted text.</returns>
	public string Sanitize(string html)
	{

		// Trim the input.
		html = html.Trim();
		if (html.Length < 1)
			return string.Empty;

		// Load HTML document.
		HtmlDocument htmlDocument = new();
		htmlDocument.LoadHtml(html);

		// Start recursize sanitiation at the document node.
		SanitizeNode(htmlDocument.DocumentNode);

		// Flatten the sanitized document and return the result.
		return htmlDocument.DocumentNode.WriteTo();
	}

	/// <summary>Sanitizes the node.</summary>
	/// <param name="node">The node.</param>
	public void SanitizeNode(HtmlNode node)
	{

		SanitizerOperation operation;

		using (new RecursionGuard(this))
		{

			if (PreprocessNode is not null && PreprocessNode.Invoke(node) == false)
				return;

			// Remove any comment nodes if instructed to do so.
			if (node.NodeType == HtmlNodeType.Comment && RemoveComments)
			{
				node.Remove();
				return;
			}

			// In theory all text should have HTML entities (ampersand, quotes, lessthan, greaterthan) encoded.
			// In practice or in case of an attack this may not be the case. Make sure all entities are encoded, but avoid 
			// double encoding correctly encoded entities. Do so by first decoding entities and then encode entities
			// in the complete text.
			if (node.NodeType == HtmlNodeType.Text && EncodeHtmlEntities)
			{
				string deentitized = WebUtility.HtmlDecode(node.InnerText);

				// Unfortunately also unicode characters are encoded, which is not really necessary.
				string entitized = WebUtility.HtmlEncode(deentitized);
				HtmlNode replacement = HtmlTextNode.CreateNode(entitized);
				_ = node.ParentNode.ReplaceChild(replacement, node);
				return;
			}

			// Only further process element nodes (includes the root document).
			if (node.NodeType is not HtmlNodeType.Element and not HtmlNodeType.Document)
			{
				return;
			}

			// Make sure the tag name is all small caps. HTML5 does not have any
			// capitalized letters in it's tag names.
			node.Name = node.Name.ToLowerInvariant();

			// Lookup the rule for this node (may be null). If we are in white list mode and no rule is found,
			// remove the node. Don't remove the document however.
			if (!Rules.TryGetValue(node.Name, out HtmlSanitizerTagRule rule)
				&& WhiteListMode && node.NodeType != HtmlNodeType.Document)
			{
				node.Remove();
			}

			if (rule != null)
			{

				// Apply the global node operation. Quit if it was removed.
				if (!ApplyNodeOperation(node, rule.Operation))
					return;

				// If the tag is empty and the rule instructs the removal of empty tag, remove the node.
				if (rule.RemoveEmpty
					&& !node.HasAttributes
					&& !node.HasChildNodes)
				{
					node.Remove();
					return;
				}

				// Rename the tag if the rule dictates so.
				if (!string.IsNullOrEmpty(rule.RenameTag))
					node.Name = rule.RenameTag;
			}

			// Sanitize every attribute of the node in reverse order.
			for (int i = node.Attributes.Count - 1; i >= 0; i--)
			{
				operation = SanitizeAttribute(node.Attributes[i], rule);
				if (!ApplyNodeOperation(node, operation))
					return;
			}

			if (rule != null)
			{

				// Add the css class if specified by the rule. This needs to be done after sanitizing 
				// the attributes as specified class may not be white listed.
				if (!string.IsNullOrEmpty(rule.SetClass))
				{
					string className = node.GetAttributeValue("class", string.Empty);
					if (string.IsNullOrEmpty(className))
						className = rule.SetClass;
					else
						className += " " + rule.SetClass;
					_ = node.SetAttributeValue("class", className);
				}

				// If the node does not have any attributes, see if we need to do anything with it.
				if (node.Attributes.Count == 0
					&& !ApplyNodeOperation(node, rule.NoAttributesOperation))
					return;

				// Ensure that all attributes are set according to the rule.
				foreach (KeyValuePair<string, string> setAttribute in rule.SetAttributes.Where(r => !node.Attributes.Contains(r.Key)))
					node.Attributes.Add(setAttribute.Key, setAttribute.Value);
			}

			// Finally process any child nodes recursively.
			// Do this in reverse to allow removal of nodes without hassle.
			for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
				SanitizeNode(node.ChildNodes[i]);

			// If the tag is empty and the rule instructs the removal of empty tag, remove the node. We are doing
			// this again because at this point the node may have become empty.
			if (rule != null
				&& rule.RemoveEmpty
				&& !node.HasAttributes
				&& !node.HasChildNodes)
			{
				node.Remove();
				return;
			}

			PostprocessNode?.Invoke(node);
		}
	}

	/// <summary>
	/// Applies the specified operation to the specified node. Return false if the node was removed or does not need any futher processing.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="operation"></param>
	/// <returns></returns>
	private bool ApplyNodeOperation(HtmlNode node, SanitizerOperation operation)
	{
		switch (operation)
		{
			case SanitizerOperation.FlattenTag:

				// Sanitize children, then insert them after this node and remove this node.
				// Do this in reverse to allow removal of nodes without hassle.
				for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
					SanitizeNode(node.ChildNodes[i]);

				foreach (HtmlNode child in node.ChildNodes)
					_ = node.ParentNode.InsertBefore(child, node);
				node.Remove();
				return false;

			case SanitizerOperation.RemoveTag:
				node.Remove();
				return false;

			case SanitizerOperation.DoNothing:
				return true;

			default:
				throw new InvalidOperationException("Unsupported sanitation operation.");
		}
	}

	/// <summary>
	/// Registers the out of the box supported sanitation checks.
	/// </summary>
	[Obsolete]
	private void RegisterChecks()
	{

		AttributeCheckRegistry.Add(HtmlSanitizerCheckType.Url, new HtmlSanitizerAttributeCheckHandler(UrlCheckHandler));
		AttributeCheckRegistry.Add(HtmlSanitizerCheckType.AllowAttribute, new HtmlSanitizerAttributeCheckHandler(x => SanitizerOperation.DoNothing));
	}
	private SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule rule)
	{

		// Ensure that the attribute name does not contain any caps.
		attribute.Name = attribute.Name.ToLowerInvariant();

		SanitizerOperation operation;

		// Apply global CSS class whitelist. If the attribute is complete removed, we are done.
		// TODO: Implement this as a global attribute check?
		if (SanitizeCssClasses && attribute.Name == "class")
		{
			operation = _cssAttributeSanitizer.SanitizeAttribute(attribute, rule);
			switch (operation)
			{
				case SanitizerOperation.RemoveAttribute:
					attribute.Remove();
					return SanitizerOperation.DoNothing;
			}
		}

		if (rule != null)
		{
			// Apply attribute checks. If the check fails, remove the attribute completely and return.
			if (rule.AttributeChecks.TryGetValue(attribute.Name, out IHtmlAttributeSanitizer attributeCheck))
			{
				operation = attributeCheck.SanitizeAttribute(attribute, rule);
				switch (operation)
				{
					case SanitizerOperation.FlattenTag:
					case SanitizerOperation.RemoveTag:

						// Can't handle these at this level. Return now as all attributes will be discared.
						return operation;
					case SanitizerOperation.RemoveAttribute:
						attribute.Remove();
						return SanitizerOperation.DoNothing;
					case SanitizerOperation.DoNothing:
						break;
					default:
						throw new InvalidOperationException("Unspported sanitation operation.");
				}
			}


			// Apply value override if it is specified by the rule.
			if (rule.SetAttributes.TryGetValue(attribute.Name, out string valueOverride))
				attribute.Value = valueOverride;

			// If we are in white listing mode and no check or override is specified, simply remove the attribute.
			// TODO: Wouldn't it be nicer is we generalized attribute rules for both checks and overrides? Would untangle code.
			if (WhiteListMode &&
					!rule.SetAttributes.ContainsKey(attribute.Name) &&
					!rule.AttributeChecks.ContainsKey(attribute.Name) && attribute.Name != "class")
			{
				attribute.Remove();
				return SanitizerOperation.DoNothing;
			}
		}

		// Do nothing else.
		return SanitizerOperation.DoNothing;
	}
}

/// <summary>
/// Types of attribute sanitizations.
/// </summary>
[Obsolete]
public enum HtmlSanitizerCheckType
{

	/// <summary>
	/// Checks if the passed HTML attribute contains a valid URL.
	/// </summary>
	Url,

	/// <summary>
	/// Specifies that this attribute is allowed and that it's value is not to be checked.
	/// </summary>
	AllowAttribute,
}