//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokenizors;
using Microsoft.OData.Query.Tokens;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizors
{
    public class SearchTokenizorTests
    {
        [Fact]
        public void SearchParenthesesTest()
        {
            ISearchTokenizor searchTokenizor = new SearchTokenizor(50);
            QueryToken token = searchTokenizor.TokenizeSearch("(A  OR BC) AND DEF");

            //var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            //var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
            //binaryToken11.Left.ShouldBeStringLiteralToken("A");
            //binaryToken11.Right.ShouldBeStringLiteralToken("BC");
            //binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
        }

        [Fact]
        public void SearchCombinedTest()
        {
            ISearchTokenizor searchTokenizor = new SearchTokenizor(50);

            QueryToken token = searchTokenizor.TokenizeSearch("a AND bc OR def AND NOT (ghij AND klmno AND pqrstu)");
           
            //var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
            //var binaryToken21 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            //var binaryToken22 = binaryToken1.Right.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            //binaryToken21.Left.ShouldBeStringLiteralToken("a");
            //binaryToken21.Right.ShouldBeStringLiteralToken("bc");
            //binaryToken22.Left.ShouldBeStringLiteralToken("def");
            //var unaryToken222 = binaryToken22.Right.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not);
            //var binaryToken222 = unaryToken222.Operand.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            //var binaryToken2221 = binaryToken222.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            //binaryToken2221.Left.ShouldBeStringLiteralToken("ghij");
            //binaryToken2221.Right.ShouldBeStringLiteralToken("klmno");
            //binaryToken222.Right.ShouldBeStringLiteralToken("pqrstu");
        }
    }
}
