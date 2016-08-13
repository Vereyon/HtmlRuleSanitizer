using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    /// <summary>
    /// Set of tests checking the functioning of the sanitizer on 'real world' documents.
    /// </summary>
    public class UseCaseTests
    {

        /// <summary>
        /// Tests the sanitation of a simple MS Word document saved as HTML.
        /// </summary>
        [Fact]
        public void SanitizeMsWordDocument()
        {

            var sanitizer = HtmlSanitizer.SimpleHtml5DocumentSanitizer();

            string input = @"<html xmlns:v=""urn:schemas-microsoft-com:vml""
xmlns:o=""urn:schemas-microsoft-com:office:office""
xmlns:w=""urn:schemas-microsoft-com:office:word""
xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml""
xmlns=""http://www.w3.org/TR/REC-html40"">

<head>
<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">
<meta name=ProgId content=Word.Document>
<meta name=Generator content=""Microsoft Word 12"">
<meta name=Originator content=""Microsoft Word 12"">
<link rel=File-List href=""Test_files/filelist.xml"">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>Akkermans</o:Author>
  <o:LastAuthor>Akkermans</o:LastAuthor>
  <o:Revision>3</o:Revision>
  <o:TotalTime>6</o:TotalTime>
  <o:Created>2015-07-08T08:39:00Z</o:Created>
  <o:LastSaved>2015-07-08T08:45:00Z</o:LastSaved>
  <o:Pages>1</o:Pages>
  <o:Words>14</o:Words>
  <o:Characters>77</o:Characters>
  <o:Lines>1</o:Lines>
  <o:Paragraphs>1</o:Paragraphs>
  <o:CharactersWithSpaces>90</o:CharactersWithSpaces>
  <o:Version>12.00</o:Version>
 </o:DocumentProperties>
</xml><![endif]-->
<link rel=themeData href=""Test_files/themedata.thmx"">
<link rel=colorSchemeMapping href=""Test_files/colorschememapping.xml"">
<!--[if gte mso 9]><xml>
 <w:WordDocument>
  <w:Zoom>120</w:Zoom>
  <w:SpellingState>Clean</w:SpellingState>
  <w:GrammarState>Clean</w:GrammarState>
  <w:TrackMoves>false</w:TrackMoves>
  <w:TrackFormatting/>
  <w:HyphenationZone>21</w:HyphenationZone>
  <w:PunctuationKerning/>
  <w:ValidateAgainstSchemas/>
  <w:SaveIfXMLInvalid>false</w:SaveIfXMLInvalid>
  <w:IgnoreMixedContent>false</w:IgnoreMixedContent>
  <w:AlwaysShowPlaceholderText>false</w:AlwaysShowPlaceholderText>
  <w:DoNotPromoteQF/>
  <w:LidThemeOther>NL</w:LidThemeOther>
  <w:LidThemeAsian>X-NONE</w:LidThemeAsian>
  <w:LidThemeComplexScript>X-NONE</w:LidThemeComplexScript>
  <w:Compatibility>
   <w:BreakWrappedTables/>
   <w:SnapToGridInCell/>
   <w:WrapTextWithPunct/>
   <w:UseAsianBreakRules/>
   <w:DontGrowAutofit/>
   <w:SplitPgBreakAndParaMark/>
   <w:DontVertAlignCellWithSp/>
   <w:DontBreakConstrainedForcedTables/>
   <w:DontVertAlignInTxbx/>
   <w:Word11KerningPairs/>
   <w:CachedColBalance/>
  </w:Compatibility>
  <w:BrowserLevel>MicrosoftInternetExplorer4</w:BrowserLevel>
  <m:mathPr>
   <m:mathFont m:val=""Cambria Math""/>
   <m:brkBin m:val=""before""/>
   <m:brkBinSub m:val=""&#45;-""/>
   <m:smallFrac m:val=""off""/>
   <m:dispDef/>
   <m:lMargin m:val=""0""/>
   <m:rMargin m:val=""0""/>
   <m:defJc m:val=""centerGroup""/>
   <m:wrapIndent m:val=""1440""/>
   <m:intLim m:val=""subSup""/>
   <m:naryLim m:val=""undOvr""/>
  </m:mathPr></w:WordDocument>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <w:LatentStyles DefLockedState=""false"" DefUnhideWhenUsed=""true""
  DefSemiHidden=""true"" DefQFormat=""false"" DefPriority=""99""
  LatentStyleCount=""267"">
  <w:LsdException Locked=""false"" Priority=""0"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Normal""/>
  <w:LsdException Locked=""false"" Priority=""9"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""heading 1""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 2""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 3""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 4""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 5""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 6""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 7""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 8""/>
  <w:LsdException Locked=""false"" Priority=""9"" QFormat=""true"" Name=""heading 9""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 1""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 2""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 3""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 4""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 5""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 6""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 7""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 8""/>
  <w:LsdException Locked=""false"" Priority=""39"" Name=""toc 9""/>
  <w:LsdException Locked=""false"" Priority=""35"" QFormat=""true"" Name=""caption""/>
  <w:LsdException Locked=""false"" Priority=""10"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Title""/>
  <w:LsdException Locked=""false"" Priority=""1"" Name=""Default Paragraph Font""/>
  <w:LsdException Locked=""false"" Priority=""11"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Subtitle""/>
  <w:LsdException Locked=""false"" Priority=""22"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Strong""/>
  <w:LsdException Locked=""false"" Priority=""20"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Emphasis""/>
  <w:LsdException Locked=""false"" Priority=""59"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Table Grid""/>
  <w:LsdException Locked=""false"" UnhideWhenUsed=""false"" Name=""Placeholder Text""/>
  <w:LsdException Locked=""false"" Priority=""1"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""No Spacing""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 1""/>
  <w:LsdException Locked=""false"" UnhideWhenUsed=""false"" Name=""Revision""/>
  <w:LsdException Locked=""false"" Priority=""34"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""List Paragraph""/>
  <w:LsdException Locked=""false"" Priority=""29"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Quote""/>
  <w:LsdException Locked=""false"" Priority=""30"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Intense Quote""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 1""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 2""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 3""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 4""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 5""/>
  <w:LsdException Locked=""false"" Priority=""60"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Shading Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""61"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light List Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""62"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Light Grid Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""63"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 1 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""64"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Shading 2 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""65"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 1 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""66"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium List 2 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""67"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 1 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""68"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 2 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""69"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Medium Grid 3 Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""70"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Dark List Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""71"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Shading Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""72"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful List Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""73"" SemiHidden=""false""
   UnhideWhenUsed=""false"" Name=""Colorful Grid Accent 6""/>
  <w:LsdException Locked=""false"" Priority=""19"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Subtle Emphasis""/>
  <w:LsdException Locked=""false"" Priority=""21"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Intense Emphasis""/>
  <w:LsdException Locked=""false"" Priority=""31"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Subtle Reference""/>
  <w:LsdException Locked=""false"" Priority=""32"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Intense Reference""/>
  <w:LsdException Locked=""false"" Priority=""33"" SemiHidden=""false""
   UnhideWhenUsed=""false"" QFormat=""true"" Name=""Book Title""/>
  <w:LsdException Locked=""false"" Priority=""37"" Name=""Bibliography""/>
  <w:LsdException Locked=""false"" Priority=""39"" QFormat=""true"" Name=""TOC Heading""/>
 </w:LatentStyles>
</xml><![endif]-->
<style>
<!--
 /* Font Definitions */
 @font-face
	{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;
	mso-font-charset:0;
	mso-generic-font-family:roman;
	mso-font-pitch:variable;
	mso-font-signature:-536870145 1107305727 0 0 415 0;}
@font-face
	{font-family:Cambria;
	panose-1:2 4 5 3 5 4 6 3 2 4;
	mso-font-charset:0;
	mso-generic-font-family:roman;
	mso-font-pitch:variable;
	mso-font-signature:-536870145 1073743103 0 0 415 0;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;
	mso-font-charset:0;
	mso-generic-font-family:swiss;
	mso-font-pitch:variable;
	mso-font-signature:-536870145 1073786111 1 0 415 0;}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{mso-style-unhide:no;
	mso-style-qformat:yes;
	mso-style-parent:"""";
	margin-top:0cm;
	margin-right:0cm;
	margin-bottom:10.0pt;
	margin-left:0cm;
	line-height:115%;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:minor-bidi;
	mso-fareast-language:EN-US;}
h1
	{mso-style-priority:9;
	mso-style-unhide:no;
	mso-style-qformat:yes;
	mso-style-link:""Heading 1 Char"";
	mso-style-next:Normal;
	margin-top:24.0pt;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:0cm;
	margin-bottom:.0001pt;
	line-height:115%;
	mso-pagination:widow-orphan lines-together;
	page-break-after:avoid;
	mso-outline-level:1;
	font-size:14.0pt;
	font-family:""Cambria"",""serif"";
	mso-ascii-font-family:Cambria;
	mso-ascii-theme-font:major-latin;
	mso-fareast-font-family:""Times New Roman"";
	mso-fareast-theme-font:major-fareast;
	mso-hansi-font-family:Cambria;
	mso-hansi-theme-font:major-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:major-bidi;
	color:#365F91;
	mso-themecolor:accent1;
	mso-themeshade:191;
	mso-font-kerning:0pt;
	mso-fareast-language:EN-US;}
h2
	{mso-style-priority:9;
	mso-style-qformat:yes;
	mso-style-link:""Heading 2 Char"";
	mso-style-next:Normal;
	margin-top:10.0pt;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:0cm;
	margin-bottom:.0001pt;
	line-height:115%;
	mso-pagination:widow-orphan lines-together;
	page-break-after:avoid;
	mso-outline-level:2;
	font-size:13.0pt;
	font-family:""Cambria"",""serif"";
	mso-ascii-font-family:Cambria;
	mso-ascii-theme-font:major-latin;
	mso-fareast-font-family:""Times New Roman"";
	mso-fareast-theme-font:major-fareast;
	mso-hansi-font-family:Cambria;
	mso-hansi-theme-font:major-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:major-bidi;
	color:#4F81BD;
	mso-themecolor:accent1;
	mso-fareast-language:EN-US;}
a:link, span.MsoHyperlink
	{mso-style-priority:99;
	color:blue;
	mso-themecolor:hyperlink;
	text-decoration:underline;
	text-underline:single;}
a:visited, span.MsoHyperlinkFollowed
	{mso-style-noshow:yes;
	mso-style-priority:99;
	color:purple;
	mso-themecolor:followedhyperlink;
	text-decoration:underline;
	text-underline:single;}
span.Heading1Char
	{mso-style-name:""Heading 1 Char"";
	mso-style-priority:9;
	mso-style-unhide:no;
	mso-style-locked:yes;
	mso-style-link:""Heading 1"";
	mso-ansi-font-size:14.0pt;
	mso-bidi-font-size:14.0pt;
	font-family:""Cambria"",""serif"";
	mso-ascii-font-family:Cambria;
	mso-ascii-theme-font:major-latin;
	mso-fareast-font-family:""Times New Roman"";
	mso-fareast-theme-font:major-fareast;
	mso-hansi-font-family:Cambria;
	mso-hansi-theme-font:major-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:major-bidi;
	color:#365F91;
	mso-themecolor:accent1;
	mso-themeshade:191;
	font-weight:bold;}
span.Heading2Char
	{mso-style-name:""Heading 2 Char"";
	mso-style-priority:9;
	mso-style-unhide:no;
	mso-style-locked:yes;
	mso-style-link:""Heading 2"";
	mso-ansi-font-size:13.0pt;
	mso-bidi-font-size:13.0pt;
	font-family:""Cambria"",""serif"";
	mso-ascii-font-family:Cambria;
	mso-ascii-theme-font:major-latin;
	mso-fareast-font-family:""Times New Roman"";
	mso-fareast-theme-font:major-fareast;
	mso-hansi-font-family:Cambria;
	mso-hansi-theme-font:major-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:major-bidi;
	color:#4F81BD;
	mso-themecolor:accent1;
	font-weight:bold;}
span.SpellE
	{mso-style-name:"""";
	mso-spl-e:yes;}
.MsoChpDefault
	{mso-style-type:export-only;
	mso-default-props:yes;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-fareast-font-family:Calibri;
	mso-fareast-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:""Times New Roman"";
	mso-bidi-theme-font:minor-bidi;
	mso-fareast-language:EN-US;}
.MsoPapDefault
	{mso-style-type:export-only;
	margin-bottom:10.0pt;
	line-height:115%;}
@page WordSection1
	{size:595.3pt 841.9pt;
	margin:70.85pt 70.85pt 70.85pt 70.85pt;
	mso-header-margin:35.4pt;
	mso-footer-margin:35.4pt;
	mso-paper-source:0;}
div.WordSection1
	{page:WordSection1;}
-->
</style>
<!--[if gte mso 10]>
<style>
 /* Style Definitions */
 table.MsoNormalTable
	{mso-style-name:""Table Normal"";
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-qformat:yes;
	mso-style-parent:"""";
	mso-padding-alt:0cm 5.4pt 0cm 5.4pt;
	mso-para-margin-top:0cm;
	mso-para-margin-right:0cm;
	mso-para-margin-bottom:10.0pt;
	mso-para-margin-left:0cm;
	line-height:115%;
	mso-pagination:widow-orphan;
	font-size:11.0pt;
	font-family:""Calibri"",""sans-serif"";
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-fareast-language:EN-US;}
</style>
<![endif]--><!--[if gte mso 9]><xml>
 <o:shapedefaults v:ext=""edit"" spidmax=""2050""/>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <o:shapelayout v:ext=""edit"">
  <o:idmap v:ext=""edit"" data=""1""/>
 </o:shapelayout></xml><![endif]-->
</head>

<body lang=NL link=blue vlink=purple style='tab-interval:35.4pt'>

<div class=WordSection1>

<h1><span class=SpellE>Heading</span> <span class=SpellE>one</span></h1>

<p class=MsoNormal><span class=SpellE>Paragraph</span></p>

<h2><span class=SpellE>Heading</span> <span class=SpellE>two</span></h2>

<p class=MsoNormal><span class=SpellE><b style='mso-bidi-font-weight:normal'>Bold</b></span><b
style='mso-bidi-font-weight:normal'><o:p></o:p></b></p>

<p class=MsoNormal><span class=SpellE><i style='mso-bidi-font-style:normal'>Italic</i></span><i
style='mso-bidi-font-style:normal'><o:p></o:p></i></p>

<p class=MsoNormal><i style='mso-bidi-font-style:normal'><a
href=""http://www.vereyon.com/"">Link</a><o:p></o:p></i></p>

</div>

</body>

</html>
";
            string expected = @"<html>



<body>



<h1><span>Heading</span> <span>one</span></h1>

<p><span>Paragraph</span></p>

<h2><span>Heading</span> <span>two</span></h2>

<p><span><strong>Bold</strong></span></p>

<p><span><i>Italic</i></span></p>

<p><i><a href=""http://www.vereyon.com/"" target=""_blank"" rel=""nofollow"">Link</a></i></p>



</body>

</html>";

            var output = sanitizer.Sanitize(input);
            Assert.Equal(expected, output);
        }
    }
}
