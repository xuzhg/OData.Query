//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser
{
    public class ExpressionLexerUtilsTests
    {
        [Fact]
        public void IsNumeric_ReturnsCorrectBoolean_IfNumericTokenOrNot()
        {
            // Positive
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DecimalLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.IntegerLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DoubleLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Int64Literal));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.SingleLiteral));

            // Negative
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Colon));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DateTimeOffsetLiteral));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.StringLiteral));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.None));
        }

        [Fact]
        public void IsInfinityLiteral_ReturnsCorrectBoolean_ForINFOrNot()
        {
            // Positive
            Assert.True(ExpressionLexerUtils.IsInfinityLiteral("INF"));

            // Negative
            Assert.False(ExpressionLexerUtils.IsInfinityLiteral(null));
            Assert.False(ExpressionLexerUtils.IsInfinityLiteral(""));
            Assert.False(ExpressionLexerUtils.IsInfinityLiteral("      "));
            Assert.False(ExpressionLexerUtils.IsInfinityLiteral("abc"));
            Assert.False(ExpressionLexerUtils.IsInfinityLiteral("inf"));
        }
    }
}
