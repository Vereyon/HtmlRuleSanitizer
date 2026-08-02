# Introduction

While there are a host of HTML sanitizers out there, I had the need for a server side HTML sanitizer which was conservative, used a white list and still would allow relatively complete HTML markup. I was using the wysihtml5 editor to enable document editing and was struck by its nice client side HTML sanitizer, but I needed something like that on the server side which was also configurable.

This HTML sanitizer, aptly called HtmlRuleSanitizer aims to provide exactly that. It's built on top of the [HTML Agility Pack (HAP)](http://htmlagilitypack.codeplex.com/) to perform HTML DOM parsing and manipulation.

Sanitizing HTML using HtmlRuleSanitizer is dead simple. Using the predefined sanitizer for simple HTML5 code usage amounts to the following two lines of code:

```csharp
var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```

It will take this obviously dirty HTML...

```html
<h1>Heading</h1>
<p onclick="alert('gotcha!')">Some comments<span></span></p>
<script type="text/javascript">Illegal script()</script>
<p><a href="http://www.google.com/">Nofollow legal link</a> and here's another one:
<a href="javascript:alert('test')">Obviously I'm illegal</a></p>
```

...and turn it into this clean and safe HTML:

```html
<h1>Heading</h1>
<p>Some comments</p>
    
<p><a href="http://www.google.com/" target="_blank" rel="nofollow">Nofollow legal link</a> 
and here&#39;s another one:
Obviously I&#39;m illegal</p>
```

In the remainder of this article I'll explain my approach to developing this HTML sanitizer. The articles concludes with a more detailed usage explanation and a summary of points of interest.

## Background

While working on a contract management application, I wanted my users to be able to edit basic HTML documents of which the structure is a known subset of the HTML5 standard. The wysihtml5 editor provided a nice client side implementation of this all, but I needed solid server side enforcement as well.

Because of the requirement to allow a certain subset of HTML5 to pass through, while still stripping out all of the nasty stuff, most existing sanitation solutions did not suffice. [Microsoft's Web Protection Library (formerly AntiXSS)](https://wpl.codeplex.com/) for example seems to strip almost all HTML tags, making it unsuitable for my use. In addition it is not configurable and considered end of life.

The wysihtml5 editor has a white list and rule based client side HTML sanitizer which I really liked. Since it did not seem like too much effort to implement it on the server side, I decided to give it a try. HtmlRuleSanitizer is the result.

## Tag Whitelisting

The first requirement was to be able to get rid of absolutely all HTML tags, except for the ones that fitted my document structure. By doing so, I wanted to ensure that I could later on easily transform the document to other formats like RTF, Word and PDF without suddenly being posed with all kinds of difficult elements for which no direct equivalent is available. This feature is also highly desirable in for example comment systems where you'll only want to allow a few HTML tags like `<a>`, `<strong>` and `<em>` to be used.

A tag is whitelisted by creating a rule for it:

```csharp
sanitizer.Tag("p");
```

## Tag Flattening

When the user uses some kind of tag that I do not like, it may be overkill to simply kick out the tag and its contents completely. To this purpose, tag flattening was built in: the tag itself is removed, but its contents are preserved in place. In this manner, one could for example get rid of needless `<div>` elements wrapping content, while preserving that very content:

```csharp
sanitizer.Tag("div").NoAttributes(SanitizerOperation.FlattenTag);
```

## Sanitation

While sanitation usually refers to the complete process of checking and cleaning HTML, I also wanted to avoid a common problem which involves a browser editor leaving empty tags. Clicking the bold button twice does in some editors result in a tag `<b></b>` remaining. Thanks but no thanks, let's get rid of that. Removal of an empty tag which is on the white list is done as follows:

```csharp
sanitizer.Tag("strong").RemoveEmpty();
```

Sanitation is performed in two steps: an 'downstream' step during which the sanitizer traverses ever more deep into the document tree removing non whitelisted nodes and empty nodes. The seconds step is 'upstream' during which every node is again checked if it is empty while the sanitizer traverses back up the document tree. This is required because the downstream step may result in upstream nodes becoming empty due to their child tags being removed.

## CSS Whitelisting

Since the `<center>` tag is deprecated in HTML5 and I also want to be able to align text to the right, I needed some CSS classes to be able to pass through. Again, absolutely everything else should be kept out, so we use a whitelist.

```csharp
sanitizer.AllowCss("legal-css");
```

## Tag Renaming

What to do with people who still manage to submit documents containing for example `<b>` tags? The use of the `<b>` is discouraged and I do not want to have to deal with both <strong> and <b> tags when transforming the document at some later stage. To this end, HtmlSanitizer is equipped with a tag renaming registry.


```csharp
sanitizer.Tag("b").Rename("strong");
```

## Attribute Enforcement and Checks

The users of the software I was working on are allowed to put in links as many as they want, but no trickery! Every link needs to be nofollow and needs to open in a new window.

I only needed one type of attribute to be checked: the href attribute of links. Only links with a valid url and an allowed URI scheme (no `javascript:blbla` funny business) are to be allowed. For extensibility, I added an attribute check registry in which attribute check callbacks can be registered.

```csharp
sanitizer.Tag("a").SetAttribute("target", "_blank")
    .SetAttribute("rel", "nofollow")
    .CheckAttribute("href", HtmlSanitizerCheckType.Url)
    .RemoveEmpty()
    .NoAttributes(SanitizerOperation.FlattenTag);
```

Note that the list line in the above code contain another goodie. An `<a>` tag without any attributes remaining is obviously rubbish, so we can instruct the sanitizer to flatten it.

## Attribute Whitelisting

Another potential danger is failing to strip attributes like onclick. For this reason any attribute for which no check or override is configured is removed. The class attribute is the only exemption to this. White listing additional attributes is possible using the AllowAttributes method:

```csharp
sanitizer.Tag("span").AllowAttributes("style");
```

## HTML Entity Encoding

The final step in protecting against XSS attacks is enforcing HTML entities to be encoded where they should be. Because the sanitizer completely parses the input HTML there should in principle never be any problem with any HTML entity trickery. In case (deliberate) failure to encode all HTML entities causes the parser to incorrectly parse the HTML this will simply result in tags being completely missed or removed due to the white listing approach. In addition the sanitizer does not evaluate any scripts so vulnerability to deliberate attacks on the sanitizer itself should be very limited.

This does however not mean that any other parses or program which will later use the sanitized HTML is not vulnerable to suchs attacks. In addition HTML with non encoded entities is simply not valid HTML. On the other hand HTML entity encoding is not as trivial as simply running all the HTML through a single encoding method.

The sanitizer relies on the standard .NET framework WebUtility class for HTML entity encoding. By default HTML entity encoding is enforced on all text portions of the HTML document. In order to prevent double encoding of correctly encoded entity first all entities are decoded. Next the text node entities are encoded and the text node is replaced. The resulting fragment of code looks as follows:

```csharp
if (node.NodeType == HtmlNodeType.Text && EncodeHtmlEntities)
{
    var deentitized = WebUtility.HtmlDecode(node.InnerText);
    var entitized = WebUtility.HtmlEncode(deentitized);
    var replacement = HtmlTextNode.CreateNode(entitized);
    node.ParentNode.ReplaceChild(replacement, node);
    return;
}
```

## Configuration

I wanted the `HtmlSanitizer` to be easy to configure. I wrote a small fluent style configuration interface using extension methods. This interface is defined in the `HtmlSanitizerFluentHelper` class. This interface is extensively used in the above examples.

# Using HtmlRuleSanitizer

The first thing you need to do to be able to use the sanitizer after downloading the sanitizer, is to download the Html Agility Pack (HAP). Either get it at their codeplex website, or get their [NuGet package](http://www.nuget.org/packages/HtmlAgilityPack/). If you use the HtmlRuleSanitizer NuGet package, the Html Agility Pack will be installed for you.

`HtmlRuleSanitizer` comes with two configuration presets. Using the predefined sanitizer for simple HTML5 code usage amounts to the following two lines of code:

```
var sanitizer = HtmlSanitizer.SimpleHtml5Sanitizer();
string cleanHtml = sanitizer.Sanitize(dirtyHtml);
```

When you want to sanitize a document which includes `<html>` and `<body>`, then use the `SimpleHtml5DocumentSanitizer`:

```
var sanitizer = HtmlSanitizer.SimpleHtml5DocumentSanitizer();
string cleanHtmlDoc = sanitizer.Sanitize(dirtyHtmlDoc);
```

## Configuration

The simple HTML5 sanitation rule set is defined as follows. This serves as a good example on how to configure more complete rule sets.

```csharp
var sanitizer = new HtmlSanitizer();

sanitizer.WhiteListMode = true;
sanitizer.Tag("h1").RemoveEmpty();
sanitizer.Tag("h2").RemoveEmpty();
sanitizer.Tag("h3").RemoveEmpty();
sanitizer.Tag("h4").RemoveEmpty();
sanitizer.Tag("h5").RemoveEmpty();
sanitizer.Tag("strong").RemoveEmpty();
sanitizer.Tag("b").Rename("strong").RemoveEmpty();
sanitizer.Tag("i").RemoveEmpty();
sanitizer.Tag("em");
sanitizer.Tag("br");
sanitizer.Tag("p");
sanitizer.Tag("div").NoAttributes(SanitizerOperation.FlattenTag);
sanitizer.Tag("span").RemoveEmpty();
sanitizer.Tag("ul");
sanitizer.Tag("ol");
sanitizer.Tag("li");
sanitizer.Tag("a").SetAttribute("target", "_blank")
    .SetAttribute("rel", "nofollow")
    .CheckAttribute("href", HtmlSanitizerCheckType.Url)
    .RemoveEmpty()
    .NoAttributes(SanitizerOperation.FlattenTag);
```

You are free to define any new configuration or extend existing configuration using the fluent configuration interface.

# Points of Interest

What good is a HTML sanitizer without any tests? While I cannot completely guarantee this sanitizer protecting you from any cross-site scripting and other trickery, I did add unit tests to backup my claim that it's working. In addition, I would be happy to hear suggestions from anyone able to find weaknesses. Some of the tests included are taken from OWASP which proved to be a valuable source of information on XSS attacks.

While searching for a solution to my original problem, I did come accross this sanitizer from mganss which seems like a very good alternative to the HTML sanitizer presented here. It has many of the same features, but uses a library called CsQuery for HTML DOM parsing.

# History

Find the latest version, see https://github.com/Vereyon/HtmlRuleSanitizer

8/13/2016: Version 1.2.0: Implemented HTML entity encoding and cascading empty node removal.
6/17/2015: Version 1.1.0: Added tag attribute white listing and additional unit tests.