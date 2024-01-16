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
        OToken[] tokens = tokenizer.GetTokens();

        // Assert
        if (ignoreWhitespace)
        {
            OToken token = Assert.Single(tokens);
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
}
