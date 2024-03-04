//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Xunit;

namespace Microsoft.OData.Query.Tests.Lexers;

public class ExpressionLexerTests
{
    private readonly LexerOptions _options;

    public ExpressionLexerTests()
    {
        _options = new LexerOptions();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExpressionLexer_ShouldTokenizeWhiteSpaceToken(bool ignoreWhitespace)
    {
        // Arrange
        _options.IgnoreWhitespace = ignoreWhitespace;
        IExpressionLexer lexer = new ExpressionLexer(" \u0085 , \u00a0 \r\v\f\n\t", _options);

        // Act
        (ExpressionKind, string, int)[] tokens = lexer.GetTokens();

        // Assert
        if (ignoreWhitespace)
        {
            (ExpressionKind, string, int) token = Assert.Single(tokens);
            token.ShouldBeToken(ExpressionKind.Comma, ",", 3);
        }
        else
        {
            Assert.Equal(3, tokens.Length);
            tokens[0].ShouldBeToken(ExpressionKind.Whitespace, " \u0085 ", 0);
            tokens[1].ShouldBeToken(ExpressionKind.Comma, ",", 3);
            tokens[2].ShouldBeToken(ExpressionKind.Whitespace, " \u00a0 \r\v\f\n\t", 4);
        }
    }

    [Theory]
    [InlineData("(", ExpressionKind.OpenParen)]
    [InlineData(")", ExpressionKind.CloseParen)]
    [InlineData("[", ExpressionKind.OpenBracket)]
    [InlineData("]", ExpressionKind.CloseBracket)] 
    [InlineData("{", ExpressionKind.OpenCurly)]
    [InlineData("}", ExpressionKind.CloseCurly)]
    [InlineData(",", ExpressionKind.Comma)]
    [InlineData("=", ExpressionKind.Equal)]
    [InlineData("/", ExpressionKind.Slash)]
    [InlineData("?", ExpressionKind.Question)]
    [InlineData(".", ExpressionKind.Dot)]
    [InlineData("*", ExpressionKind.Star)]
    [InlineData(":", ExpressionKind.Colon)]
    [InlineData("&", ExpressionKind.Ampersand)]
    [InlineData(";", ExpressionKind.SemiColon)]
    [InlineData("$", ExpressionKind.Dollar)]
    [InlineData("@", ExpressionKind.At)]
    public void ExpressionLexer_ShouldReadSpecialCharacter_ReturnCorrectToken(string text, ExpressionKind kind)
    {
        // Arrange
        IExpressionLexer tokenizer = new ExpressionLexer(text, _options);

        // Act
        (ExpressionKind, string, int)[] tokens = tokenizer.GetTokens();

        // Assert
        (ExpressionKind, string, int) token = Assert.Single(tokens);
        token.ShouldBeToken(kind, text, 0);
    }

    [Theory]
    [InlineData("2014-09-19T12:13:14+00:00", ExpressionKind.DateTimeOffsetLiteral)]
    [InlineData("2014-09-19", ExpressionKind.DateOnlyLiteral)]
    [InlineData("12:30:03.900", ExpressionKind.TimeOnlyLiteral)]
    [InlineData("42", ExpressionKind.IntegerLiteral)]
    [InlineData("2147483647" + "111", ExpressionKind.Int64Literal)] // int.MaxValue + "111"
    [InlineData("1234567.001", ExpressionKind.DoubleLiteral)]
    [InlineData("123.001", ExpressionKind.SingleLiteral)]
    [InlineData("3258.678765765489753678965390", ExpressionKind.DecimalLiteral)]
    [InlineData("'a\\'a'", ExpressionKind.StringLiteral)]
    [InlineData("\"a\\\"'a\"", ExpressionKind.StringLiteral)]
    [InlineData("0C6C8FD1-1231-4D24-9023-7687B39892B0", ExpressionKind.GuidLiteral)]
    [InlineData("name", ExpressionKind.Identifier)]
    [InlineData("true", ExpressionKind.BooleanLiteral)]
    [InlineData("null", ExpressionKind.NullLiteral)]
    [InlineData("@name.t", ExpressionKind.AnnotationIdentifier)]
    public void ExpressionLexer_ShouldReadExpression_ReturnCorrectToken(string text, ExpressionKind kind)
    {
        // Arrange
        IExpressionLexer tokenizer = new ExpressionLexer(text, _options);

        // Act
        (ExpressionKind, string, int)[] tokens = tokenizer.GetTokens();

        // Assert
        (ExpressionKind, string, int) token = Assert.Single(tokens);
        token.ShouldBeToken(kind, text, 0);
    }
}
