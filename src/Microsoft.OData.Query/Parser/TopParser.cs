//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $top clause.
/// </summary>
public class TopParser : ITopParser
{
    /// <summary>
    /// Parses the $top expression to an integer value.
    /// </summary>
    /// <param name="top">The $top expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $top option.</returns>
    public virtual async ValueTask<long> ParseAsync(ReadOnlyMemory<char> top, QueryParserContext context)
    {
        if (top.IsEmpty)
        {
            throw new ArgumentNullException(nameof(top));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!long.TryParse(top.Span, out long topValue) || topValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidNonNegativeIntegerValue, top.Span.ToString(), "$top"));
        }

        return topValue;
    }
}
