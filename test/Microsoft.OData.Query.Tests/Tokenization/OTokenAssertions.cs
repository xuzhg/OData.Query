//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public static class OTokenAssertions
{
    public static void ShouldBeToken(this (OTokenKind, string, int) token, OTokenKind kind, int startingPosition)
    {
        Assert.Equal(kind, token.Item1);
        Assert.Equal(startingPosition, token.Item3);
    }

    public static void ShouldBeToken(this (OTokenKind, string, int) token, OTokenKind kind, string text, int startingPosition)
    {
        Assert.Equal(kind, token.Item1);
        Assert.Equal(text, token.Item2);
        Assert.Equal(startingPosition, token.Item3);
    }
}
