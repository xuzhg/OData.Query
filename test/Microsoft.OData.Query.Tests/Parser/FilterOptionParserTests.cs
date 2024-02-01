//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class FilterOptionParserTests
{
    [Fact]
    public void ParseFilter_WorksForBasicFilterExpression()
    {
        FilterOptionParser parser = new FilterOptionParser(new OTokenizerFactory());

        QueryToken token = parser.ParseFilter("2 eq Id", new OrderByOptionParserContext());

        Assert.NotNull(token);
        BinaryOperatorToken binaryOperatorToken = Assert.IsType<BinaryOperatorToken>(token);
        Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorToken.OperatorKind);
    }
}
