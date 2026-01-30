//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $expand query expression and produces the token object model.
/// </summary>
public interface IExpandTokenizer
{
    /// <summary>
    /// Tokenizes the $expand expression.
    /// </summary>
    /// <param name="expand">The $expand expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The expand token tokenized.</returns>
    ValueTask<ExpandToken> TokenizeAsync(ReadOnlyMemory<char> expand, QueryTokenizerContext context);
}
