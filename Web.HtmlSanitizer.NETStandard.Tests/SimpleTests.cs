using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    public class SimpleTests
    {
        [Fact]
        public void SimpleSanitizerTests()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();

            string input = @"<h1>Heading</h1>
<p onclick=""alert('gotcha!')"">Some comments<span></span></p>
<script type=""text/javascript"">I'm illegal for sure</script>
<p><a href=""http://www.vereyon.com/"">Nofollow legal link</a> and here's another one: <a href=""javascript:alert('test')"">Obviously I'm illegal</a></p>";
            string expected = @"<h1>Heading</h1>
<p>Some comments</p>

<p><a href=""http://www.vereyon.com/"" target=""_blank"" rel=""nofollow"">Nofollow legal link</a> and here&#39;s another one: Obviously I&#39;m illegal</p>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        [Fact]
        public void SimpleDocumentSanitizerTests()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5DocumentSanitizer();

            string input = @"<html>
<body>
    <h1>Heading</h1>
    <p>Some comments<span></span></p>
    <script type=""text/javascript"">I'm illegal for sure</script>
    <p><a href=""http://www.vereyon.com/"">Nofollow legal link</a> and here's another one: <a href=""javascript:alert('test')"">Obviously I'm illegal</a></p>
</body>
</html>";
            string expected = @"<html>
<body>
    <h1>Heading</h1>
    <p>Some comments</p>
    
    <p><a href=""http://www.vereyon.com/"" target=""_blank"" rel=""nofollow"">Nofollow legal link</a> and here&#39;s another one: Obviously I&#39;m illegal</p>
</body>
</html>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Simple test to see if potentially dangerous attributes are stripped.
        /// </summary>
        [Fact]
        public void DirtyAttributesTest()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();

            string input = @"<p><span onclick=""alert('test')"">Test</span></p>";
            string expected = @"<p><span>Test</span></p>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Simple test to check if old skool capitalized html is handled.
        /// </summary>
        [Fact]
        public void CapitalizedHtml()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();

            string input = @"<p><SPAN ID=""1234abc"">Test</SPAN></p>";
            string expected = @"<p><span>Test</span></p>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Simple test to check if unclosed tags are cleaned up. Not sure if this is the desired behaviour, but at least the resulting HTML must be clean.
        /// </summary>
        [Fact]
        public void UnclosedTagsTest()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();

            string input = @"<div><strong>Not properly closed</div>";
            string expected = @"<strong>Not properly closed</strong>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);

            // Also test with an unclosed tag at the end.
            input = @"<div>The next tag is not properly closed<strong></div>";
            expected = @"The next tag is not properly closed";

            output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Tests if comment removal is working.
        /// </summary>
        [Fact]
        public void StripCommentsTest()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.RemoveComments = true;

            string input = @"Test <!-- No comment --> Test";
            string expected = @"Test  Test";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Tests if allowing comments works.
        /// </summary>
        [Fact]
        public void AllowCommentsTest()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.RemoveComments = false;

            string input = @"Test <!-- No comment --> Test";
            string expected = @"Test <!-- No comment --> Test";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }

        /// <summary>
        /// Tests if HTML entities are correctly escaped.
        /// https://www.w3.org/International/questions/qa-escapes#use
        /// </summary>
        [Fact]
        public void EscapeCharactersTest()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
            sanitizer.RemoveComments = false;

            // The extra greater than characters are going to get lost because the tags are malformed.
            // I would say this is sort of to be expected.
            string input = @"<<p>"">&lt;test<</p>"" test";
            string expected = @"<p>&quot;&gt;&lt;test</p>&quot; test";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }
    }
}
