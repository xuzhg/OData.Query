//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

public static class ParserExtensions
{
    /// <summary>
    /// Parses the $filter expression to an <see cref="FilterClause" /> value.
    /// </summary>
    /// <param name="filterParser">The filter parser.</param>
    /// <param name="filter">The $filter expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $filter option.</returns>
    public static async ValueTask<FilterClause> ParseAsync(this IFilterParser filterParser, string filter, QueryParserContext context)
    {
        if (filterParser == null)
        {
            throw new ArgumentNullException(nameof(filterParser));
        }

        return await filterParser.ParseAsync(filter.AsMemory(), context);
    }

    /// <summary>
    /// Parses the $orderby expression to an <see cref="FilterClause"/> value.
    /// </summary>
    /// <param name="orderByParser">The orderby parser.</param>
    /// <param name="orderBy">The $orderby expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>A value representing that $orderby option.</returns>
    public static async ValueTask<OrderByClause> ParseAsync(this IOrderByParser orderByParser, string orderBy, QueryParserContext context)
    {
        if (orderByParser == null)
        {
            throw new ArgumentNullException(nameof(orderByParser));
        }

        return await orderByParser.ParseAsync(orderBy.AsMemory(), context);
    }

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