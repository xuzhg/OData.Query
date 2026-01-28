//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class SelectOptionTokenizerTests
{
    private readonly ISelectOptionTokenizer _selectTokenizer;

    public SelectOptionTokenizerTests()
    {
        _selectTokenizer = new SelectOptionTokenizer();
    }

    [Fact]
    public async ValueTask SelectTokenizer_CanTokenizeSelectWordAndPhrase()
    {
        // Arrange
        string select = "name";

        // & Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, QueryTokenizerContext.Default);

        // Assert
        IQueryToken selectItemToken = Assert.Single(token);
        selectItemToken.ShouldBeStringLiteralToken("name");
    }
}
