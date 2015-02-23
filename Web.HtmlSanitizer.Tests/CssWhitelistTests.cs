using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    public class CssWhitelistTests
    {

        [Fact]
        public void SimpleNoCssTest()
        {

            string input = @"<p class=""illegal"">Test content</p>Outside tag";
            string expected = @"<p>Test content</p>Outside tag";

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            var result = sanitizer.Sanitize(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void SimpleWhitelistCssTest()
        {

            string input = @"<p class=""illegal"">Test content</p>Outside tag";

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.AllowCss("illegal");
            var result = sanitizer.Sanitize(input);

            Assert.Equal(input, result);
        }

        [Fact]
        public void SimpleWhitelistCssRemoveTest()
        {

            string input = @"<p class=""illegal legal"">Test content</p>Outside tag";
            string expected = @"<p class=""legal"">Test content</p>Outside tag";

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.AllowCss("legal");
            var result = sanitizer.Sanitize(input);

            Assert.Equal(expected, result);
        }
    }
}
