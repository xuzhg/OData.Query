//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tests.Nodes;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class FilterParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private IFilterParser _parser = new FilterParser();

    [Fact]
    public async Task ParseFilter_ThrowsArgumentNull_Filter()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("filter", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("filter", async () => await _parser.ParseAsync("", null));
    }

    [Fact]
    public async Task ParseFilter_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any", null));
    }

    [Fact]
    public async Task ParseFilter_WorksForBasicFilterExpression()
    {
        FilterClause clause = await _parser.ParseAsync("Name eq 'Sam'", new QueryParserContext(typeof(Customer)));

        BinaryOperatorNode binaryOperatorNode = clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(typeof(Customer).GetProperty("Name"));
        binaryOperatorNode.Right.ShouldBeConstantQueryNode("'Sam'");
    }
}
