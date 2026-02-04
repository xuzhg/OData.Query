//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tests.Commons;
using Moq;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class SkipParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private ISkipParser _parser = new SkipParser();

    [Fact]
    public async Task ParseSkipOption_ThrowsArgumentNull_Skip()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("skip", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("skip", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseSkipOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any", null));
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1 ", 1)]
    [InlineData("888 ", 888)]
    [InlineData("9223372036854775807", long.MaxValue)] // long.MaxValue
    public async Task ParseSkipOption_Works_ForValidSkip(string skip, long expected)
    {
        // Arrange & Act
        long actual = await _parser.ParseAsync(skip, _context);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("      ")]
    [InlineData("Any")]
    [InlineData("9223372036854775808")] // long.MaxValue + 1
    [InlineData("-1")]
    [InlineData("-9223372036854775808")] // long.MinValue
    public async Task ParseSkipOption_Throws_ForInvalidSkip(string skip)
    {
        // Arrange & Act
        Func<Task> test = async () => await _parser.ParseAsync(skip, _context);

        // Assert
        await test.ThrowsAsync<QueryParserException>($"Invalid value '{skip}' for '$skip' query option found. The '$skip' query option requires a non-negative integer value.");
    }
}
