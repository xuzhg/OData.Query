//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $expand query expression and produces the lexical object model.
/// </summary>
public interface IExpandOptionTokenizer
{
    /// <summary>
    /// Tokenize the $expand expression.
    /// </summary>
    /// <param name="expand">The $expand expression string to tokenize.</param>
    /// <returns>The expand token tokenized.</returns>
    ValueTask<ExpandToken> TokenizeAsync(string expand, QueryTokenizerContext context);
}