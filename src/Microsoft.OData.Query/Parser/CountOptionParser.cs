//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $count clause.
/// </summary>
public class CountOptionParser : ICountOptionParser
{
    /// <summary>
    /// Parses the $count expression to a boolean value.
    /// Valid Samples: $count=true; $count=false
    /// </summary>
    /// <param name="count">The $count expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $count option.</returns>
    public virtual async ValueTask<bool> ParseAsync(string count, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(count))
        {
            throw new ArgumentNullException(nameof(count));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        bool result;
        if (context.EnableCaseInsensitive)
        {
            if (!bool.TryParse(count, out result))
            {
                throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidBooleanQueryOptionValue, count));
            }
        }
        else
        {
            switch (count.Trim())
            {
                case "true":
                    result = true;
                    break;

                case "false":
                    result = false;
                    break;

                default:
                    throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidBooleanQueryOptionValue, count));
            }
        }

        return await ValueTask.FromResult(result);
    }
}
