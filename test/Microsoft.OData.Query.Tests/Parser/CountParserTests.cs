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

public class CountParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private ICountParser _parser = new CountParser();

    [Fact]
    public async Task ParseCountOption_ThrowsArgumentNull_Count()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("count", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("count", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseCountOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any", null));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData(" true ", true)]
    [InlineData("false", false)]
    [InlineData("false  ", false)]
    [InlineData("  false", false)]
    public async Task ParseCountOption_Works_ForValidCount(string count, bool expected)
    {
        // Arrange & Act
        bool actual = await _parser.ParseAsync(count, _context);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Any")]
    [InlineData("1")]
    public async Task ParseCountOption_Throws_ForInvalidCount_DefaultConfig(string count)
    {
        // Arrange & Act
        Func<Task> test = async () => await _parser.ParseAsync(count, _context);

        // Assert
        await test.ThrowsAsync<QueryParserException>($"Invalid value '{count}' for $count query option found. The $count query option requires 'true'/'false'.");
    }

    [Theory]
    [InlineData("True")]
    [InlineData("fAlse")]
    public async Task ParseCountOption_Throws_ForInvalidCount_DisableCaseInsensitive(string count)
    {
        // Arrange
        _context.EnableCaseInsensitive = false;

        // Act
        Func<Task> test = async () => await _parser.ParseAsync(count, _context);

        // Assert
        await test.ThrowsAsync<QueryParserException>($"Invalid value '{count}' for $count query option found. The $count query option requires 'true'/'false'.");
    }
}
