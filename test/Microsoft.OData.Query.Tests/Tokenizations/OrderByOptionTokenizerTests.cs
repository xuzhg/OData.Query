//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class OrderByOptionTokenizerTests
{
    private readonly IOrderByOptionTokenizer _orderByTokenizer;

    public OrderByOptionTokenizerTests()
    {
        _orderByTokenizer = new OrderByOptionTokenizer(ExpressionLexerFactory.Default);
    }

    [Fact]
    public async Task OrderByTokenizer_CanTokenizeOrderByQueryOption()
    {
        // Arrange
        string compute = "Prop1,Prop2 asc,Prop3 desc";

        // Act
        OrderByToken token = await _orderByTokenizer.TokenizeAsync(compute, QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.OrderBy, token.Kind);
        int index = 0;
        while (token != null)
        {
            EndPathToken path = Assert.IsType<EndPathToken>(token.Expression);
            Assert.Null(path.NextToken);

            if (index == 0)
            {
                Assert.Equal("Prop1", path.Identifier);
                Assert.Equal(OrderByDirection.Ascending, token.Direction);
            }
            else if (index == 1)
            {
                Assert.Equal("Prop2", path.Identifier);
                Assert.Equal(OrderByDirection.Ascending, token.Direction);
            }
            else
            {
                Assert.Equal("Prop3", path.Identifier);
                Assert.Equal(OrderByDirection.Descending, token.Direction);
            }

            index++;
            token = token.ThenBy;
        }

        Assert.Equal(3, index);
    }
}
