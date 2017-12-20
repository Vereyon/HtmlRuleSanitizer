using System;
using System.Collections.Generic;
using System.Linq;

namespace Vereyon.Web
{
    /// <summary>
    /// The HtmlSanitizerTagRule class specifies how the ocurrence of it's associated HTML tag is to be handled. The mere
    /// presence of an rule for a tag white lists the tag.
    /// </summary>
    public class HtmlSanitizerTagRule
    {

        /// <summary>
        /// Gets the HTML tag name this rule is to act upon.
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Gets / sets a global operation to be applied to this tag type.
        /// </summary>
        public SanitizerOperation Operation { get; set; }

        /// <summary>
        /// Gets / sets if this tag should be removed while it's contents are preserved.
        /// </summary>
        public bool Flatten { get; set; }

        /// <summary>
        /// Gets / sets to which name this tag should be renamed. Set to null to not rename the tag.
        /// </summary>
        public string RenameTag { get; set; }

        /// <summary>
        /// Gets / sets a string of CSS classes to be added to this tag.
        /// </summary>
        public string SetClass { get; set; }

        /// <summary>
        /// Gets / sets the operation to be performed on the tag if it does not have any attributes.
        /// </summary>
        /// <remarks>
        /// Example usage includes flattening any link tags which had illegal urls, while still preserving their contents.
        /// </remarks>
        public SanitizerOperation NoAttributesOperation { get; set; }

        /// <summary>
        /// Sets which attributes should have their value set or overridden. Attribute name as key, value to set as value.
        /// </summary>
        public IDictionary<string, string> SetAttributes { get; protected set; }

        /// <summary>
        /// Sets which checks to perform on which attributes. Attribute name as key, check type as value.
        /// </summary>
        public IDictionary<string, HtmlSanitizerCheckType> CheckAttributes { get; protected set; }

        /// <summary>
        /// Gets / sets if empty instances of this tag should be removed.
        /// </summary>
        public bool RemoveEmpty { get; set; }

        public HtmlSanitizerTagRule(string tagName)
        {
            TagName = tagName;
            Operation = SanitizerOperation.DoNothing;
            SetAttributes = new Dictionary<string, string>();
            CheckAttributes = new Dictionary<string, HtmlSanitizerCheckType>();
            NoAttributesOperation = SanitizerOperation.DoNothing;
        }
    }

    public enum SanitizerOperation
    {
        /// <summary>
        /// Default operation. Does nothing.
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Strip the tag while preserving it's contents.
        /// </summary>
        FlattenTag,

        /// <summary>
        /// Completely remove the tag and it's contents.
        /// </summary>
        RemoveTag,

        /// <summary>
        /// Removes only the attribute while preserving the tag itself.
        /// </summary>
        RemoveAttribute
    }
}