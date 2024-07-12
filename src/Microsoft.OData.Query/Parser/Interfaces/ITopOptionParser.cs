//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $top clause.
/// </summary>
public interface ITopOptionParser
{
    /// <summary>
    /// Parses the $top expression to an integer value.
    /// </summary>
    /// <param name="top">The $top expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $top option.</returns>
    ValueTask<long> ParseAsync(string top, QueryParserContext context);
}