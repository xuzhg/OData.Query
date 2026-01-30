//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $filter expression and produces the filter clause.
/// </summary>
public interface IFilterParser
{
    /// <summary>
    /// Parses the $filter expression like "Name eq 'Sam'" to a search tree.
    /// </summary>
    /// <param name="filter">The $filter expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter clause parsed.</returns>
    ValueTask<FilterClause> ParseAsync(ReadOnlyMemory<char> filter, QueryParserContext context);
}
