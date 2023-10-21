using HtmlAgilityPack;
using System;
using Xunit;

namespace Vereyon.Web
{
    public class ElementSanitizationTests
    {
        [Fact]
        public void WrapElement()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("p");
            sanitizer.Tag("span").Sanitize(new CustomSanitizer(element =>
            {
                var wrapper = element.OwnerDocument.CreateElement("div");
                element.ParentNode.ReplaceChild(wrapper, element);
                wrapper.AppendChild(element);
                return SanitizerOperation.DoNothing;
            }));

            var input = "<p><span>Text</span></p>";
            var expected = "<p><div><span>Text</span></div></p>";
            var result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RemoveElementBasedOnContent()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("span").Sanitize(new CustomSanitizer(element =>
            {
                return element.InnerText == "remove me"
                    ? SanitizerOperation.RemoveTag
                    : SanitizerOperation.DoNothing;
            }));

            var input = "<span>keep me</span><span>remove me</span>";
            var expected = "<span>keep me</span>";
            var result = sanitizer.Sanitize(input);
            Assert.Equal(expected, result);
        }


        class CustomSanitizer : IHtmlElementSanitizer
        {
            private readonly Func<HtmlNode, SanitizerOperation> sanitize;

            public CustomSanitizer(Func<HtmlNode, SanitizerOperation> sanitize)
            {
                this.sanitize = sanitize;
            }

            public SanitizerOperation SanitizeElement(HtmlNode element, HtmlSanitizerTagRule tagRule)
            {
                return sanitize(element);
            }
        }
    }
}
