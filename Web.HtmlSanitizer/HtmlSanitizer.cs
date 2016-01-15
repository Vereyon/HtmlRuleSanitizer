using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace Vereyon.Web
{
    /// <summary>
    /// The HtmlSanitizer class implements a rule based HTML sanitizer.
    /// </summary>
    /// <remarks>
    /// Inspired by: https://htmlagilitypack.codeplex.com/discussions/215674
    ///         and: https://github.com/xing/wysihtml5/blob/master/parser_rules/simple.js
    /// </remarks>
    public class HtmlSanitizer
    {

        /// <summary>
        /// Sets which CSS classes are allowed on any HTML tag.
        /// </summary>
        public IList<string> AllowedCssClasses { get; protected set; }

        /// <summary>
        /// Sets which HTML sanitation rules are to be applied to a tag. Tag name as key, rule as value.
        /// </summary>
        public IDictionary<string, HtmlSanitizerTagRule> Rules { get; protected set; }

        /// <summary>
        /// Contains sanitation checks supported HtmlSanitizer class instance.
        /// </summary>
        public IDictionary<HtmlSanitizerCheckType, HtmlSanitizerAttributeCheckHandler> AttributeCheckRegistry { get; protected set; }

        /// <summary>
        /// Gets / sets if any HTML comments should be removed.
        /// </summary>
        public bool RemoveComments { get; set; }

        /// <summary>
        /// Gets / sets if the sanitizer operates in white list mode. If so, only tags for which rules are
        /// set and attributes for which checks are set are preserved.
        /// </summary>
        public bool WhiteListMode { get; set; }

        public HtmlSanitizer()
        {
            WhiteListMode = true;
            AllowedCssClasses = new List<string>();
            Rules = new Dictionary<string, HtmlSanitizerTagRule>();
            AttributeCheckRegistry = new Dictionary<HtmlSanitizerCheckType, HtmlSanitizerAttributeCheckHandler>();

            RegisterChecks();
        }

        /// <summary>
        /// Registers the out of the box supported sanitation checks.
        /// </summary>
        private void RegisterChecks()
        {

            AttributeCheckRegistry.Add(HtmlSanitizerCheckType.Url, new HtmlSanitizerAttributeCheckHandler(LinkHrefCheck));
            AttributeCheckRegistry.Add(HtmlSanitizerCheckType.AllowAttribute, new HtmlSanitizerAttributeCheckHandler(x => SanitizerOperation.DoNothing));
        }

        /// <summary>
        /// A attribute check handler returns false if the attribute is to be rejected and removed.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public delegate SanitizerOperation HtmlSanitizerAttributeCheckHandler(HtmlAttribute attribute);

        public static IEnumerable<string> AllowedUriSchemes = new string[] { "http", "https" };

        /// <summary>
        /// Checks if the passed HTML attribute contains a valid URL.
        /// </summary>
        /// <param name="attribute"></param>
        public static bool AttributeUrlCheck(HtmlAttribute attribute)
        {

            string url = attribute.Value;

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
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

        /// <summary>
        /// Checks if the href attribute contains a valid link.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static SanitizerOperation LinkHrefCheck(HtmlAttribute attribute)
        {

            if (attribute.Name != "href")
                throw new ArgumentException("Expected href attribute.");

            // Check the url. There's no use in keeping link tags without a link, so flatten the tag on failure.
            if (!AttributeUrlCheck(attribute))
                return SanitizerOperation.FlattenTag;

            return SanitizerOperation.DoNothing;
        }

        /// <summary>
        /// Sanitizes the passed HTML.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string Sanitize(string html)
        {

            // Trim the input.
            html = html.Trim();
            if (html.Length < 1)
                return string.Empty;

            // Load HTML document.
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            // Start recursize sanitiation at the document node.
            SanitizeNode(htmlDocument.DocumentNode);

            // Flatten the sanitized document and return the result.
            return htmlDocument.DocumentNode.WriteTo();
        }

        public void SanitizeNode(HtmlNode node)
        {

            HtmlSanitizerTagRule rule;
            SanitizerOperation operation;

            // Remove any comment nodes if instructed to do so.
            if (node.NodeType == HtmlNodeType.Comment && RemoveComments)
            {
                node.Remove();
                return;
            }

            // Only further process element nodes (includes the root document).
            if (node.NodeType != HtmlNodeType.Element
                && node.NodeType != HtmlNodeType.Document)
                return;

            // Make sure the tag name is all small caps. HTML5 does not have any
            // capitalized letters in it's tag names.
            node.Name = node.Name.ToLowerInvariant();

            // Lookup the rule for this node (may be null). If we are in white list mode and no rule is found,
            // remove the node. Don't remove the document however.
            if (!Rules.TryGetValue(node.Name, out rule)
                && WhiteListMode && node.NodeType != HtmlNodeType.Document)
            {
                node.Remove();
                return;
            }

            if (rule != null)
            {

                // Apply the global node operation. Quit if it was removed.
                if (!ApplyNodeOperation(node, rule.Operation))
                    return;

                // If the tag is empty and the rule instructs the removal of empty tag, remove the node.
                if (rule.RemoveEmpty
                    && string.IsNullOrEmpty(node.InnerHtml)
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
                    var className = node.GetAttributeValue("class", string.Empty);
                    if (string.IsNullOrEmpty(className))
                        className = rule.SetClass;
                    else
                        className += " " + rule.SetClass;
                    node.SetAttributeValue("class", className);
                }

                // If the node does not have any attributes, see if we need to do anything with it.
                if (node.Attributes.Count == 0)
                {
                    if (!ApplyNodeOperation(node, rule.NoAttributesOperation))
                        return;
                }

                // Ensure that all attributes are set according to the rule.
                foreach (var setAttribute in rule.SetAttributes.Where(r => !node.Attributes.Contains(r.Key)))
                    node.Attributes.Add(setAttribute.Key, setAttribute.Value);
            }

            // Finally process any child nodes recursively.
            SanitizeChildren(node);
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
                    SanitizeChildren(node);
                    foreach (var child in node.ChildNodes)
                        node.ParentNode.InsertBefore(child, node);
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

        private void SanitizeChildren(HtmlNode node)
        {

            // Do this in reverse to allow removal of nodes without hassle.
            for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
                SanitizeNode(node.ChildNodes[i]);
        }

        private SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule rule)
        {

            // Ensure that the attribute name does not contain any caps.
            attribute.Name = attribute.Name.ToLowerInvariant();
            
            // Apply global CSS class whitelist. If the attribute is complete removed, we are done.
            // TODO: Implement this as a global attribute check?
            if (attribute.Name == "class")
            {
                if (!ApplyCssWhitelist(attribute))
                    return SanitizerOperation.DoNothing;
            }

            HtmlSanitizerCheckType checkType;
            SanitizerOperation operation;

            // Apply attribute checks. If the check fails, remove the attribute completely and return.
            if (rule.CheckAttributes.TryGetValue(attribute.Name, out checkType))
            {
                operation = AttributeCheckRegistry[checkType](attribute);
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
            
            string valueOverride;

            // Apply value override if it is specified by the rule.
            if(rule.SetAttributes.TryGetValue(attribute.Name, out valueOverride))
                attribute.Value = valueOverride;

            // If we are in white listing mode and no check or override is specified, simply remove the attribute.
            // TODO: Wouldn't it be nicer is we generalized attribute rules for both checks and overrides? Would untangle code.
            if (WhiteListMode &&
                !rule.SetAttributes.ContainsKey(attribute.Name) &&
                !rule.CheckAttributes.ContainsKey(attribute.Name) && attribute.Name != "class")
            {
                attribute.Remove();
                return SanitizerOperation.DoNothing;
            }

            // Do nothing else.
            return SanitizerOperation.DoNothing;
        }

        /// <summary>
        /// Applies the CSS class white list to the passed attribute. Returns false if the complete attribute is removed.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private bool ApplyCssWhitelist(HtmlAttribute attribute)
        {

            // Break the attribute contents on white spaces after trimming off any white spaces.
            var cssClasses = attribute.Value.Trim().Split(' ');
            var passedClasses = string.Empty;

            // Inspect each class.
            foreach (var cssClass in cssClasses)
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
                attribute.Remove();
                return false;
            }

            attribute.Value = passedClasses;
            return true;
        }

        /// <summary>
        /// Returns an instance of the HtmlSanitizer with a HTML5 compliant rule set for documents with simple markup.
        /// </summary>
        /// <remarks>Strips all CSS and only allows simple links. Enfores nofollow.</remarks>
        /// <returns></returns>
        public static HtmlSanitizer SimpleHtml5Sanitizer()
        {

            var sanitizer = new HtmlSanitizer();

            sanitizer.WhiteListMode = true;
            sanitizer.Tag("h1").RemoveEmpty();
            sanitizer.Tag("h2").RemoveEmpty();
            sanitizer.Tag("h3").RemoveEmpty();
            sanitizer.Tag("h4").RemoveEmpty();
            sanitizer.Tag("h5").RemoveEmpty();
            sanitizer.Tag("strong").RemoveEmpty();
            sanitizer.Tag("b").Rename("strong").RemoveEmpty();
            sanitizer.Tag("i").RemoveEmpty();
            sanitizer.Tag("em");
            sanitizer.Tag("br");
            sanitizer.Tag("p");
            sanitizer.Tag("div").NoAttributes(SanitizerOperation.FlattenTag);
            sanitizer.Tag("span").RemoveEmpty();
            sanitizer.Tag("ul");
            sanitizer.Tag("ol");
            sanitizer.Tag("li");
            sanitizer.Tag("a").SetAttribute("target", "_blank")
                .SetAttribute("rel", "nofollow")
                .CheckAttribute("href", HtmlSanitizerCheckType.Url)
                .RemoveEmpty()
                .NoAttributes(SanitizerOperation.FlattenTag);

            return sanitizer;
        }

        /// <summary>
        /// Equal to the SimpleHtml5Sanitizer but allows html and body declarations.
        /// </summary>
        /// <returns></returns>
        public static HtmlSanitizer SimpleHtml5DocumentSanitizer()
        {

            var sanitizer = SimpleHtml5Sanitizer();

            sanitizer.Tag("html");
            sanitizer.Tag("body");

            return sanitizer;
        }
    }

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
}