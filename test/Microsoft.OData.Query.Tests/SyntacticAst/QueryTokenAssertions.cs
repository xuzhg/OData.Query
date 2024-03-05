//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Xunit;

namespace Microsoft.OData.Query.Tests.SyntacticAst;

public static class QueryTokenAssertions
{
    public static BinaryOperatorToken ShouldBeBinaryOperatorQueryToken(this QueryToken token, BinaryOperatorKind expectedOperatorKind)
    {
        Assert.NotNull(token);
        BinaryOperatorToken propertyAccessQueryToken = Assert.IsType<BinaryOperatorToken>(token);
        Assert.Equal(QueryTokenKind.BinaryOperator, propertyAccessQueryToken.Kind);
        Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
        return propertyAccessQueryToken;
    }

    public static UnaryOperatorToken ShouldBeUnaryOperatorQueryToken(this QueryToken token, UnaryOperatorKind expectedOperatorKind)
    {
        Assert.NotNull(token);
        UnaryOperatorToken propertyAccessQueryToken = Assert.IsType<UnaryOperatorToken>(token);
        Assert.Equal(QueryTokenKind.UnaryOperator, propertyAccessQueryToken.Kind);
        Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
        return propertyAccessQueryToken;
    }

    public static StringLiteralToken ShouldBeStringLiteralToken(this QueryToken token, string text)
    {
        Assert.NotNull(token);
        StringLiteralToken stringLiteralToken = Assert.IsType<StringLiteralToken>(token);
        Assert.Equal(text, stringLiteralToken.Text);
        return stringLiteralToken;
    }
}
