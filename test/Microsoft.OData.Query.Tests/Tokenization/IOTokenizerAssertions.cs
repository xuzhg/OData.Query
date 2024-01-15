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
    public static OToken[] GetTokens(this IOTokenizer tokenizer)
    {
        Assert.NotNull(tokenizer);

        IList<OToken> tokens = new List<OToken>();
        while (tokenizer.NextToken())
        {
            tokens.Add(tokenizer.CurrentToken);
        }

        Assert.Equal(OTokenKind.EndOfInput, tokenizer.CurrentToken.Kind);
        return tokens.ToArray();
    }
}
