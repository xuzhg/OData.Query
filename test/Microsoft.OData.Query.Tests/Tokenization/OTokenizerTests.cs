//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public class OTokenizerTests
{
    private readonly OTokenizerContext _context;

    public OTokenizerTests()
    {
        _context = new OTokenizerContext();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OTokenizer_ShouldTokenizeWhiteSpaceToken(bool ignoreWhitespace)
    {
        // Arrange
        _context.IgnoreWhitespace = ignoreWhitespace;
        IOTokenizer tokenizer = new OTokenizer(" \u0085 , \u00a0 \r\v\f\n\t", _context);

        // Act
        (OTokenKind, string, int)[] tokens = tokenizer.GetTokens();

        // Assert
        if (ignoreWhitespace)
        {
            (OTokenKind, string, int) token = Assert.Single(tokens);
            token.ShouldBeToken(OTokenKind.Comma, ",", 3);
        }
        else
        {
            Assert.Equal(3, tokens.Length);
            tokens[0].ShouldBeToken(OTokenKind.Whitespace, " \u0085 ", 0);
            tokens[1].ShouldBeToken(OTokenKind.Comma, ",", 3);
            tokens[2].ShouldBeToken(OTokenKind.Whitespace, " \u00a0 \r\v\f\n\t", 4);
        }
    }

    [Theory]
    [InlineData("(", OTokenKind.OpenParen)]
    [InlineData(")", OTokenKind.CloseParen)]
    [InlineData("[", OTokenKind.OpenBracket)]
    [InlineData("]", OTokenKind.CloseBracket)] 
    [InlineData("{", OTokenKind.OpenCurly)]
    [InlineData("}", OTokenKind.CloseCurly)]
    [InlineData(",", OTokenKind.Comma)]
    [InlineData("=", OTokenKind.Equal)]
    [InlineData("/", OTokenKind.Slash)]
    [InlineData("?", OTokenKind.Question)]
    [InlineData(".", OTokenKind.Dot)]
    [InlineData("*", OTokenKind.Star)]
    [InlineData(":", OTokenKind.Colon)]
    [InlineData("&", OTokenKind.Ampersand)]
    [InlineData(";", OTokenKind.SemiColon)]
    [InlineData("$", OTokenKind.Dollar)]
    [InlineData("@", OTokenKind.At)]
    public void OTokenizer_ShouldReadSpecialCharacter_ReturnCorrectToken(string text, OTokenKind kind)
    {
        // Arrange
        IOTokenizer tokenizer = new OTokenizer(text, _context);

        // Act
        (OTokenKind, string, int)[] tokens = tokenizer.GetTokens();

        // Assert
        (OTokenKind, string, int) token = Assert.Single(tokens);
        token.ShouldBeToken(kind, text, 0);
    }

    [Theory]
    [InlineData("2014-09-19T12:13:14+00:00", OTokenKind.DateTimeOffsetLiteral)]
    [InlineData("2014-09-19", OTokenKind.DateOnlyLiteral)]
    [InlineData("12:30:03.900", OTokenKind.TimeOnlyLiteral)]
    [InlineData("42", OTokenKind.IntegerLiteral)]
    [InlineData("2147483647" + "111", OTokenKind.Int64Literal)] // int.MaxValue + "111"
    [InlineData("1234567.001", OTokenKind.DoubleLiteral)]
    [InlineData("123.001", OTokenKind.SingleLiteral)]
    [InlineData("3258.678765765489753678965390", OTokenKind.DecimalLiteral)]
    [InlineData("'a\\'a'", OTokenKind.StringLiteral)]
    [InlineData("\"a\\\"'a\"", OTokenKind.StringLiteral)]
    [InlineData("0C6C8FD1-1231-4D24-9023-7687B39892B0", OTokenKind.GuidLiteral)]
    [InlineData("name", OTokenKind.Identifier)]
    [InlineData("true", OTokenKind.BooleanLiteral)]
    [InlineData("null", OTokenKind.NullLiteral)]
    [InlineData("@name.t", OTokenKind.AnnotationIdentifier)]
    public void OTokenizer_ShouldReadExpression_ReturnCorrectToken(string text, OTokenKind kind)
    {
        // Arrange
        IOTokenizer tokenizer = new OTokenizer(text, _context);

        // Act
        (OTokenKind, string, int)[] tokens = tokenizer.GetTokens();

        // Assert
        (OTokenKind, string, int) token = Assert.Single(tokens);
        token.ShouldBeToken(kind, text, 0);
    }
}
