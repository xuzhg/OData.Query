//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $orderby clause.
/// </summary>
public interface IOrderByOptionParser
{
    /// <summary>
    /// Parses the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <returns>The order by clause parsed.</returns>
    ValueTask<OrderByClause> ParseAsync(string orderBy, QueryParserContext context);
}
