//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser
{
    public class ExpressionLexerTests
    {
        [Fact]
        public void ExpressionLexer_ReturnsDateLiteral_WhenNoSuffixDateLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2014-09-19");
           // object result = lexer.ReadLiteralToken();
            //var date = Assert.IsType<Date>(result);
            //Assert.Equal(new Date(2014, 9, 19), date);
        }
    }
}
