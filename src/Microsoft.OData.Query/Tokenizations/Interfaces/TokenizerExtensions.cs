//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

public static class TokenizerExtensions
{
    /// <summary>
    /// Tokenizes the given $filter expression string.
    /// </summary>
    /// <param name="tokenizer">The filter tokenizer/</param>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The filter token tokenized.</returns>
    public static async ValueTask<IQueryToken> TokenizeAsync(this IFilterTokenizer tokenizer, string filter, QueryTokenizerContext context)
    {
        if (tokenizer == null)
        {
            throw new ArgumentNullException(nameof(tokenizer));
        }

        return await tokenizer.TokenizeAsync(filter.AsMemory(), context);
    }

    /// <summary>
    /// Tokenizes the given $orderby expression string.
    /// </summary>
    /// <param name="tokenizer">The orderby tokenizer/</param>
    /// <param name="orderby">The $orderby expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The orderby token tokenized.</returns>
    public static async ValueTask<OrderByToken> TokenizeAsync(this IOrderByTokenizer tokenizer, string orderby, QueryTokenizerContext context)
    {
        if (tokenizer == null)
        {
            throw new ArgumentNullException(nameof(tokenizer));
        }

        return await tokenizer.TokenizeAsync(orderby.AsMemory(), context);
    }

    /// <summary>
    /// Tokenizes the given $search expression string.
    /// </summary>
    /// <param name="tokenizer">The search tokenizer/</param>
    /// <param name="search">The search expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The search token tokenized.</returns>
    public static async ValueTask<IQueryToken> TokenizeAsync(this ISearchTokenizer tokenizer, string search, QueryTokenizerContext context)
    {
        if (tokenizer == null)
        {
            throw new ArgumentNullException(nameof(tokenizer));
        }

        return await tokenizer.TokenizeAsync(search.AsMemory(), context);
    }
}