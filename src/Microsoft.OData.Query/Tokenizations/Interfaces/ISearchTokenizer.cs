//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenize the $search query expression and produces the lexical object model.
/// </summary>
public interface ISearchTokenizer
{
    /// <summary>
    /// Tokenizes the $search expression.
    /// </summary>
    /// <param name="search">The $search expression string to tokenize.</param>
    /// <param name="context">The query tokenizer context.</param>
    /// <returns>The search token tokenized.</returns>
    ValueTask<IQueryToken> TokenizeAsync(ReadOnlyMemory<char> search, QueryTokenizerContext context);
}

public static class ISearchTokenizerExtensions
{
    public static async ValueTask<IQueryToken> TokenizeAsync(this ISearchTokenizer tokenizer, string search, QueryTokenizerContext context)
        => await tokenizer.TokenizeAsync(search.AsMemory(), context);
}
