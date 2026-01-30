//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenize which consumes the $apply query expression and produces the lexical object model.
/// </summary>
public interface IApplyTokenizer
{
    /// <summary>
    /// Tokenizes the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The apply token tokenized.</returns>
    ValueTask<ApplyToken> TokenizeAsync(ReadOnlyMemory<char> apply, QueryTokenizerContext context);
}
