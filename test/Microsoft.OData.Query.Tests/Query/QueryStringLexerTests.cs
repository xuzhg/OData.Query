//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Query.Tests;

public class QueryStringLexerTests
{
    [Theory]
    [InlineData("?$key1=value1&key2=value2")]
    [InlineData("$key1=value1&key2=value2")]
    public void QueryStringLexer_Tokenize_QueryWithUniqueKeysWorks(string queryString)
    {
        // Arrange
        QueryStringLexer lexer = new QueryStringLexer(queryString);

        // Act
        int index = 0;
        while (lexer.MoveNext())
        {
            // Assert
            Assert.False(lexer.CurrentName.IsEmpty);
            Assert.False(lexer.CurrentValue.IsEmpty);

            if (index == 0)
            {
                Assert.True(lexer.CurrentName.Span.Equals("$key1", StringComparison.Ordinal));
                Assert.True(lexer.CurrentValue.Span.Equals("value1", StringComparison.Ordinal));
            }
            else if (index == 1)
            {
                Assert.True(lexer.CurrentName.Span.Equals("key2", StringComparison.Ordinal));
                Assert.True(lexer.CurrentValue.Span.Equals("value2", StringComparison.Ordinal));
            }

            index++;
        }

        Assert.Equal(2, index);
    }

    [Theory]
    [InlineData("?q")]
    [InlineData("?q&")]
    [InlineData("?q=")]
    [InlineData("?q=&")]
    public void QueryStringLexer_Tokenize_KeyWithoutValuesAddedToQueryCollection(string queryString)
    {
        // Arrange
        QueryStringLexer lexer = new QueryStringLexer(queryString);

        // Act
        int index = 0;
        while (lexer.MoveNext())
        {
            // Assert
            Assert.False(lexer.CurrentName.IsEmpty);
            Assert.True(lexer.CurrentName.Span.Equals("q", StringComparison.Ordinal));

            Assert.True(lexer.CurrentValue.IsEmpty);
            index++;
        }

        Assert.Equal(1, index);
    }
}
