//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class FilterParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private IFilterParser _parser = new FilterParser();

    [Fact]
    public async Task ParseFilterOption_ThrowsArgumentNull_Filter()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("filter", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("filter", async () => await _parser.ParseAsync("".AsMemory(), null));
    }

    [Fact]
    public async Task ParseFilterOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any".AsMemory(), null));
    }

    [Fact]
    public void ParseFilter_WorksForBasicFilterExpression()
    {
        //FilterOptionParser parser = new FilterOptionParser(new OTokenizerFactory());

        //QueryToken token = parser.ParseFilter("2 eq Id", new OrderByOptionParserContext());

        //Assert.NotNull(token);
        //BinaryOperatorToken binaryOperatorToken = Assert.IsType<BinaryOperatorToken>(token);
        //Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorToken.OperatorKind);
    }
}
