//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class SearchTokenizerTests
{
    private readonly ISearchTokenizer _searchTokenizer;

    public SearchTokenizerTests()
    {
        _searchTokenizer = new SearchTokenizer();
    }

    [Theory]
    [InlineData("Any")]
    [InlineData("\"A AND BC AND DEF\"")] // one phrase
    public async ValueTask SearchTokenizer_CanTokenizeSearchWordAndPhrase(string search)
    {
        // Arrange & Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        token.ShouldBeStringLiteralToken(search);
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_Add()
    {
        // Arrange
        string search = "A and BC and DEF";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        binaryToken11.Left.ShouldBeStringLiteralToken("A");
        binaryToken11.Right.ShouldBeStringLiteralToken("BC");
        binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_SpaceImplies()
    {
        // Arrange
        string search = "A BC DEF";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        binaryToken11.Left.ShouldBeStringLiteralToken("A");
        binaryToken11.Right.ShouldBeStringLiteralToken("BC");
        binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_Or()
    {
        // Arrange
        string search = "foo or bar";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
        binaryToken.Left.ShouldBeStringLiteralToken("foo");
        binaryToken.Right.ShouldBeStringLiteralToken("bar");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_Combined()
    {
        // Arrange
        string search = "(A  OR BC) AND DEF";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
        binaryToken11.Left.ShouldBeStringLiteralToken("A");
        binaryToken11.Right.ShouldBeStringLiteralToken("BC");
        binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_SpaceImpliesCombined()
    {
        // Arrange
        string search = "(A BC) DEF";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        binaryToken11.Left.ShouldBeStringLiteralToken("A");
        binaryToken11.Right.ShouldBeStringLiteralToken("BC");
        binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_Not()
    {
        // Arrange
        string search = "not foo";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var unaryToken = token.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not);
        unaryToken.Operand.ShouldBeStringLiteralToken("foo");
    }

    [Fact]
    public async ValueTask SearchTokenizer_CanTokenizeSearchOptions_AdvancedCombined()
    {
        // Arrange
        string search = "a and bc or def and not (efj and keh and xyz)";

        // Act
        IQueryToken token = await _searchTokenizer.TokenizeAsync(search, QueryTokenizerContext.Default);

        // Assert
        var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
        var binaryToken21 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken22 = binaryToken1.Right.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        binaryToken21.Left.ShouldBeStringLiteralToken("a");
        binaryToken21.Right.ShouldBeStringLiteralToken("bc");
        binaryToken22.Left.ShouldBeStringLiteralToken("def");
        var unaryToken222 = binaryToken22.Right.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not);
        var binaryToken222 = unaryToken222.Operand.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        var binaryToken2221 = binaryToken222.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
        binaryToken2221.Left.ShouldBeStringLiteralToken("efj");
        binaryToken2221.Right.ShouldBeStringLiteralToken("keh");
        binaryToken222.Right.ShouldBeStringLiteralToken("xyz");
    }

    //[Fact]
    //public void SearchUnMatchedParenthesisTest()
    //{
    //    Action action = () => searchParser.ParseSearch("(A BC DEF");
    //    action.Throws<ODataException>(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(9, "(A BC DEF"));
    //}

    //[Fact]
    //public void SearchOperandMissingTest()
    //{
    //    Action action = () => searchParser.ParseSearch("A AND");
    //    action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(5, "A AND"));
    //}

    //[Fact]
    //public void SearchOperandMissingInParenthesisTest()
    //{
    //    Action action = () => searchParser.ParseSearch("(A AND)");
    //    action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(6, "(A AND)"));
    //}

    //[Fact]
    //public void SearchEmptyPhrase()
    //{
    //    Action action = () => searchParser.ParseSearch("A \"\"");
    //    action.Throws<ODataException>(Strings.ExpressionToken_IdentifierExpected(2));
    //}
}
