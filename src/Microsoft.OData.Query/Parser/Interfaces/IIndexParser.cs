//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $index clause.
/// </summary>
public interface IIndexParser
{
    /// <summary>
    /// Parses the $index expression to an integer value.
    /// </summary>
    /// <param name="index">The $index expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $index option.</returns>
    ValueTask<long> ParseAsync(ReadOnlyMemory<char> index, QueryParserContext context);
}
