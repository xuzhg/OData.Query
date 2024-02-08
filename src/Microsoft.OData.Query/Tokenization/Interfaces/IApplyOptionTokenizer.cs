//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize which consumes the $apply query expression and produces the lexical object model.
/// </summary>
public interface IApplyOptionTokenizer
{
    /// <summary>
    /// Tokenize the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to tokenize.</param>
    /// <returns>The apply token tokenized.</returns>
    ApplyToken ParseApply(string apply, QueryTokenizerContext context);
}
