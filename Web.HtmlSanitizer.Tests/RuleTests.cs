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
                .CheckAttributeUrl("href")
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

        /// <summary>
        /// Tests is the RemoveEmpty instruction on tag rules is applied as expected.
        /// </summary>
        [Fact]
        public void RemoveEmptyTest()
        {

            string input, result, expected;

            var sanitizer = new HtmlSanitizer();
            sanitizer.WhiteListMode = true;
            sanitizer.Tag("span").RemoveEmpty();
            sanitizer.Tag("font").RemoveEmpty().AllowAttributes("id");

            // Simply empty tags must be removed if instructed to do so.
            input = @"<span>Test test<font></font></span>";
            expected = @"<span>Test test</span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);

            // While empty content wise, it does have attributes. It should not simply be removed.
            input = @"<span>Test test<font id=""test-id""></font></span>";
            expected = @"<span>Test test<font id=""test-id""></font></span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);

            // The font tag is not really empty as it contains a space.
            input = @"<span>Test test<font> </font></span>";
            expected = @"<span>Test test<font> </font></span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);

            // The font tag is not really empty as it contains a linebreak.
            input = @"<span>Test test<font>
</font></span>";
            expected = @"<span>Test test<font>
</font></span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);

            // The unclosed font tag should not show up at all, and at least be removed because it is empty.
            input = @"<span>Test test<font></span>";
            expected = @"<span>Test test</span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests is the RemoveEmpty instruction on tag rules is applied as expected in cascading cases.
        /// </summary>
        [Fact]
        public void RemoveEmptyCascadedTest()
        {

            string input, result, expected;

            var sanitizer = new HtmlSanitizer();
            sanitizer.WhiteListMode = true;
            sanitizer.Tag("span").RemoveEmpty();
            sanitizer.Tag("font").RemoveEmpty().AllowAttributes("id"); 

            // The font tag is empty and will be removed. This results in the span tag also being empty, which would
            // lead one to expect the span tag to be removed completely.
            input = @"<span><font></font></span>";
            expected = @"";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }
    }
}
