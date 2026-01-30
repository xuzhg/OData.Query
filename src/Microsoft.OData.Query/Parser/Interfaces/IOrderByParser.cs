//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $orderby clause.
/// </summary>
public interface IOrderByParser
{
    /// <summary>
    /// Parses the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The order by clause parsed.</returns>
    ValueTask<OrderByClause> ParseAsync(ReadOnlyMemory<char> orderBy, QueryParserContext context);
}
