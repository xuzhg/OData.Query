//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $count clause.
/// </summary>
public interface ICountOptionParser
{
    /// <summary>
    /// Parses the $count expression to a boolean value.
    /// </summary>
    /// <param name="count">The $count expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $count option.</returns>
    ValueTask<bool> ParseAsync(string count, QueryParserContext context);
}