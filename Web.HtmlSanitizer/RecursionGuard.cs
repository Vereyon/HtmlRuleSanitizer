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

		/// <summary>Initializes a new instance of the <see cref="RecursionGuard"/> class.</summary>
		/// <param name="sanitizer">The sanitizer.</param>
		/// <exception cref="System.InvalidOperationException">Maximum recursion depth execeeded.</exception>
		public RecursionGuard(HtmlSanitizer sanitizer)
        {

            _sanitizer = sanitizer;
            _sanitizer.Depth++;

            if (_sanitizer.Depth > _sanitizer.MaxRecursionDepth)
                throw new InvalidOperationException("Maximum recursion depth execeeded.");
        }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
        {
            _sanitizer.Depth--;
        }

		/// <summary>Gets the depth.</summary>
		/// <value>The depth.</value>
		public int Depth { get { return _sanitizer.Depth; } }
    }
}
