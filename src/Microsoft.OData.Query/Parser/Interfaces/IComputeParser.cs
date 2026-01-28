//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $compute expression and produces the compute clause.
/// </summary>
public interface IComputeParser
{
    /// <summary>
    /// Parses the $compute expression like "$compute=Price mul 1.2 as NewPrice" to a compute clause.
    /// </summary>
    /// <param name="compute">The $compute query option value.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The compute clause.</returns>
    ValueTask<ComputeClause> ParseAsync(ReadOnlyMemory<char> compute, QueryParserContext context);
}
