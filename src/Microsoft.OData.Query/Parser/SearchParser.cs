//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a filter clause.
/// </summary>
public class SearchParser : QueryBinder, ISearchParser
{
    /// <summary>
    /// Parses the $search expression to a search tree.
    /// </summary>
    /// <param name="search">The $search expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<SearchClause> ParseAsync(ReadOnlyMemory<char> search, QueryParserContext context)
    {
        if (search.IsEmpty)
        {
            throw new ArgumentNullException(nameof(search));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        ISearchTokenizer tokenizer = context.GetOrCreateSearchTokenizer();

        IQueryToken token = await tokenizer.TokenizeAsync(search, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        SearchClause searchClause = new SearchClause((SingleValueNode)expressionNode/*, context.ImplicitRangeVariable*/);

        return searchClause;
    }
}
