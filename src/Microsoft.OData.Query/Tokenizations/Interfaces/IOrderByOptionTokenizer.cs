//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $orderby query expression and produces the lexical object model.
/// </summary>
public interface IOrderByOptionTokenizer
{
    /// <summary>
    /// Tokenizes the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to tokenize.</param>
    /// <returns>The order by token tokenized.</returns>
    ValueTask<OrderByToken> TokenizeAsync(string orderBy, QueryTokenizerContext context);
}
