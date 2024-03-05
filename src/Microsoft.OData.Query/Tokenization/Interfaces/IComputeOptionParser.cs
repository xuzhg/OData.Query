//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $compute query expression and produces the lexical object model.
/// </summary>
public interface IComputeOptionTokenizer
{
    /// <summary>
    /// Tokenize $compute query option.
    /// </summary>
    /// <param name="compute">The $compute expression string to tokenize.</param>
    /// <returns>The compute token tokenized.</returns>
    ValueTask<ComputeToken> TokenizeAsync(string compute, QueryTokenizerContext context);
}