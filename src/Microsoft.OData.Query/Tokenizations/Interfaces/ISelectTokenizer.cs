//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $select query expression and produces the query token object model.
/// </summary>
public interface ISelectTokenizer
{
    /// <summary>
    /// Tokenizes the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to tokenize.</param>
    /// <param name="context">The context for tokenization, providing necessary information and services.</param>
    /// <returns>The select token tokenized.</returns>
    ValueTask<SelectToken> TokenizeAsync(ReadOnlyMemory<char> select, QueryTokenizerContext context);
}