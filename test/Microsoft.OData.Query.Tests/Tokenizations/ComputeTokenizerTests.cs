//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class ComputeTokenizerTests
{
    private readonly IComputeTokenizer _computeTokenizer;

    public ComputeTokenizerTests()
    {
        _computeTokenizer = new ComputeTokenizer();
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeComputeWithMathematicalOperations()
    {
        // Arrange
        string compute = "Prop1 mul Prop2 as Product,Prop1 div Prop2 as Ratio,Prop2 mod Prop2 as Remainder";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.Compute, token.Kind);
        List<ComputeItemToken> tokens = token.Items.ToList();
        Assert.Equal(3, tokens.Count);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[0].Kind);
        Assert.Equal("Product", tokens[0].Alias);
        Assert.Equal(BinaryOperatorKind.Multiply, (tokens[0].Expression as BinaryOperatorToken).OperatorKind);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[1].Kind);
        Assert.Equal("Ratio", tokens[1].Alias);
        Assert.Equal(BinaryOperatorKind.Divide, (tokens[1].Expression as BinaryOperatorToken).OperatorKind);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[2].Kind);
        Assert.Equal("Remainder", tokens[2].Alias);
        Assert.Equal(BinaryOperatorKind.Modulo, (tokens[2].Expression as BinaryOperatorToken).OperatorKind);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ComputeTokenizer_ThrowsForNullOrEmpty(string compute)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("compute", async () =>
        {
            await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);
        });
    }

    [Fact]
    public async Task ComputeTokenizer_ThrowsWhenContextIsNull()
    {
        // Arrange
        string compute = "Prop1 add Prop2 as Sum";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () =>
        {
            await _computeTokenizer.TokenizeAsync(compute.AsMemory(), null);
        });
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeSingleComputeItem()
    {
        // Arrange
        string compute = "Price mul Quantity as Total";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.Compute, token.Kind);
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal(QueryTokenKind.ComputeItem, item.Kind);
        Assert.Equal("Total", item.Alias);
        Assert.NotNull(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeAdditionAndSubtraction()
    {
        // Arrange
        string compute = "Price add Tax as TotalPrice,Price sub Discount as DiscountedPrice";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        List<ComputeItemToken> items = token.Items.ToList();
        Assert.Equal(2, items.Count);

        Assert.Equal("TotalPrice", items[0].Alias);
        Assert.Equal(BinaryOperatorKind.Add, (items[0].Expression as BinaryOperatorToken).OperatorKind);

        Assert.Equal("DiscountedPrice", items[1].Alias);
        Assert.Equal(BinaryOperatorKind.Subtract, (items[1].Expression as BinaryOperatorToken).OperatorKind);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeComplexExpression()
    {
        // Arrange
        string compute = "Price mul Quantity add Tax as GrandTotal";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("GrandTotal", item.Alias);
        Assert.NotNull(item.Expression);
        Assert.IsType<BinaryOperatorToken>(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithLiterals()
    {
        // Arrange
        string compute = "Price mul 1.1 as PriceWithTax";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("PriceWithTax", item.Alias);
        BinaryOperatorToken binaryOp = Assert.IsType<BinaryOperatorToken>(item.Expression);
        Assert.Equal(BinaryOperatorKind.Multiply, binaryOp.OperatorKind);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithParentheses()
    {
        // Arrange
        string compute = "(Price add Tax) mul Quantity as Total";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("Total", item.Alias);
        Assert.NotNull(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeMultipleItems()
    {
        // Arrange
        string compute = "A as B,C as D,E as F,G as H,I as J";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        List<ComputeItemToken> items = token.Items.ToList();
        Assert.Equal(5, items.Count);
        Assert.Equal("B", items[0].Alias);
        Assert.Equal("D", items[1].Alias);
        Assert.Equal("F", items[2].Alias);
        Assert.Equal("H", items[3].Alias);
        Assert.Equal("J", items[4].Alias);
    }

    [Fact]
    public async Task ComputeTokenizer_ThrowsWhenMissingAsKeyword()
    {
        // Arrange
        string compute = "Price mul Quantity Total";

        // Act & Assert
        await Assert.ThrowsAsync<QueryTokenizerException>(async () =>
        {
            await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);
        });
    }

    [Fact]
    public async Task ComputeTokenizer_ThrowsWhenMissingAlias()
    {
        // Arrange
        string compute = "Price mul Quantity as";

        // Act & Assert
        await Assert.ThrowsAsync<QueryTokenizerException>(async () =>
        {
            await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);
        });
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithNestedProperties()
    {
        // Arrange
        string compute = "Address/City as City";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("City", item.Alias);
        Assert.NotNull(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithLogicalOperators()
    {
        // Arrange
        string compute = "Price gt 100 and Quantity lt 10 as IsSpecialCase";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("IsSpecialCase", item.Alias);
        BinaryOperatorToken binaryOp = Assert.IsType<BinaryOperatorToken>(item.Expression);
        Assert.Equal(BinaryOperatorKind.And, binaryOp.OperatorKind);
    }

    [Fact]
    public async Task ComputeTokenizer_AllItemsHaveCorrectKind()
    {
        // Arrange
        string compute = "A as B,C as D,E as F";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.All(token.Items, item =>
        {
            Assert.Equal(QueryTokenKind.ComputeItem, item.Kind);
            Assert.NotNull(item.Expression);
            Assert.NotEmpty(item.Alias);
        });
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithWhitespace()
    {
        // Arrange
        string compute = "  Price   mul   Quantity   as   Total  ";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("Total", item.Alias);
        Assert.NotNull(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeComplexMultipleItems()
    {
        // Arrange
        string compute = "Price mul Quantity as Subtotal,Subtotal mul 1.1 as Total,Total sub Discount as Final";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        List<ComputeItemToken> items = token.Items.ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal("Subtotal", items[0].Alias);
        Assert.Equal("Total", items[1].Alias);
        Assert.Equal("Final", items[2].Alias);
    }

    [Theory]
    [InlineData("Value as Result")]
    [InlineData("value as result")]
    [InlineData("VALUE as RESULT")]
    public async Task ComputeTokenizer_CanTokenizeWithDifferentCasing(string compute)
    {
        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.NotNull(item.Alias);
        Assert.NotNull(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeWithUnaryOperator()
    {
        // Arrange
        string compute = "not IsActive as IsInactive";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        ComputeItemToken item = Assert.Single(token.Items);
        Assert.Equal("IsInactive", item.Alias);
        Assert.IsType<UnaryOperatorToken>(item.Expression);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeManyItems()
    {
        // Arrange
        var items = Enumerable.Range(1, 20).Select(i => $"Prop{i} as Alias{i}");
        string compute = string.Join(",", items);

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(20, token.Items.Count());
    }

    [Fact]
    public async Task ComputeTokenizer_ThrowsOnInvalidSyntax()
    {
        // Arrange
        string compute = "Price mul Quantity as Total Extra";

        // Act & Assert
        await Assert.ThrowsAsync<QueryTokenizerException>(async () =>
        {
            await _computeTokenizer.TokenizeAsync(compute.AsMemory(), QueryTokenizerContext.Default);
        });
    }
}
