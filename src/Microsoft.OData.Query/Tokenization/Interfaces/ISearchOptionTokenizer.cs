//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $search query expression and produces the lexical object model.
/// </summary>
public interface ISearchOptionTokenizer
{
    /// <summary>
    /// Tokenize the $search expression.
    /// </summary>
    /// <param name="search">The $search expression string to tokenize.</param>
    /// <returns>The search token tokenized.</returns>
    ValueTask<IQueryToken> TokenizeAsync(string search, QueryTokenizerContext context);
}
