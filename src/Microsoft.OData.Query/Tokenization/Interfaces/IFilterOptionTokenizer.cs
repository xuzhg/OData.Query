//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

public interface IFilterOptionTokenizer
{
    /// <summary>
    /// Tokenize the $filter expression like "Name eq 'Sam'" to a search tree.
    /// </summary>
    /// <param name="filter">The $filter expression string to tokenize.</param>
    /// <returns>The filter token tokenized.</returns>
    QueryToken Tokenize(string filter, QueryTokenizerContext context);
}
