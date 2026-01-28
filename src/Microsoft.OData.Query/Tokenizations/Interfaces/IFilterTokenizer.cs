//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $filter query expression and produces the lexical object model.
/// </summary>
public interface IFilterTokenizer
{
    /// <summary>
    /// Tokenizes the $filter expression like "Name eq 'Sam'" to tokens.
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <param name="context">The query tokenizer context.</param>
    /// <returns>The filter token tokenized.</returns>
    ValueTask<IQueryToken> TokenizeAsync(ReadOnlyMemory<char> filter, QueryTokenizerContext context);
}
