using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Vereyon.Web
{
    public class RecursionGuardTests
    {
        /// <summary>
        /// Verifies if recursion counting is working as expected.
        /// </summary>
        [Fact]
        public void TestRecursionCount()
        {

            var sanitizer = new HtmlSanitizer();

            using (var guard1 = new RecursionGuard(sanitizer))
            {
                Assert.Equal(1, guard1.Depth);

                using (var guard2 = new RecursionGuard(sanitizer))
                {
                    Assert.Equal(2, guard1.Depth);
                    Assert.Equal(2, guard2.Depth);
                }

                Assert.Equal(1, guard1.Depth);
            }
        }

        [Fact]
        public void TestRecursionMax()
        {

            var sanitizer = new HtmlSanitizer();
            sanitizer.MaxRecursionDepth = 1;

            using (var guard1 = new RecursionGuard(sanitizer))
            {
                Assert.Equal(1, guard1.Depth);
                Assert.Throws<InvalidOperationException>(() =>
              {
                  using (var guard2 = new RecursionGuard(sanitizer))
                  {
                      Assert.Equal(2, guard2.Depth);
                  }
              });

            }
        }

        [Fact]
        public void DivRecursionAttempt()
        {

            string result;
            var sanitizer = new HtmlSanitizer();
            sanitizer.Tag("div");
            sanitizer.MaxRecursionDepth = 10;

            // Test 11 nested div elements
            var input = @"
<div> <div> <div> <div> <div> <div> <div> <div> <div> <div> <div>
</div></div></div></div></div></div></div></div></div></div></div>";
            Assert.Throws<InvalidOperationException>(() => { result = sanitizer.Sanitize(input); });
        }
    }
}

