//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class ODataQueryOptionParserTests
{
    [Fact]
    public async Task QueryParser_Parse_QueryWithUniqueKeysWorks()
    {
        // Arrange
        IODataQueryOptionParser parser = new ODataQueryOptionParser(null);
        string query = "?$orderby=Name";
        QueryOptionParserContext context = new QueryOptionParserContext(typeof(Person));

        // Act
        var result = await parser.ParseQueryAsync(query, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.OrderBy);
    }
}
