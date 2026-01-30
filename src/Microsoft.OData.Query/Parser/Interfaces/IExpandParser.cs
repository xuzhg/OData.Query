//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $expand clause.
/// </summary>
public interface IExpandParser
{
    /// <summary>
    /// Parses the $expand expression.
    /// </summary>
    /// <param name="expand">The $expand expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The expand clause parsed.</returns>
    ValueTask<ExpandClause> ParseAsync(ReadOnlyMemory<char> expand, QueryParserContext context);
}
