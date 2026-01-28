//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser to parse OData query string to query nodes.
/// </summary>
public interface IQueryParser
{
    /// <summary>
    /// Parses the input query string to query results.
    /// </summary>
    /// <param name="query">The query string to parse.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The parsed query result.</returns>
    ValueTask<QueryParsedResult> ParseAsync(ReadOnlyMemory<char> query, QueryParserContext context);
}