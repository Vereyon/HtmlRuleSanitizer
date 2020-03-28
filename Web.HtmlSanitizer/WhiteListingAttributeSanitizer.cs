using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Vereyon.Web
{
    /// <summary>
    /// Attribute sanitizer which solely whitelists a attribute.
    /// </summary>
    public class WhiteListingAttributeSanitizer : IHtmlAttributeSanitizer
    {
        public SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule)
        {
            return SanitizerOperation.DoNothing;
        }

        /// <summary>
        /// Returns the default instance.
        /// </summary>
        public static WhiteListingAttributeSanitizer Default { get; private set; } = new WhiteListingAttributeSanitizer();
    }
}
