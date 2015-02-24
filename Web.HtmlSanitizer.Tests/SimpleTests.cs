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
<p>Some comments<span></span></p>
<script type=""text/javascript"">I'm illegal for sure</script>
<p><a href=""http://www.vereyon.com/"">Nofollow legal link</a> and here's another one: <a href=""javascript:alert('test')"">Obviously I'm illegal</a></p>";
            string expected = @"<h1>Heading</h1>
<p>Some comments</p>
    
<p><a href=""http://www.vereyon.com/"" target=""_blank"" rel=""nofollow"">Nofollow legal link</a> and here's another one: Obviously I'm illegal</p>";

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
    
    <p><a href=""http://www.vereyon.com/"" target=""_blank"" rel=""nofollow"">Nofollow legal link</a> and here's another one: Obviously I'm illegal</p>
</body>
</html>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }
    }
}
