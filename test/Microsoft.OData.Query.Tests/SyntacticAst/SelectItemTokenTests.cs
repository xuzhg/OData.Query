//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using Microsoft.OData.Query.SyntacticAst;
using Xunit;

namespace Microsoft.OData.Query.Tests.SyntacticAst;

public class SelectItemTokenTests
{
    [Fact]
    public void PathToPropertyCannotBeNullInCtor()
    {
        Assert.Throws<ArgumentNullException>("pathToProperty", () => new SelectItemToken(null));
        Assert.Throws<ArgumentNullException>("pathToProperty", () => new SelectItemToken(null, null, null, null, null, null, null, null, null));
    }

    [Fact]
    public void InnerQueryOptionsCanBeNull()
    {
        // Arrange & Act
        SelectItemToken selectItemToken = new SelectItemToken(new SegmentToken("stuff"), null, null, null, null, null, null, null, null);

        // Assert
        Assert.Null(selectItemToken.Filter);
        Assert.Null(selectItemToken.OrderBy);
        Assert.Null(selectItemToken.Top);
        Assert.Null(selectItemToken.Skip);
        Assert.Null(selectItemToken.Count);
        Assert.Null(selectItemToken.Search);
        Assert.Null(selectItemToken.Select);
    }
    /*
    [Fact]
    public void InnerQueryOptionsPropertySetCorrectly()
    {
        // Arrange & Act
        SelectItemToken selectTermToken1 = new SelectItemToken(new SegmentToken("stuff"));
        SelectItemToken selectTermToken2 = new SelectItemToken(new SegmentToken("stuff"), null);

        // Assert
        Assert.NotNull(selectTermToken1.PathToProperty);
        NonSystemToken nonSystemToken = Assert.IsType<NonSystemToken>(selectTermToken1.PathToProperty);
        Assert.Equal("stuff", nonSystemToken.Identifier);

        Assert.NotNull(selectTermToken2.PathToProperty);
        nonSystemToken = Assert.IsType<NonSystemToken>(selectTermToken2.PathToProperty);
        Assert.Equal("stuff", nonSystemToken.Identifier);
    }

    [Fact]
    public void InnerFilterSetCorrectly()
    {
        // Arrange & Act
        QueryToken filter = new LiteralToken(21);
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            filter, null, null, null, null, null, null, null);

        // Assert
        Assert.NotNull(selectTerm.FilterOption);
        LiteralToken literalToken = Assert.IsType<LiteralToken>(selectTerm.FilterOption);
        Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
        Assert.Equal(21, literalToken.Value);
    }

    [Fact]
    public void InnerOrderBySetCorrectly()
    {
        // Arrange & Act
        OrderByToken orderBy = new OrderByToken(new LiteralToken(42), OrderByDirection.Descending);
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            null, new OrderByToken[] { orderBy }, null, null, null, null, null, null);

        // Assert
        Assert.NotNull(selectTerm.OrderByOptions);
        OrderByToken expectedOrderBy = Assert.Single(selectTerm.OrderByOptions);
        Assert.Equal(QueryTokenKind.OrderBy, expectedOrderBy.Kind);

        Assert.NotNull(expectedOrderBy.Expression);
        LiteralToken literalToken = Assert.IsType<LiteralToken>(expectedOrderBy.Expression);
        Assert.Equal(QueryTokenKind.Literal, literalToken.Kind);
        Assert.Equal(42, literalToken.Value);
    }

    [Fact]
    public void InnerTopSetCorrectly()
    {
        // Arrange & Act
        long top = 42;
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            null, null, top, null, null, null, null, null);

        // Assert
        Assert.NotNull(selectTerm.TopOption);
        Assert.Equal(42, selectTerm.TopOption);
    }

    [Fact]
    public void InnerSkipSetCorrectly()
    {
        // Arrange & Act
        long skip = 42;
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            null, null, null, skip, null, null, null, null);

        // Assert
        Assert.NotNull(selectTerm.SkipOption);
        Assert.Equal(42, selectTerm.SkipOption);
    }

    [Fact]
    public void InnerCountSetCorrectly()
    {
        // Arrange & Act
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            null, null, null, null, false, null, null, null);

        // Assert
        Assert.NotNull(selectTerm.CountQueryOption);
        Assert.False(selectTerm.CountQueryOption);
    }

    [Fact]
    public void InnerSearchSetCorrectly()
    {
        // Arrange & Act
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
            null, null, null, null, null, new StringLiteralToken("searchMe"), null, null);

        // Assert
        Assert.NotNull(selectTerm.SearchOption);
        StringLiteralToken token = Assert.IsType<StringLiteralToken>(selectTerm.SearchOption);
        Assert.Equal("searchMe", token.Text);
    }

    [Fact]
    public void InnerSelectSetCorrectly()
    {
        // Arrange & Act
        SelectToken select = new SelectToken(new PathSegmentToken[] { new NonSystemToken("abc", null, null) });
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null), select);

        // Assert
        Assert.NotNull(selectTerm.SelectOption);
        PathSegmentToken token = Assert.Single(selectTerm.SelectOption.Properties);
        NonSystemToken nonSystemToken = Assert.IsType<NonSystemToken>(token);
        Assert.Equal("abc", nonSystemToken.Identifier);
    }

    [Fact]
    public void InnerComputeSetCorrectly()
    {
        // Arrange & Act
        ComputeToken compute = new ComputeToken(new ComputeExpressionToken[] { });
        SelectTermToken selectTerm = new SelectTermToken(new NonSystemToken("stuff", null, null),
                                                         null,
                                                         null,
                                                         null,
                                                         null,
                                                         null,
                                                         null,
                                                         null,
                                                         compute);

        // Assert
        Assert.NotNull(selectTerm.ComputeOption);
        Assert.Same(compute, selectTerm.ComputeOption);
    }
*/
}
