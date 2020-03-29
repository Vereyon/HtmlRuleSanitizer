using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Xunit;

namespace Vereyon.Web
{
    public class AttributeSanitizationTests
    {

        /// <summary>
        /// Tests if basic attribute white listing is working.
        /// </summary>
        [Fact]
        public void AttributeWhiteListing()
        {

            string result;

            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("input").AllowAttributes("whitelisted");

            var input = @"<input nonwhitelisted="""" whitelisted="""">";
            var expected = @"<input whitelisted="""">";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests if obviously illegal URL's are caught while obviously legal ones are left alone.
        /// </summary>
        [Fact]
        public void AHrefUrlCheckTest()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("a").CheckAttributeUrl("href");

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
        /// Tests if obviously illegal URL's are caught while obviously legal ones are left alone.
        /// </summary>
        [Fact]
        public void AHrefUrlCheckTestLegacy()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("a").CheckAttributeUrl("href");

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
        /// Regression test for checking if relative URL's are accepted.
        /// </summary>
        [Fact]
        public void AHrefUrlCheckRelativeTest()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("a").CheckAttribute("href", HtmlSanitizerCheckType.Url);

            // Test a relative url, which should pass.
            var input = @"<a href=""../relative.htm"">Relative link</a>";
            var expected = @"<a href=""../relative.htm"">Relative link</a>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }


        /// <summary>
        /// Verifies the functioning of the URL check on src attributes.
        /// </summary>
        [Fact]
        public void ImgSrcUrlCheckTest()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("img").CheckAttributeUrl("src");

            // Test some illegal href
            var inputIllegal = @"<img src=""javascript:alert('test')"">";
            var expectedIllegal = @"";
            result = sanitizer.Sanitize(inputIllegal);
            Assert.Equal(expectedIllegal, result);

            // Test a legal well formed url
            var inputLegal = @"<img src=""http://www.google.com/a.png"">";
            result = sanitizer.Sanitize(inputLegal);
            Assert.Equal(inputLegal, result);
        }

        /// <summary>
        /// Regression test for checking if relative URL's are accepted.
        /// </summary>
        [Fact]
        public void ImgSrcUrlCheckRelativeTest()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("img").CheckAttributeUrl("src");

            // Test a relative url, which should pass.
            var input = @"<img src=""../relative.png"">";
            var expected = @"<img src=""../relative.png"">";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Tests if empty attributes are left untouched.
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

        /// <summary>
        /// Checks if unescaped values in attributes are correctly cleaned up.
        /// </summary>
        [Fact]
        public void CustomAttributeSanitization()
        {

            string result;

            var sanitizer = new HtmlSanitizer();
            var attributeSanitizer = new CustomSanitizer();
            sanitizer.Tag("span").SanitizeAttributes("style", attributeSanitizer);

            var input = @"<span style=""heading"">Text</span>";
            var expected = @"<span style=""123"">Text</span>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        class CustomSanitizer : IHtmlAttributeSanitizer
        {
            public SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule)
            {
                attribute.Value = "123";
                return SanitizerOperation.DoNothing;
            }
        }
    }
}
