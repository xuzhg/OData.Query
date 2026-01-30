//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $search expression and produces the search clause.
/// </summary>
public interface ISearchParser
{
    /// <summary>
    /// Parses the $search expression like "$search=food" to a search tree.
    /// </summary>
    /// <param name="search">The $search expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The search clause parsed.</returns>
    ValueTask<SearchClause> ParseAsync(ReadOnlyMemory<char> search, QueryParserContext context);
}
