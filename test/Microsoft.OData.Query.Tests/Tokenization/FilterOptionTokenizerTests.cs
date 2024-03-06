//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public class FilterOptionTokenizerTests
{
    private readonly IFilterOptionTokenizer _filterTokenizer;

    public FilterOptionTokenizerTests()
    {
        _filterTokenizer = new FilterOptionTokenizer(ExpressionLexerFactory.Default);
    }

    [Fact]
    public async Task FilterTokenizer_CanTokenizeFilterQueryOption()
    {
        // Arrange
        string filter = "Name eq 'Sam'";

        // Act
        QueryToken token = await _filterTokenizer.TokenizeAsync(filter, QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.BinaryOperator, token.Kind);
        BinaryOperatorToken binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Equal);
        binaryToken.Left.ShouldBeEndPathToken("Name");
        binaryToken.Right.ShouldBeLiteralToken("'Sam'");
    }
}
