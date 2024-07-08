//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the $select query expression and produces the abstract search node.
/// </summary>
public interface ISelectOptionParser
{
    /// <summary>
    /// Parses the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to parse.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The select clause parsed.</returns>
    ValueTask<SelectClause> ParseAsync(string select, QueryParserContext context);
}
