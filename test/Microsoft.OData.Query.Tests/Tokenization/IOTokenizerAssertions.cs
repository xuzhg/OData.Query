//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public static class IOTokenizerAssertions
{
    public static (OTokenKind, string, int)[] GetTokens(this IOTokenizer tokenizer)
    {
        Assert.NotNull(tokenizer);

        IList<(OTokenKind, string, int)> tokens = new List<(OTokenKind, string, int)>();
        while (tokenizer.NextToken())
        {
            OToken token = tokenizer.CurrentToken;

            tokens.Add((token.Kind, token.Text.ToString(), token.Position));
        }

        Assert.Equal(OTokenKind.EndOfInput, tokenizer.CurrentToken.Kind);
        return tokens.ToArray();
    }
}
