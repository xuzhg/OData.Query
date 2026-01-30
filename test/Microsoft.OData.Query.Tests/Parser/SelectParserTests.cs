//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tests.Commons;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests.Parser;

public class SelectParserTests
{
    private QueryParserContext _context = new QueryParserContext<object>();
    private ISelectParser _parser = new SelectParser();

    [Fact]
    public async Task ParseSelectOption_ThrowsArgumentNull_Select()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("select", async () => await _parser.ParseAsync(null, null));
        await Assert.ThrowsAsync<ArgumentNullException>("select", async () => await _parser.ParseAsync("".AsMemory(), null));
    }

    [Fact]
    public async Task ParseSelectOption_ThrowsArgumentNull_Context()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () => await _parser.ParseAsync("any".AsMemory(), null));
    }
}
