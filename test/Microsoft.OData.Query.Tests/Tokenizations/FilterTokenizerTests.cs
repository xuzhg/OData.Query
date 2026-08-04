//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class FilterTokenizerTests
{
    private readonly IFilterTokenizer _filterTokenizer;

    public FilterTokenizerTests()
    {
        _filterTokenizer = new FilterTokenizer();
    }

    [Fact]
    public async Task FilterTokenizer_CanTokenizeFilterQueryOption()
    {
        // Arrange
        string filter = "Name eq 'Sam'";

        // Act
        IQueryToken token = await _filterTokenizer.TokenizeAsync(filter.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.BinaryOperator, token.Kind);
        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        binaryToken.Left.ShouldBeEndPathToken("Name");
        binaryToken.Right.ShouldBeLiteralToken("Sam");
    }

    [Fact]
    public async Task FilterTokenizer_RespectsLogicalOperatorPrecedence()
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            "Name eq 'Sam' and Id gt 1".AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken and = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        and.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        and.Right.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.GreaterThan);
    }

    [Fact]
    public async Task FilterTokenizer_UnescapesDoubledQuoteInStringLiteral()
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            "Name eq 'O''Brien'".AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        binaryToken.Right.ShouldBeLiteralToken("O'Brien");
    }

    [Theory]
    [InlineData("Payload eq 0x0102FF")]
    [InlineData("Payload eq binary'AQL/'")]
    public async Task FilterTokenizer_ParsesBinaryLiteral(string filter)
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            filter.AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        LiteralToken literal = Assert.IsType<LiteralToken>(binaryToken.Right);
        Assert.Equal(new byte[] { 1, 2, 255 }, Assert.IsType<byte[]>(literal.Value));
    }

    [Theory]
    [InlineData("Value eq INF", double.PositiveInfinity)]
    [InlineData("Value eq -INF", double.NegativeInfinity)]
    [InlineData("Value eq NaN", double.NaN)]
    public async Task FilterTokenizer_ParsesSpecialDoubleLiteral(string filter, double expected)
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            filter.AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        LiteralToken literal = Assert.IsType<LiteralToken>(binaryToken.Right);
        Assert.Equal(expected, Assert.IsType<double>(literal.Value));
    }

    [Fact]
    public async Task FilterTokenizer_ParsesDurationLiteral()
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            "Value eq duration'P1DT2H'".AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        LiteralToken literal = Assert.IsType<LiteralToken>(binaryToken.Right);
        Assert.Equal(TimeSpan.FromHours(26), Assert.IsType<TimeSpan>(literal.Value));
    }

    [Theory]
    [InlineData("duration'P1Y'")]
    [InlineData("duration'P1M'")]
    public async Task FilterTokenizer_RejectsNonODataDuration(string duration)
    {
        await Assert.ThrowsAsync<FormatException>(async () =>
            await _filterTokenizer.TokenizeAsync(
                $"Value eq {duration}".AsMemory(),
                QueryTokenizerContext.Default));
    }

    [Fact]
    public async Task FilterTokenizer_TruncatesTemporalPrecisionToClrTicks()
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            "Value eq 2014-09-19T12:13:14.123456789012Z".AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        LiteralToken literal = Assert.IsType<LiteralToken>(binaryToken.Right);
        DateTimeOffset value = Assert.IsType<DateTimeOffset>(literal.Value);
        Assert.Equal(1234567, value.Ticks % TimeSpan.TicksPerSecond);
    }

    [Theory]
    [InlineData("Value eq 2147483648L", typeof(long), 2147483648L)]
    [InlineData("Value eq 12.5M", typeof(decimal), 12.5)]
    public async Task FilterTokenizer_ParsesSuffixedNumericLiteral(
        string filter,
        Type expectedType,
        object expected)
    {
        IQueryToken token = await _filterTokenizer.TokenizeAsync(
            filter.AsMemory(),
            QueryTokenizerContext.Default);

        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        LiteralToken literal = Assert.IsType<LiteralToken>(binaryToken.Right);
        Assert.IsType(expectedType, literal.Value);
        Assert.Equal(Convert.ChangeType(expected, expectedType), literal.Value);
    }
}
