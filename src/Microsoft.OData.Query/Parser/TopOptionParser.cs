//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $top clause.
/// </summary>
public class TopOptionParser : ITopOptionParser
{
    /// <summary>
    /// Parses the $top expression to an integer value.
    /// </summary>
    /// <param name="top">The $top expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $top option.</returns>
    public virtual async ValueTask<long> ParseAsync(string top, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(top))
        {
            throw new ArgumentNullException(nameof(top));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!long.TryParse(top, out long indexValue))
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidIntegerQueryOptionValue, top, "$top"));
        }

        return await ValueTask.FromResult(indexValue);
    }
}
