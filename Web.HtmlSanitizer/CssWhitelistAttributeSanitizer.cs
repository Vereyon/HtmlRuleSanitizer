using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Vereyon.Web
{
    /// <summary>
    /// Attribute sanitizer which only allows whitelisted CSS classes, and removes the class attribute alltogether if no CSS declarations remain.
    /// </summary>
    public class CssWhitelistAttributeSanitizer : IHtmlAttributeSanitizer
    {
        public SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule)
        {

            if (!ApplyCssWhitelist(attribute))
                return SanitizerOperation.RemoveAttribute;

            return SanitizerOperation.DoNothing;
        }

        /// <summary>
        /// Gets / sets the list of allowed CSS classes
        /// </summary>
        public virtual IList<string> AllowedCssClasses { get; set; } = new List<string>();

        /// <summary>
        /// Applies the CSS class white list to the passed attribute. Returns false if the complete attribute is removed.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected virtual bool ApplyCssWhitelist(HtmlAttribute attribute)
        {

            // Break the attribute contents on white spaces after trimming off any white spaces.
            var cssClasses = attribute.Value.Trim().Split(' ');
            var passedClasses = string.Empty;

            // Inspect each class.
            foreach (var cssClass in cssClasses)
            {

                // No empty or white space classes.
                if (string.IsNullOrEmpty(cssClass?.Trim()))
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
}
