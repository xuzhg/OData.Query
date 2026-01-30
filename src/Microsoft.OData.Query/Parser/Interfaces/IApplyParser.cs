//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the $apply query expression and produces the abstract search node.
/// </summary>
public interface IApplyParser
{
    /// <summary>
    /// Parses the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to parse.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The apply token parsed.</returns>
    ValueTask<ApplyClause> ParseAsync(ReadOnlyMemory<char> apply, QueryParserContext context);
}
