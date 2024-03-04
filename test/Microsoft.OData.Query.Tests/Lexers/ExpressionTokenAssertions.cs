//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Xunit;

namespace Microsoft.OData.Query.Tests.Lexers;

public static class ExpressionTokenAssertions
{
    public static void ShouldBeToken(this (ExpressionKind, string, int) token, ExpressionKind kind, int startingPosition)
    {
        Assert.Equal(kind, token.Item1);
        Assert.Equal(startingPosition, token.Item3);
    }

    public static void ShouldBeToken(this (ExpressionKind, string, int) token, ExpressionKind kind, string text, int startingPosition)
    {
        Assert.Equal(kind, token.Item1);
        Assert.Equal(text, token.Item2);
        Assert.Equal(startingPosition, token.Item3);
    }
}
