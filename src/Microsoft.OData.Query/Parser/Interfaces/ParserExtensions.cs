//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

public static class ParserExtensions
{
    /// <summary>
    /// Parses the $count expression to a boolean value.
    /// </summary>
    /// <param name="countParser">The count parser.</param>
    /// <param name="count">The $count expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $count option.</returns>
    public static async ValueTask<bool> ParseAsync(this ICountParser countParser, string count, QueryParserContext context)
        => await countParser.ParseAsync(count.AsMemory(), context);

    /// <summary>
    /// Parses the $skip expression to a long value.
    /// </summary>
    /// <param name="skipParser">The skip parser.</param>
    /// <param name="skip">The $skip expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $skip option.</returns>
    public static async ValueTask<long> ParseAsync(this ISkipParser skipParser, string skip, QueryParserContext context)
        => await skipParser.ParseAsync(skip.AsMemory(), context);

    /// <summary>
    /// Parses the $top expression to a long value.
    /// </summary>
    /// <param name="topParser">The top parser.</param>
    /// <param name="top">The $top expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $top option.</returns>
    public static async ValueTask<long> ParseAsync(this ITopParser topParser, string top, QueryParserContext context)
        => await topParser.ParseAsync(top.AsMemory(), context);

    /// <summary>
    /// Parses the $index expression to a long value.
    /// </summary>
    /// <param name="indexParser">The index parser.</param>
    /// <param name="index">The $index expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $index option.</returns>
    public static async ValueTask<long> ParseAsync(this IIndexParser indexParser, string index, QueryParserContext context)
        => await indexParser.ParseAsync(index.AsMemory(), context);
}