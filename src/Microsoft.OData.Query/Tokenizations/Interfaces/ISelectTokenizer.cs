//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $select query expression and produces the lexical object model.
/// </summary>
public interface ISelectTokenizer
{
    /// <summary>
    /// Tokenizes the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to tokenize.</param>
    /// <returns>The select token tokenized.</returns>
    ValueTask<SelectToken> TokenizeAsync(ReadOnlyMemory<char> select, QueryTokenizerContext context);
}

public static class ISelectTokenizerExtensions
{
    public static async ValueTask<SelectToken> TokenizeAsync(this ISelectTokenizer tokenizer, string select, QueryTokenizerContext context)
        => await tokenizer.TokenizeAsync(select.AsMemory(), context);
}