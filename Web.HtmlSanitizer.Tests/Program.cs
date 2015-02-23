using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vereyon.Web
{
    public class Program
    {

        public static void Main(string[] args)
        {

            AppDomain.CurrentDomain.ExecuteAssembly(@"..\..\..\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe",
                new string[] { Assembly.GetExecutingAssembly().Location });
        }
    }
}
