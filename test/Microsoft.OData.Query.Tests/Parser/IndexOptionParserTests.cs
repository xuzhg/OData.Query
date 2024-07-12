//-----------------------------------------------------------------------
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

public class IndexOptionParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private IIndexOptionParser _parser = new IndexOptionParser();

    [Fact]
    public async Task ParseIndexOption_ThrowsArgumentNull_Index()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("index", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("index", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseIndexOption_ThrowsArgumentNull_Context()
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
    public async Task ParseIndexOption_Works_ForValidIndex(string index, long expected)
    {
        // Arrange & Act
        long actual = await _parser.ParseAsync(index, _context);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("      ")]
    [InlineData("Any")]
    [InlineData("9223372036854775808")] // long.MaxValue + 1
    public async Task ParseIndexOption_Throws_ForInvalidIndex(string index)
    {
        // Arrange & Act
        Func<Task> test = async () => await _parser.ParseAsync(index, _context);

        // Assert
        await test.ThrowsAsync<QueryParserException>($"Invalid value '{index}' for '$index' query option found. The '$index' query option requires an integer value.");
    }
}
