using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    public class AttributeCheckTests
    {

        /// <summary>
        /// Tests if obviously illegal URL's are caught while obviously legal ones are left alone.
        /// </summary>
        [Fact]
        public void UrlCheckTest()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("a").CheckAttribute("href", HtmlSanitizerCheckType.Url);

            // Test some illegal href
            var inputIllegal = @"<a href=""javascript:alert('test')"">That XSS trick</a>";
            var expectedIllegal = @"That XSS trick";
            result = sanitizer.Sanitize(inputIllegal);
            Assert.Equal(expectedIllegal, result);

            // Test a legal well formed url
            var inputLegal = @"<a href=""http://www.google.com/"">Legal link</a>";
            result = sanitizer.Sanitize(inputLegal);
            Assert.Equal(inputLegal, result);
        }

        /// <summary>
        /// Tests if empty attributes are left alone.
        /// </summary>
        [Fact]
        public void EmptyAttributeTest()
        {

            string result;

            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("input").AllowAttributes("disabled value");

            var input = @"<input disabled value=""test"">";
            var expected = @"<input disabled="""" value=""test"">";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Checks if unescaped values in attributes are correctly cleaned up.
        /// </summary>
        [Fact]
        public void UnescapedAttributeTest()
        {

            string result;

            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("span").AllowAttributes("style");

            var input = @"<span style=""<strong>Whats this?</strong>"">Text</span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(input, result);
        }
    }
}
