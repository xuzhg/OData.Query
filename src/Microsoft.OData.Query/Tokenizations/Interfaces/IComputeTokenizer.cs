//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenize the $compute query expression and produces the lexical object model.
/// </summary>
public interface IComputeTokenizer
{
    /// <summary>
    /// Tokenizes $compute query option.
    /// </summary>
    /// <param name="compute">The $compute expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The compute token tokenized.</returns>
    ValueTask<ComputeToken> TokenizeAsync(ReadOnlyMemory<char> compute, QueryTokenizerContext context);
}