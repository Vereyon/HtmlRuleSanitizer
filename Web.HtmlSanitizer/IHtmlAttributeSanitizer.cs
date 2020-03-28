﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vereyon.Web
{
    /// <summary>
    /// Defines the interface for attribute checks.
    /// </summary>
    public interface IHtmlAttributeSanitizer
    {

        /// <summary>
        /// Checks the passed attribute.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="tagRule"></param>
        /// <returns></returns>
        SanitizerOperation SanitizeAttribute(HtmlAttribute attribute, HtmlSanitizerTagRule tagRule);
    }
}
