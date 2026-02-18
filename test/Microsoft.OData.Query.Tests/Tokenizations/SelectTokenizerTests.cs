//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tests.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;
using Xunit;

namespace Microsoft.OData.Query.Tests.Tokenizations;

public class SelectTokenizerTests
{
    private QueryTokenizerContext _context;
    private readonly ISelectTokenizer _selectTokenizer;

    public SelectTokenizerTests()
    {
        _selectTokenizer = new SelectTokenizer();
        _context = new QueryTokenizerContext();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task SelectTokenizer_ThrowsForNullOrEmpty(string select)
    {
        await Assert.ThrowsAsync<ArgumentNullException>("select", async () =>
        {
            await _selectTokenizer.TokenizeAsync(select, _context);
        });
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

    [Fact]
    public async Task SelectTokenizer_CanTokenizeMultipleSelectItems()
    {
        // Arrange
        SelectToken token = await _selectTokenizer.TokenizeAsync("foo,bar,thing,prop,yoda", _context);

        // Assert
        Assert.Equal(5, token.Count);
    }

    [Fact]
    public async Task SelectTokenizer_ReturnsEmptyTokenForWhitespace()
    {
        // Arrange
        string select = "   ";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.NotNull(token);
        Assert.Empty(token);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeNestedPath()
    {
        // Arrange
        string select = "Address/City";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        SelectItemToken selectItem = Assert.Single(token);
        Assert.NotNull(selectItem.PathToProperty);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeMultipleNestedPaths()
    {
        // Arrange
        string select = "Address/City,Person/Name,Product/Price";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(3, token.Count);
        Assert.All(token, item => Assert.NotNull(item.PathToProperty));
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeWildcard()
    {
        // Arrange
        string select = "*";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        SelectItemToken selectItem = Assert.Single(token);
        Assert.NotNull(selectItem.PathToProperty);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeWildcardWithOtherProperties()
    {
        // Arrange
        string select = "*,Name,Age";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(3, token.Count);
    }

    [Fact]
    public async Task SelectTokenizer_HandlesTrailingComma()
    {
        // Arrange
        string select = "Name,Age,";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(2, token.Count);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeQualifiedName()
    {
        // Arrange
        string select = "Namespace.EntityType/Property";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        SelectItemToken selectItem = Assert.Single(token);
        Assert.NotNull(selectItem.PathToProperty);
    }

    [Fact]
    public async Task SelectTokenizer_ThrowsOnInvalidSyntax()
    {
        // Arrange
        string select = "Name,Age Extra";

        // Act & Assert
        await Assert.ThrowsAsync<QueryTokenizerException>(async () =>
        {
            await _selectTokenizer.TokenizeAsync(select, _context);
        });
    }

    [Fact]
    public async Task SelectTokenizer_TokenKindIsSelect()
    {
        // Arrange
        string select = "Name";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(QueryTokenKind.Select, token.Kind);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeWithSpaces()
    {
        // Arrange
        string select = "  Name  ,  Age  ";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(2, token.Count);
    }

    [Fact]
    public async Task SelectTokenizer_SelectItemHasCorrectKind()
    {
        // Arrange
        string select = "Name";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        SelectItemToken selectItem = Assert.Single(token);
        Assert.Equal(QueryTokenKind.SelectItem, selectItem.Kind);
    }

    [Fact]
    public async Task SelectTokenizer_MultipleItemsAreAllSelectItems()
    {
        // Arrange
        string select = "Name,Age,Email";

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(3, token.Count);
        Assert.All(token, item =>
        {
            Assert.Equal(QueryTokenKind.SelectItem, item.Kind);
            Assert.NotNull(item.PathToProperty);
        });
    }

    [Fact]
    public async Task SelectTokenizer_ThrowsWhenContextIsNull()
    {
        // Arrange
        string select = "Name";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>("context", async () =>
        {
            await _selectTokenizer.TokenizeAsync(select, null);
        });
    }

    [Theory]
    [InlineData("A")]
    [InlineData("a")]
    [InlineData("Name123")]
    [InlineData("_underscore")]
    public async Task SelectTokenizer_CanTokenizeSinglePropertyVariations(string propertyName)
    {
        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(propertyName, _context);

        // Assert
        SelectItemToken selectItem = Assert.Single(token);
        Assert.NotNull(selectItem.PathToProperty);
    }

    [Fact]
    public async Task SelectTokenizer_CanTokenizeManyItems()
    {
        // Arrange
        string select = string.Join(",", Enumerable.Range(1, 20).Select(i => $"Prop{i}"));

        // Act
        SelectToken token = await _selectTokenizer.TokenizeAsync(select, _context);

        // Assert
        Assert.Equal(20, token.Count);
    }
}
