//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public class OTokenizerTests
{
    private OTokenizerContext _context;

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
        OToken[] tokens = GetTokens(tokenizer);

        // Assert
        if (ignoreWhitespace)
        {
            OToken token = Assert.Single(tokens);
            token.ShouldBeToken(OTokenKind.Comma, 3);
        }
        else
        {
            Assert.Equal(3, tokens.Length);
            tokens[0].ShouldBeToken(OTokenKind.Whitespace, " \u0085 ", 0);
            tokens[1].ShouldBeToken(OTokenKind.Comma, 3);
            tokens[2].ShouldBeToken(OTokenKind.Whitespace, " \u00a0 \r\v\f\n\t", 4);
        }
    }

    private static void RunAndVerify(IOTokenizer tokenizer, params OToken[] tokens)
    {
        int i = 0;
        while (tokenizer.NextToken())
        {
            OToken token = tokenizer.CurrentToken;

        }
    }

    private static OToken[] GetTokens(IOTokenizer tokenizer)
    {
        IList<OToken> tokens = new List<OToken>();
        while (tokenizer.NextToken())
        {
            tokens.Add(tokenizer.CurrentToken);
        }

        Assert.Equal(OTokenKind.EndOfInput, tokenizer.CurrentToken.Kind);
        return tokens.ToArray();
    }
}
