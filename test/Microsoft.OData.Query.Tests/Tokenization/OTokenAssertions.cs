//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public static class OTokenAssertions
{
    public static void ShouldBeToken(this OToken token, OTokenKind kind, int startingPosition)
    {
        Assert.Equal(kind, token.Kind);
        Assert.Null(token.Text);
        Assert.Equal(startingPosition, token.Position);
    }

    public static void ShouldBeToken(this OToken token, OTokenKind kind, string text, int startingPosition)
    {
        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
        Assert.Equal(startingPosition, token.Position);
    }
}
