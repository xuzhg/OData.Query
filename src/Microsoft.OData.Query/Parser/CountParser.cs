//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $count clause.
/// </summary>
public class CountParser : ICountParser
{
    /// <summary>
    /// Parses the $count expression to a boolean value.
    /// Valid Samples: $count=true; $count=false
    /// </summary>
    /// <param name="count">The $count expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $count option.</returns>
    public virtual async ValueTask<bool> ParseAsync(ReadOnlyMemory<char> count, QueryParserContext context)
    {
        if (count.IsEmpty)
        {
            throw new ArgumentNullException(nameof(count));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        ReadOnlySpan<char> trimmedCount = count.Span.Trim();

        bool result;
        if (context.EnableCaseInsensitive)
        {
            if (!bool.TryParse(trimmedCount, out result))
            {
                throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidBooleanQueryOptionValue, count.Span.ToString()));
            }
        }
        else if (trimmedCount.Equals("true", StringComparison.Ordinal))
        {
            result = true;
        }
        else if (trimmedCount.Equals("false", StringComparison.Ordinal))
        {
            result = false;
        }
        else
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidBooleanQueryOptionValue, count.Span.ToString()));
        }

        return result;
    }
}
