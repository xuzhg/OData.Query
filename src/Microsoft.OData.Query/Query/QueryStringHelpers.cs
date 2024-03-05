//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query;

/// <summary>
/// Helper methods for query option string.
/// </summary>
internal static class QueryStringHelpers
{
    /// <summary>
    /// Split the query string into key value pairs.
    /// </summary>
    /// <param name="query">The escaped query string.</param>
    /// <returns>The key value pairs.</returns>
    public static IDictionary<string, ReadOnlyMemory<char>> SplitQuery(string query)
    {
        IDictionary<string, ReadOnlyMemory<char>> queryOptions
            = new Dictionary<string, ReadOnlyMemory<char>>();

        QueryStringLexer lexer = new QueryStringLexer(query);
        while (lexer.MoveNext())
        {
            //Uri.UnescapeDataString()
            queryOptions.Add(lexer.CurrentName.Span.ToString(), lexer.CurrentValue);
        }

        return queryOptions;
    }

    /// <summary>
    /// Split the query string into key value pairs.
    /// </summary>
    /// <param name="query">The escaped query string.</param>
    /// <returns>The key value pairs.</returns>
    public static IDictionary<string, string> SplitQuery2(string query)
    {
        IDictionary<string, string> queryOptions
            = new Dictionary<string, string>();

        QueryStringLexer lexer = new QueryStringLexer(query);
        while (lexer.MoveNext())
        {
            queryOptions.Add(
                lexer.CurrentName.Decode(),
                lexer.CurrentValue.Decode());
        }

        return queryOptions;
    }

    private static string Decode(this ReadOnlyMemory<char> chars)
    {
        // If the value is short, it's cheap to check up front if it really needs decoding. If it doesn't,
        // then we can save some allocations.
        if (chars.Length < 16 && chars.Span.IndexOfAny('%', '+') < 0)
        {
            return chars.Span.ToString();
        }

        ReadOnlySpan<char> span = chars.Span;
        return Uri.UnescapeDataString(span.ToString());
    }

    /// <summary>
    /// Gets query options according to case sensitivity and whether no dollar query options is enabled.
    /// </summary>
    /// <param name="name">The query option name with $ prefix.</param>
    /// <param name="value">The value of the query option.</param>
    /// <returns>Whether value successfully retrieved.</returns>
    public static bool TryGetQueryOption(this IDictionary<string, ReadOnlyMemory<char>> queryOptions,
        string name, Parser.QueryParserContext context, out ReadOnlyMemory<char> value)
    {
        value = null;
        if (name == null)
        {
            return false;
        }

        // Trim name to prevent caller from passing in untrimmed name for comparison with already trimmed keys in queryOptions dictionary.
        string trimmedName = name.Trim();

        bool isCaseInsensitiveEnabled = context.EnableIdentifierCaseSensitive;
        bool isNoDollarQueryOptionsEnabled = context.EnableNoDollarSignOption;

        if (!isCaseInsensitiveEnabled && !isNoDollarQueryOptionsEnabled)
        {
            return queryOptions.TryGetValue(trimmedName, out value);
        }

        StringComparison stringComparison = isCaseInsensitiveEnabled ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        string nameWithoutDollarPrefix = (isNoDollarQueryOptionsEnabled && trimmedName.StartsWith("$", StringComparison.Ordinal)) ?
            trimmedName.Substring(1) : null;

        var list = queryOptions
            .Where(pair => string.Equals(trimmedName, pair.Key, stringComparison)
            || (nameWithoutDollarPrefix != null && string.Equals(nameWithoutDollarPrefix, pair.Key, stringComparison)))
            .ToList();

        if (list.Count == 0)
        {
            return false;
        }
        else if (list.Count == 1)
        {
            value = list.First().Value;
            return true;
        }

        throw new Exception("Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(");
    }
}
