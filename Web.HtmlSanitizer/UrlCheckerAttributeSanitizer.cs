using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Vereyon.Web
{
    /// <summary>
    /// Attribute sanitizer which performs a check on the url in a attribute value.
    /// </summary>
    public class UrlCheckerAttributeSanitizer : IHtmlAttributeSanitizer
    {

        /// <summary>
        /// Returns the default instance.
        /// </summary>
        public static UrlCheckerAttributeSanitizer Default { get; private set; } = new UrlCheckerAttributeSanitizer();

        /// <summary>
        /// Checks if the attribute contains a valid link.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public virtual SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule)
        {
            // Check the url. We assume that there's no use in keeping for example a link tag without a href, so flatten the tag on failure.
            if (!AttributeUrlCheck(attribute))
                return SanitizerOperation.FlattenTag;

            return SanitizerOperation.DoNothing;
        }

        /// <summary>
        /// Checks if the passed HTML attribute contains a valid URL.
        /// </summary>
        /// <param name="attribute"></param>
        protected internal virtual bool AttributeUrlCheck(HtmlAttribute attribute)
        {

            string url = attribute.Value;

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                return false;

            // Reject the url if it is not well formed.
            if (!uri.IsWellFormedOriginalString())
                return false;

            // Reject the url if it has invalid scheme. Only do this check if we are dealing with an absolute url.
            if (uri.IsAbsoluteUri && !HtmlSanitizer.AllowedUriSchemes.Contains(uri.Scheme, StringComparer.OrdinalIgnoreCase))
                return false;

            // Make sure to the url is well formed.
            attribute.Value = uri.ToString();

            return true;
        }
    }

    public static class UrlCheckerAttributeSanitizerFluentHelper
    {

        public static HtmlSanitizerTagRule CheckAttributeUrl(this HtmlSanitizerTagRule rule, string attribute)
        {
            rule.AttributeChecks.Add(attribute, UrlCheckerAttributeSanitizer.Default);
            return rule;
        }
    }
}
