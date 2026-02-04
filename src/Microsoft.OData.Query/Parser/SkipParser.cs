//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $skip clause.
/// </summary>
public class SkipParser : ISkipParser
{
    /// <summary>
    /// Parses the $skip expression to an integer value.
    /// </summary>
    /// <param name="skip">The $skip expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $skip option.</returns>
    public virtual async ValueTask<long> ParseAsync(ReadOnlyMemory<char> skip, QueryParserContext context)
    {
        if (skip.IsEmpty)
        {
            throw new ArgumentNullException(nameof(skip));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!long.TryParse(skip.Span, out long skipValue) || skipValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidNonNegativeIntegerValue, skip.Span.ToString(), "$skip"));
        }

        return skipValue;
    }
}