//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A parser to parse a $skip clause.
/// </summary>
public interface ISkipParser
{
    /// <summary>
    /// Parses the $skip expression to an integer value.
    /// </summary>
    /// <param name="skip">The $skip expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $skip option.</returns>
    ValueTask<long> ParseAsync(ReadOnlyMemory<char> skip, QueryParserContext context);
}
