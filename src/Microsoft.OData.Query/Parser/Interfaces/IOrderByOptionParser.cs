//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the $orderby query expression and produces the lexical object model.
/// </summary>
public interface IOrderByOptionParser
{
    /// <summary>
    /// Parses the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <returns>The order by token parsed.</returns>
    OrderByClause Parse(OrderByToken orderBy, QueryOptionParserContext context);
}
