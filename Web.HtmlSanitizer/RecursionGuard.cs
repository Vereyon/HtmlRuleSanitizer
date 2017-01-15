using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vereyon.Web
{
    /// <summary>
    /// Helper class for preventing stack overflows due to excessive recursion.
    /// </summary>
    public class RecursionGuard : IDisposable
    {

        private HtmlSanitizer _sanitizer;

        public RecursionGuard(HtmlSanitizer sanitizer)
        {

            _sanitizer = sanitizer;
            _sanitizer.Depth++;

            if (_sanitizer.Depth > _sanitizer.MaxRecursionDepth)
                throw new InvalidOperationException("Maximum recursion depth execeeded.");
        }

        public void Dispose()
        {
            _sanitizer.Depth--;
        }

        public int Depth { get { return _sanitizer.Depth; } }
    }
}
