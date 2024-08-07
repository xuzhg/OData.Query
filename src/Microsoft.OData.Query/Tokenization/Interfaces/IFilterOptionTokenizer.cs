﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenizes the $filter query expression and produces the lexical object model.
/// </summary>
public interface IFilterOptionTokenizer
{
    /// <summary>
    /// Tokenizes the $filter expression like "Name eq 'Sam'".
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <returns>The filter token tokenized.</returns>
    ValueTask<IQueryToken> TokenizeAsync(string filter, QueryTokenizerContext context);
}
