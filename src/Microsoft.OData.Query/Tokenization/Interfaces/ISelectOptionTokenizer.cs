//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $select query expression and produces the lexical object model.
/// </summary>
public interface ISelectOptionTokenizer
{
    /// <summary>
    /// Tokenize the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to tokenize.</param>
    /// <returns>The select token tokenized.</returns>
    ValueTask<SelectToken> TokenizeAsync(string select, QueryTokenizerContext context);
}