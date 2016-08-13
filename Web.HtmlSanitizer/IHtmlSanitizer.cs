using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vereyon.Web
{
    /// <summary>
    /// The IHtmlSanitizer interface defines an HTML sanitation interface for protecting against XSS attacks and for transforming
    /// HTML formatted input.
    /// </summary>
    public interface IHtmlSanitizer
    {

        /// <summary>
        /// Sanitizes the passed HTML string and returns the sanitized HTML.
        /// </summary>
        /// <param name="html">A string containing HTML formatted text.</param>
        /// <returns>A string containing sanitized HTML formatted text.</returns>
        string Sanitize(string html);
    }
}
