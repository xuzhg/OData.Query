//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Metadata;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a filter clause.
/// </summary>
public class SearchOptionParser : QueryOptionParser, ISearchOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchOptionParser" /> class.
    /// </summary>
    public SearchOptionParser()
        : this(SearchOptionTokenizer.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizer">The search option tokenizer.</param>
    public SearchOptionParser(ISearchOptionTokenizer tokenizer)
    {
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    /// <summary>
    /// Gets the tokenizer.
    /// </summary>
    public ISearchOptionTokenizer Tokenizer { get; }

    /// <summary>
    /// Parses the $search expression to a search tree.
    /// </summary>
    /// <param name="search">The $search expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<SearchClause> ParseAsync(string search, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(search))
        {
            throw new ArgumentNullException(nameof(search));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        QueryToken token = await Tokenizer.TokenizeAsync(search, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        SearchClause searchClause = new SearchClause(/*expressionResultNode, context.ImplicitRangeVariable*/);

        return searchClause;
    }
}
