using Microsoft.OData.Query.Lexers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TokenizerSample
{
    internal class OtherTests
    {
        public static void Test() 
        {
            ExpressionToken token1 = new ExpressionToken();
            //token1.Kind = ExpressionKind.StringLiteral;
            token1.Position = 0;
            //token1.Text = "Hello World".AsMemory();
            Console.WriteLine(token1);


            char? testCh = null;

            if (testCh == 'x')
            {
                Console.WriteLine("testCh is x");

            }
            else
            {
                Console.WriteLine("testCh is not x");
            }
        }
    }
}
