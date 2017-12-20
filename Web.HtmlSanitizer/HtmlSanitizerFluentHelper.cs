using System;
using System.Collections.Generic;
using System.Linq;

namespace Vereyon.Web
{
    /// <summary>
    /// The HtmlSanitizerFluentHelper class implements fluent style extension methods for configuring the HtmlSanitizer.
    /// </summary>
    public static class HtmlSanitizerFluentHelper
    {

        /// <summary>
        /// White lists the specified CSS class names.
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public static HtmlSanitizer AllowCss(this HtmlSanitizer sanitizer, params string[] classNames)
        {
            foreach (var className in classNames)
            {

                // Make sure no empty class names are added.
                string cleanedClassName = className.Trim();
                if (string.IsNullOrEmpty(cleanedClassName))
                    continue;
                sanitizer.AllowedCssClasses.Add(cleanedClassName);
            }

            return sanitizer;
        }

        /// <summary>
        /// White lists the specified space seperated CSS class names.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static HtmlSanitizer AllowCss(this HtmlSanitizer sanitizer, string classNames)
        {
            return sanitizer.AllowCss(classNames.Split(' '));
        }

        /// <summary>
        /// White lists the specified HTML tag, creating a rule for it which allows further specification of what is to be done
        /// with the tag.
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static HtmlSanitizerTagRule Tag(this HtmlSanitizer sanitizer, string tagName)
        {
            return sanitizer.Tag(tagName, true);
        }

        /// <summary>
        /// White lists the specified HTML tag, creating a rule for it which allows further specification of what is to be done
        /// with the tag.
        /// </summary>
        /// <param name="sanitizer"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static HtmlSanitizerTagRule Tag(this HtmlSanitizer sanitizer, string tagName, bool replace)
        {

            HtmlSanitizerTagRule rule;

            if (sanitizer.Rules.TryGetValue(tagName, out rule))
            {
                if (replace)
                    sanitizer.Rules.Remove(tagName);
                else
                    return rule;
            }

            rule = new HtmlSanitizerTagRule(tagName);
            sanitizer.Rules.Add(tagName, rule);
            return rule;
        }

        /// <summary>
        /// Removes the tag and it's contents of the tag matched by this rule.
        /// </summary>
        /// <param name="rule"></param>
        public static void Remove(this HtmlSanitizerTagRule rule)
        {
            rule.Operation = SanitizerOperation.RemoveTag;
        }

        /// <summary>
        /// Removes the tag but preserves it's contents in place of the tag matched by this rule.
        /// </summary>
        /// <param name="rule"></param>
        public static void Flatten(this HtmlSanitizerTagRule rule)
        {
            rule.Operation = SanitizerOperation.FlattenTag;
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
        /// Specifies that the given check is be performed on any attribute with the given name.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="attribute"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static HtmlSanitizerTagRule CheckAttribute(this HtmlSanitizerTagRule rule, string attribute, HtmlSanitizerCheckType check)
        {
            rule.CheckAttributes[attribute] = check;
            return rule;
        }

        /// <summary>
        /// Specifies that the specified space seperated list of attributes are allowed on this tag.
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static HtmlSanitizerTagRule AllowAttributes(this HtmlSanitizerTagRule rule, string attributes)
        {
            foreach (var attribute in attributes.Split(' '))
            {
                var trimmed = attribute.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                rule.CheckAttributes[trimmed] = HtmlSanitizerCheckType.AllowAttribute;
            }

            return rule;
        }

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
        /// Specifies the operation to perform if this node does not have any attributes set.
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static HtmlSanitizerTagRule NoAttributes(this HtmlSanitizerTagRule rule, SanitizerOperation operation)
        {
            rule.NoAttributesOperation = operation;
            return rule;
        }
    }
}