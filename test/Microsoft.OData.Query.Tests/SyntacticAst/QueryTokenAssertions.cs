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
    public static LiteralToken ShouldBeLiteralToken(this IQueryToken token, object expectedValue)
    {
        Assert.NotNull(token);
        LiteralToken literalToken = Assert.IsType<LiteralToken>(token);
        Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
        Assert.Equal(expectedValue, literalToken.Value);
        return literalToken;
    }

    public static EndPathToken ShouldBeEndPathToken(this IQueryToken token, string expectedName)
    {
        Assert.NotNull(token);
        EndPathToken propertyAccessQueryToken = Assert.IsType<EndPathToken>(token);
        Assert.Equal(QueryTokenKind.EndPath, propertyAccessQueryToken.Kind);
        Assert.Equal(expectedName, propertyAccessQueryToken.Identifier);
        return propertyAccessQueryToken;
    }

    public static BinaryOperatorToken ShouldBeBinaryOperatorQueryToken(this IQueryToken token, BinaryOperatorKind expectedOperatorKind)
    {
        Assert.NotNull(token);
        BinaryOperatorToken propertyAccessQueryToken = Assert.IsType<BinaryOperatorToken>(token);
        Assert.Equal(QueryTokenKind.BinaryOperator, propertyAccessQueryToken.Kind);
        Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
        return propertyAccessQueryToken;
    }

    public static UnaryOperatorToken ShouldBeUnaryOperatorQueryToken(this IQueryToken token, UnaryOperatorKind expectedOperatorKind)
    {
        Assert.NotNull(token);
        UnaryOperatorToken propertyAccessQueryToken = Assert.IsType<UnaryOperatorToken>(token);
        Assert.Equal(QueryTokenKind.UnaryOperator, propertyAccessQueryToken.Kind);
        Assert.Equal(expectedOperatorKind, propertyAccessQueryToken.OperatorKind);
        return propertyAccessQueryToken;
    }

    public static StringLiteralToken ShouldBeStringLiteralToken(this IQueryToken token, string text)
    {
        Assert.NotNull(token);
        StringLiteralToken stringLiteralToken = Assert.IsType<StringLiteralToken>(token);
        Assert.Equal(text, stringLiteralToken.Text);
        return stringLiteralToken;
    }
}
