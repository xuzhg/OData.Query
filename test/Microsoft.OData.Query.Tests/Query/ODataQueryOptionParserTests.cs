//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tests.Models;
using Xunit;

namespace Microsoft.OData.Query.Tests;

public class ODataQueryOptionParserTests
{
    [Fact]
    public async Task QueryParser_Parse_OrderBy()
    {
        // Arrange
        IODataQueryOptionParser parser = new ODataQueryOptionParser(null);
        string query = "?$orderby=Name";
        QueryParserContext context = new QueryParserContext(typeof(Person));

        // Act
        var result = await parser.ParseQueryAsync(query, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OrderBy);
    }

    [Fact]
    public async Task QueryParser_Parse_AdvancedOrderBy()
    {
        // Arrange
        IODataQueryOptionParser parser = new ODataQueryOptionParser(null);
        string query = "?$orderby=location/street";
        QueryParserContext context = new QueryParserContext(typeof(Person));

        // Act
        var result = await parser.ParseQueryAsync(query, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OrderBy);
    }
}
