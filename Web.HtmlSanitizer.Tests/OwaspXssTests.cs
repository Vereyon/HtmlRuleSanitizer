using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{

    /// <summary>
    /// Testcase taken from OWASP.
    /// https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet
    /// </summary>
    public class OwaspXssTests
    {

        [Fact]
        public void EmbeddedTab()
        {

            string input, result, expected;

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.Tag("img");

            input = @"<IMG SRC=""jav ascript:alert('XSS'); "">";
            expected = @"<img>";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void BreakoutSrcCheck()
        {

            string input, result, expected;

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.Tag("img").CheckAttributeUrl("src");

            input = @"<IMG SRC =# onmouseover=""alert('xxs')"">";
            expected = @"";
            result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }
        
    }
}
