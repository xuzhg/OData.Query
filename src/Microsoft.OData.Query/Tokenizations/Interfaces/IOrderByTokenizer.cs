//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $orderby query expression and produces the token object model.
/// </summary>
public interface IOrderByTokenizer
{
    /// <summary>
    /// Tokenizes the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to tokenize.</param>
    /// <param name="context">The context for tokenizing the $orderby expression.</param>
    /// <returns>The order by token tokenized.</returns>
    ValueTask<OrderByToken> TokenizeAsync(ReadOnlyMemory<char> orderBy, QueryTokenizerContext context);
}
