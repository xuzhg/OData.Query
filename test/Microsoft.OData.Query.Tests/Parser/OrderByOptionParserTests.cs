//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class OrderByOptionParserTests
{
    [Fact]
    public void ParseOrderBy_WorksForBasicOrderByExpression()
    {
        OrderByOptionParser parser = new OrderByOptionParser(new OTokenizerFactory());

        OrderByToken token = parser.ParseOrderBy("Name,Id", new OrderByOptionParserContext());

        Assert.NotNull(token);
    }
}
