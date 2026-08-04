//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Lexers;

public class ExpressionKindTests
{
    [Fact]
    public void QueryTokenizerContext_AlwaysIgnoresWhitespace()
    {
        var context = new QueryTokenizerContext
        {
            LexerOptions = new LexerOptions { IgnoreWhitespace = false }
        };

        IExpressionLexer lexer = context.CreateLexer(" name");

        Assert.True(lexer.NextToken());
        Assert.Equal(ExpressionKind.Identifier, lexer.CurrentToken.Kind);
    }
}
