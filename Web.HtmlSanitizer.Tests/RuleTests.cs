using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    public class RuleTests
    {

        [Fact]
        public void RuleOperationTests()
        {

            var input = "before <span>content</span> after";
            var remove = "before  after";
            var flatten = "before content after";
            string result;

            var sanitizer = new HtmlSanitizer();

            // Test do nothing
            sanitizer.Tag("span").Operation(SanitizerOperation.DoNothing);
            result = sanitizer.Sanitize(input);
            Assert.Equal(input, result);

            // Test flatten
            sanitizer.Tag("span").Operation(SanitizerOperation.FlattenTag);
            result = sanitizer.Sanitize(input);
            Assert.Equal(flatten, result);

            // Test remove
            sanitizer.Tag("span").Operation(SanitizerOperation.RemoveTag);
            result = sanitizer.Sanitize(input);
            Assert.Equal(remove, result);
        }

        [Fact]
        public void TagRenameTest()
        {

            var input = "<b>before</b> <strong>content</strong> after<b></b>";
            var expected = "<strong>before</strong> <strong>content</strong> after";
            string result;

            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("strong").RemoveEmpty();
            sanitizer.Tag("b").Rename("strong").RemoveEmpty();
            sanitizer.Tag("i").RemoveEmpty();
            sanitizer.Tag("a").SetAttribute("target", "_blank")
                .SetAttribute("rel", "nofollow")
                .CheckAttribute("href", HtmlSanitizerCheckType.Url)
                .RemoveEmpty();

            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests tag flattening on more complex content.
        /// </summary>
        [Fact]
        public void TagFlattenTests()
        {

            var input = @"<p>Some prepended content</p>
<p>before <span><b>Preserve before</b> content <i>Preserve</i></span> after</p>
<p>Some trailing content</p>";
            var expected = @"<p>Some prepended content</p>
<p>before <b>Preserve before</b> content <i>Preserve</i> after</p>
<p>Some trailing content</p>";
            string result;

            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("span").Operation(SanitizerOperation.FlattenTag);
            sanitizer.Tag("i");
            sanitizer.Tag("b");
            sanitizer.Tag("p");

            // Test flatten
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }
    }
}
