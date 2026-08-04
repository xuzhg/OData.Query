//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using System.Globalization;
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
    [InlineData("2014-09-19T12:13Z", ExpressionKind.DateTimeOffsetLiteral)]
    [InlineData("2014-09-19T12:13+00:00", ExpressionKind.DateTimeOffsetLiteral)]
    [InlineData("2014-09-19T12:13:14.123456789012Z", ExpressionKind.DateTimeOffsetLiteral)]
    [InlineData("2014-09-19", ExpressionKind.DateOnlyLiteral)]
    [InlineData("12:30:03.900", ExpressionKind.TimeOnlyLiteral)]
    [InlineData("12:30", ExpressionKind.TimeOnlyLiteral)]
    [InlineData("12:30:03.123456789012", ExpressionKind.TimeOnlyLiteral)]
    [InlineData("42", ExpressionKind.IntegerLiteral)]
    [InlineData("2147483647" + "111", ExpressionKind.Int64Literal)] // int.MaxValue + "111"
    [InlineData("1234567.001", ExpressionKind.DoubleLiteral)]
    [InlineData("123.001", ExpressionKind.SingleLiteral)]
    [InlineData("3258.678765765489753678965390", ExpressionKind.DecimalLiteral)]
    [InlineData("0x0A0b", ExpressionKind.HexBinaryLiteral)]
    [InlineData("binary'AQID'", ExpressionKind.BinaryLiteral)]
    [InlineData("'a\\'a'", ExpressionKind.StringLiteral)]
    [InlineData("'O''Brien'", ExpressionKind.StringLiteral)]
    [InlineData("\"a\\\"'a\"", ExpressionKind.StringLiteral)]
    [InlineData("0C6C8FD1-1231-4D24-9023-7687B39892B0", ExpressionKind.GuidLiteral)]
    [InlineData("name", ExpressionKind.Identifier)]
    [InlineData("true", ExpressionKind.BooleanLiteral)]
    [InlineData("null", ExpressionKind.NullLiteral)]
    [InlineData("@name.t", ExpressionKind.AnnotationIdentifier)]
    [InlineData("@name", ExpressionKind.ParameterAlias)]
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

    [Theory]
    [InlineData("'unterminated")]
    [InlineData("duration'unterminated")]
    public void ExpressionLexer_ThrowsForUnterminatedQuotedLiteral(string text)
    {
        IExpressionLexer lexer = new ExpressionLexer(text, _options);

        Assert.Throws<ExpressionLexerException>(() => lexer.NextToken());
    }

    [Theory]
    [InlineData("0x")]
    [InlineData("0x0")]
    [InlineData("0x00GG")]
    [InlineData("0x0011junk")]
    public void ExpressionLexer_ThrowsForInvalidHexBinaryLiteral(string text)
    {
        IExpressionLexer lexer = new ExpressionLexer(text, _options);

        Assert.Throws<ExpressionLexerException>(() => lexer.NextToken());
    }

    [Theory]
    [InlineData("1Lfoo")]
    [InlineData("12.5Mfoo")]
    [InlineData("1Dfoo")]
    [InlineData("1foo")]
    public void ExpressionLexer_ThrowsWhenNumericLiteralHasInvalidTrailingCharacters(string text)
    {
        IExpressionLexer lexer = new ExpressionLexer(text, _options);

        Assert.Throws<ExpressionLexerException>(() => lexer.NextToken());
    }

    [Theory]
    [InlineData("2014-09-19T12:13:14+00:00)", ExpressionKind.DateTimeOffsetLiteral, ")")]
    [InlineData("2014-09-19&", ExpressionKind.DateOnlyLiteral, "&")]
    [InlineData("12:30:03.900;", ExpressionKind.TimeOnlyLiteral, ";")]
    [InlineData("0C6C8FD1-1231-4D24-9023-7687B39892B0/", ExpressionKind.GuidLiteral, "/")]
    public void ExpressionLexer_StopsTemporalAndGuidLiteralsAtDelimiters(
        string text,
        ExpressionKind expectedKind,
        string delimiter)
    {
        IExpressionLexer lexer = new ExpressionLexer(text, _options);

        Assert.True(lexer.NextToken());
        Assert.Equal(expectedKind, lexer.CurrentToken.Kind);
        Assert.True(lexer.NextToken());
        Assert.Equal(delimiter, lexer.CurrentToken.Text.ToString());
    }

    [Fact]
    public void ExpressionLexer_SnapshotsOptions()
    {
        var options = new LexerOptions { IgnoreWhitespace = false };
        IExpressionLexer lexer = new ExpressionLexer(" name", options);
        options.IgnoreWhitespace = true;

        Assert.True(lexer.NextToken());
        Assert.Equal(ExpressionKind.Whitespace, lexer.CurrentToken.Kind);
    }

    [Fact]
    public void ExpressionLexer_PeekDoesNotAdvance()
    {
        IExpressionLexer lexer = new ExpressionLexer("Name eq 'Sam'", _options);
        Assert.True(lexer.NextToken());

        Assert.True(lexer.PeekNextToken(out ExpressionToken peeked));
        Assert.Equal("eq", peeked.Text.ToString());
        Assert.Equal("Name", lexer.CurrentToken.Text.ToString());

        Assert.True(lexer.NextToken());
        Assert.Equal(peeked.Kind, lexer.CurrentToken.Kind);
        Assert.Equal(peeked.Position, lexer.CurrentToken.Position);
        Assert.Equal(peeked.Text, lexer.CurrentToken.Text);
    }

    [Fact]
    public void ExpressionLexer_RecognizesLiteralsIndependentlyOfCurrentCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            IExpressionLexer lexer = new ExpressionLexer(
                "2014-09-19 12:30:03.900 1.5",
                _options);

            (ExpressionKind, string, int)[] tokens = lexer.GetTokens();

            Assert.Equal(ExpressionKind.DateOnlyLiteral, tokens[0].Item1);
            Assert.Equal(ExpressionKind.TimeOnlyLiteral, tokens[1].Item1);
            Assert.Equal(ExpressionKind.SingleLiteral, tokens[2].Item1);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
