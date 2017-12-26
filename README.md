HtmlRuleSanitizer
=================

[![NuGet version](https://badge.fury.io/nu/Vereyon.Web.HtmlSanitizer.svg)](http://badge.fury.io/nu/Vereyon.Web.HtmlSanitizer)

HtmlRuleSanitizer is a white list rule based HTML sanitizer built on top of the HTML Agility Pack.

```C#
var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```

Without configuration HtmlRuleSanitizer will strip absolutely everything. This ensures that you are in control of what HTML is getting through. It was inspired by the client side parser of the wysihtml5 editor.

Use cases
---------

HtmlRuleSanitizer was designed with the following use cases in mind:

 * Prevent cross-site scripting (XSS) attacks by removing javascript and other malicious HTML fragments.
 * Restrict HTML to simple markup in order to allow for easy transformation to other document types without having to deal with all possible HTML tags.
 * Enforce nofollow on links to discourage link spam.
 * Cleanup submitted HTML by removing empty tags for example.
 * Restrict HTML to a limited set of tags, for example in a comment system.

Features
--------

 * CSS class white listing
 * Empty tag removal
 * Tag white listing
 * Tag attribute and CSS class enforcement
 * Tag flattening to simplify document structure while maintaining content
 * Tag renaming
 * Attribute checks (e.g. URL validity) and white listing
 * A fluent style configuration interface
 * HTML entity encoding
 
Usage
-----

Install the [HtmlRuleSanitizer NuGet package](https://www.nuget.org/packages/Vereyon.Web.HtmlSanitizer/).
Optionally add the following ```using``` statement in the file where you intend to use HtmlRuleSanitizer:

```C#
using Vereyon.Web;
```

### Basic usage

```C#
var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```

*Note: the SimpleHtml5Sanitizer returns a rule set which does not allow for a full document definition. Use SimpleHtml5DocumentSanitizer*

### Sanitize a document

```C#
var sanitizer = HtmlSanitizer.SimpleHtml5DocumentSanitizer();
string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```

### Configuration

The code below demonstrates how to configure a rule set which only allows **strong**, **i** and **a** tags and which enforces the link tags to have a valid url, be no-follow and open in a new window. In addition, any **b** tag is renamed to **strong** because they more or less do the same anyway and **b** is deprecated. Any empty tags are removed to get rid of them. This would be a nice example for comment processing.

```C#
var sanitizer = new HtmlSanitizer();
sanitizer.Tag("strong").RemoveEmpty();
sanitizer.Tag("b").Rename("strong").RemoveEmpty();
sanitizer.Tag("i").RemoveEmpty();
sanitizer.Tag("a").SetAttribute("target", "_blank")
	.SetAttribute("rel", "nofollow")
	.CheckAttribute("href", HtmlSanitizerCheckType.Url)
	.RemoveEmpty();

string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```



Tests
-----

Got tests? Yes, see the tests project. It uses xUnit.


More information
-----

 * [HtmlRuleSanitizer NuGet package](https://www.nuget.org/packages/Vereyon.Web.HtmlSanitizer/)
 * [CodeProject article on HtmlRuleSanitizer](http://www.codeproject.com/Articles/879381/Rule-based-HTML-sanitizer)
 * [Used in AlertA Contract Management](http://www.alert.eu)

License
-------

[MIT X11](http://en.wikipedia.org/wiki/MIT_License)
