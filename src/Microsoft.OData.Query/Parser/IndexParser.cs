//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $index clause.
/// </summary>
public class IndexParser : IIndexParser
{
    /// <summary>
    /// Parses the $index expression to an integer value.
    /// </summary>
    /// <param name="index">The $index expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $index option.</returns>
    public virtual async ValueTask<long> ParseAsync(ReadOnlyMemory<char> index, QueryParserContext context)
    {
        if (index.IsEmpty)
        {
            throw new ArgumentNullException(nameof(index));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // A negative ordinal number indexes from the end of the collection,
        // with -1 representing an insert as the last item in the collection.
        if (!long.TryParse(index.Span, out long indexValue))
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidIntegerQueryOptionValue, index.ToString(), "$index"));
        }

        return indexValue;
    }
}
