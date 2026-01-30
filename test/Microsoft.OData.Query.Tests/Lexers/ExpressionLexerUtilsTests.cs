//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using System;
using Xunit;

namespace Microsoft.OData.Query.Tests.Lexers;

public class ExpressionLexerUtilsTests
{
    [Theory]
    [InlineData(ExpressionKind.DecimalLiteral)]
    [InlineData(ExpressionKind.IntegerLiteral)]
    [InlineData(ExpressionKind.DoubleLiteral)]
    [InlineData(ExpressionKind.Int64Literal)]
    [InlineData(ExpressionKind.SingleLiteral)]
    public void IsNumericTokenKind_ReturnsTrueIfNumericToken(ExpressionKind kind)
    {
        Assert.True(ExpressionLexerUtils.IsNumericTokenKind(kind));
    }

    [Theory]
    [InlineData(ExpressionKind.Colon)]
    [InlineData(ExpressionKind.DateOnlyLiteral)]
    [InlineData(ExpressionKind.GeographyLiteral)]
    [InlineData(ExpressionKind.Identifier)]
    [InlineData(ExpressionKind.None)]
    public void IsNumericTokenKind_ReturnsFalseIfNotNumericToken(ExpressionKind kind)
    {
        Assert.False(ExpressionLexerUtils.IsNumericTokenKind(kind));
    }

    [Theory]
    [InlineData("INF", true)]
    [InlineData("inf", false)]
    [InlineData("INd", false)]
    [InlineData("INz", false)]
    public void IsInfinityLiteral_ReturnsCorrectly(string text, bool expected)
    {
        bool result = ExpressionLexerUtils.IsInfinity(text);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("INFf", true)]
    [InlineData("inff", false)]
    [InlineData("INFd", false)]
    [InlineData("INzf", false)]
    public void IsSingleInfinityLiteral_ReturnsCorrectly(string text, bool expected)
    {
        bool result = ExpressionLexerUtils.IsSingleInfinity(text.AsSpan());
        Assert.Equal(expected, result);
    }
}
