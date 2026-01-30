//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class FilterTokenizerTests
{
    private readonly IFilterTokenizer _filterTokenizer;

    public FilterTokenizerTests()
    {
        _filterTokenizer = new FilterTokenizer();
    }

    [Fact]
    public async Task FilterTokenizer_CanTokenizeFilterQueryOption()
    {
        // Arrange
        string filter = "Name eq 'Sam'";

        // Act
        IQueryToken token = await _filterTokenizer.TokenizeAsync(filter.AsMemory(), QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.BinaryOperator, token.Kind);
        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        binaryToken.Left.ShouldBeEndPathToken("Name");
        binaryToken.Right.ShouldBeLiteralToken("'Sam'");
    }
}
