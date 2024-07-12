﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tests.Commons;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class TopOptionParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private ITopOptionParser _parser = new TopOptionParser();

    [Fact]
    public async Task ParseTopOption_ThrowsArgumentNull_Top()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("top", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("top", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseTopOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any", null));
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1 ", 1)]
    [InlineData("-1", -1)]
    [InlineData("9223372036854775807", long.MaxValue)] // long.MaxValue
    [InlineData("-9223372036854775808", long.MinValue)] // long.MinValue
    public async Task ParseTopOption_Works_ForValidTop(string top, long expected)
    {
        // Arrange & Act
        long actual = await _parser.ParseAsync(top, _context);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("      ")]
    [InlineData("Any")]
    [InlineData("9223372036854775808")] // long.MaxValue + 1
    public async Task ParseTopOption_Throws_ForInvalidTop(string top)
    {
        // Arrange & Act
        Func<Task> test = async () => await _parser.ParseAsync(top, _context);

        // Assert
        await test.ThrowsAsync<QueryParserException>($"Invalid value '{top}' for '$top' query option found. The '$top' query option requires an integer value.");
    }
}
