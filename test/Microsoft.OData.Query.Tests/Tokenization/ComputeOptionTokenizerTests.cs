//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenization;

public class ComputeOptionTokenizerTests
{
    private readonly IComputeOptionTokenizer _computeTokenizer;

    public ComputeOptionTokenizerTests()
    {
        _computeTokenizer = new ComputeOptionTokenizer(ExpressionLexerFactory.Default);
    }

    [Fact]
    public async Task ComputeTokenizer_CanTokenizeComputeWithMathematicalOperations()
    {
        // Arrange
        string compute = "Prop1 mul Prop2 as Product,Prop1 div Prop2 as Ratio,Prop2 mod Prop2 as Remainder";

        // Act
        ComputeToken token = await _computeTokenizer.TokenizeAsync(compute, QueryTokenizerContext.Default);

        // Assert
        Assert.Equal(QueryTokenKind.Compute, token.Kind);
        List<ComputeItemToken> tokens = token.Items.ToList();
        Assert.Equal(3, tokens.Count);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[0].Kind);
        Assert.Equal("Product", tokens[0].Alias);
        Assert.Equal(BinaryOperatorKind.Multiply, (tokens[0].Expression as BinaryOperatorToken).OperatorKind);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[1].Kind);
        Assert.Equal("Ratio", tokens[1].Alias);
        Assert.Equal(BinaryOperatorKind.Divide, (tokens[1].Expression as BinaryOperatorToken).OperatorKind);

        Assert.Equal(QueryTokenKind.ComputeItem, tokens[2].Kind);
        Assert.Equal("Remainder", tokens[2].Alias);
        Assert.Equal(BinaryOperatorKind.Modulo, (tokens[2].Expression as BinaryOperatorToken).OperatorKind);
    }
}
