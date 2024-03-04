//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Query.Lexers;
using Xunit;

namespace Microsoft.OData.Query.Tests.Lexers;

public static class IExpressionLexerAssertions
{
    public static (ExpressionKind, string, int)[] GetTokens(this IExpressionLexer lexer)
    {
        Assert.NotNull(lexer);

        IList<(ExpressionKind, string, int)> tokens = new List<(ExpressionKind, string, int)>();
        while (lexer.NextToken())
        {
            ExpressionToken token = lexer.CurrentToken;

            tokens.Add((token.Kind, token.Text.ToString(), token.Position));
        }

        Assert.Equal(ExpressionKind.EndOfInput, lexer.CurrentToken.Kind);
        return tokens.ToArray();
    }
}
