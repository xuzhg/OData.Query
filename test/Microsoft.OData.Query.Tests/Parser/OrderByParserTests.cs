//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.Nodes;
using Microsoft.OData.Query.Tokenizations;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class OrderByParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private IOrderByParser _parser = new OrderByParser();

    [Fact]
    public async Task ParseOrderByOption_ThrowsArgumentNull_OrderBy()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("orderBy", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("orderBy", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseOrderByOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any", null));
    }

    [Fact]
    public async Task ParseOrderBy_WorksForBasicOrderByExpression()
    {
        OrderByClause clause = await _parser.ParseAsync("Name,Id", new QueryParserContext(typeof(Customer)));

        clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(typeof(Customer).GetProperty("Name"));
        Assert.Equal(OrderByDirection.Ascending, clause.Direction);

        // ThenBy
        Assert.NotNull(clause.ThenBy);
        OrderByClause thenBy = clause.ThenBy;
        thenBy.Expression.ShouldBeSingleValuePropertyAccessQueryNode(typeof(Customer).GetProperty("Id"));
        Assert.Equal(OrderByDirection.Ascending, thenBy.Direction);

        Assert.Null(thenBy.ThenBy);
    }
}
