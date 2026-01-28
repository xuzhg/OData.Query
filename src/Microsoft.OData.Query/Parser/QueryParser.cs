//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

public record QueryParsedResult
{
    public ComputeClause Compute = null;

    public FilterClause Filter = null;

    public bool? Count = null;

    public long? Top = null;
    public long? Skip = null;

    /// <summary>
    /// The value of the $index system query option is the zero-based ordinal position where the item is to be inserted.
    /// The ordinal positions of items within the collection greater than or equal to the inserted position are increased by one.
    /// A negative ordinal number indexes from the end of the collection, with -1 representing an insert as the last item in the collection.
    /// </summary>
    public long? Index = null;
}

public class QueryParser : IQueryParser
{
    /// <summary>
    /// Parses the input query string to query results.
    /// </summary>
    /// <param name="query">The odata query string, it should be escaped query string.</param>
    /// <param name="context">The parser context.</param>
    /// <returns>The query parsed result.</returns>
    public virtual async ValueTask<QueryParsedResult> ParseAsync(ReadOnlyMemory<char> query, QueryParserContext context)
    {
        if (query.IsEmpty)
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IDictionary<string, ReadOnlyMemory<char>> queryOptionsDict = QueryStringHelpers.SplitQuery(query);

        QueryParsedResult result = new QueryParsedResult();

        // Parse $apply first, keep this order.
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.Apply, context, out ReadOnlyMemory<char> apply))
        {
            await ParseApplyAsync(apply, context, result);
        }

        // Parse $compute second, keep this order, since the computed properties may be used in filter/orderby/select/expand
        if (queryOptionsDict.TryGetQueryOption(QueryStringConstants.Compute, context, out ReadOnlyMemory<char> compute))
        {
            await ParseComputeAsync(compute, context, result);
        }

        // Then parse others
        foreach (var queryOptionItem in queryOptionsDict)
        {
            string normalizedQueryOptionName = NormalizeQueryOption(queryOptionItem.Key, context);

            switch (normalizedQueryOptionName)
            {
                case QueryStringConstants.Apply: // $apply
                case QueryStringConstants.Compute: // $compute
                    // already parsed ahead
                    break;

                case QueryStringConstants.Filter: // $filter
                    await ParseFilterAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.OrderBy: // $orderby
                    await ParseOrderByAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Select: // $select
                    await ParseSelectAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Expand: // $expand
                    await ParseExpandAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Search: // $search
                    await ParseSearchAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Count: // $count
                    await ParseCountAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Index: // $index
                    await ParseIndexAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Top: // $top
                    await ParseTopAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.Skip: // $skip
                    await ParseSkipAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.SkipToken: // $skiptoken
                    await ParseSkipTokenAsync(queryOptionItem.Value, context, result);
                    break;

                case QueryStringConstants.DeltaToken: // $deltatoken
                    await ParseDeltaTokenAsync(queryOptionItem.Value, context, result);
                    break;

                default:
                    await ParseCustomizedAsync(queryOptionItem.Key, queryOptionItem.Value, context, result);
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apply"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual async ValueTask ParseApplyAsync(ReadOnlyMemory<char> apply, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.Apply != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.Apply, context);
        //}

        //IApplyOptionParser applyParser = _serviceProvider?.GetService<IApplyOptionParser>() ?? new ApplyOptionParser();
        //queryOption.Apply = await applyParser.ParseAsync(apply.Span.ToString(), context);
    }

    /// <summary>
    /// Parses a compute clause on the given full Uri, binding the text into semantic nodes using the constructed mode.
    /// </summary>
    /// <param name="compute">The $compute query option value.</param>
    /// <param name="context">The parser context.</param>
    /// <param name="result">The parsed result.</param>
    protected virtual async ValueTask ParseComputeAsync(ReadOnlyMemory<char> compute, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Compute != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Compute, context);
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IComputeParser computeParser = context.GetOrCreateComputeParser();

        result.Compute = await computeParser.ParseAsync(compute, context);
    }

    protected virtual async ValueTask ParseFilterAsync(ReadOnlyMemory<char> filter, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Filter != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Filter, context);
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Get the filter parser service from parser context
        IFilterParser filterParser = context.GetOrCreateFilterParser();

        result.Filter = await filterParser.ParseAsync(filter, context);
    }

    protected virtual async ValueTask ParseOrderByAsync(ReadOnlyMemory<char> orderBy, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.OrderBy != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.OrderBy, context);
        //}

        //IOrderByOptionParser orderByParser = _serviceProvider?.GetService<IOrderByOptionParser>() ?? new OrderByOptionParser();
        //queryOption.OrderBy = await orderByParser.ParseAsync(orderBy.Span.ToString(), context);
    }

    protected virtual async ValueTask ParseSelectAsync(ReadOnlyMemory<char> select, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.Select != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.Select, context);
        //}

        //ISelectOptionParser selectParser = _serviceProvider?.GetService<ISelectOptionParser>() ?? new SelectOptionParser();
        //queryOption.Select = await selectParser.ParseAsync(select.Span.ToString(), context);
    }

    protected virtual async ValueTask ParseExpandAsync(ReadOnlyMemory<char> expand, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.Expand != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.Expand, context);
        //}

        //IExpandOptionParser expandParser = _serviceProvider?.GetService<IExpandOptionParser>() ?? new ExpandOptionParser();
        //queryOption.Expand = await expandParser.ParseAsync(expand.Span.ToString(), context);
    }

    /// <summary>
    /// The $count system query option with a value of true specifies that the total count of items within a collection matching the request be returned along with the result.
    /// </summary>
    /// <param name="count">The $count string from the query</param>
    /// <param name="context">The parser context.</param>
    /// <param name="result">The parsed result so far.</param>
    /// <returns>The ValueTask.</returns>
    protected virtual async ValueTask ParseCountAsync(ReadOnlyMemory<char> count, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Count != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Count, context);
        }

        StringComparison comparison = context.EnableCaseInsensitive ?
            StringComparison.OrdinalIgnoreCase :
            StringComparison.Ordinal;

        if (count.Span.Equals("true", comparison))
        {
            result.Count = true;
        }
        else if (count.Span.Equals("false", comparison))
        {
            result.Count = false;
        }
        else
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidDollarCount, count.Span.ToString()));
        }
    }

    /// <summary>
    /// The $top system query option specifies a non-negative integer n that limits the number of items returned from a collection.
    /// </summary>
    /// <param name="top">The $top string from the query</param>
    /// <param name="context">The parser context.</param>
    /// <param name="result">The parsed result so far.</param>
    /// <returns>The ValueTask.</returns>
    protected virtual async ValueTask ParseTopAsync(ReadOnlyMemory<char> top, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Top != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Top, context);
        }

        long topValue;
        if (!long.TryParse(top.Span, out topValue) || topValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidNonNegativeIntegerValue, top.Span.ToString(), QueryStringConstants.Top));
        }

        result.Top = topValue;
    }

    /// <summary>
    /// The $skip system query option specifies a non-negative integer n that excludes the first n items of the queried collection from the result.
    /// </summary>
    /// <param name="skip">The $skip string from the query.</param>
    /// <param name="context">The parser context.</param>
    /// <param name="result">The parsed result so far.</param>
    /// <returns>The ValueTask.</returns>
    protected virtual async ValueTask ParseSkipAsync(ReadOnlyMemory<char> skip, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Skip != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Skip, context);
        }

        long skipValue;
        if (!long.TryParse(skip.Span, out skipValue) || skipValue < 0)
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidNonNegativeIntegerValue, skip.Span.ToString(), QueryStringConstants.Skip));
        }

        result.Skip = skipValue;
    }

    /// <summary>
    /// The $index system query option allows clients to do a positional insert into a collection annotated with using the Core.PositionalInsert term.
    /// </summary>
    /// <param name="index">The $index string from the query.</param>
    /// <param name="context">The parser context.</param>
    /// <param name="result">The parsed result so far.</param>
    /// <returns>The ValueTask.</returns>
    protected virtual async ValueTask ParseIndexAsync(ReadOnlyMemory<char> index, QueryParserContext context, QueryParsedResult result)
    {
        if (result.Index != null)
        {
            ThrowQueryParameterMoreThanOnce(QueryStringConstants.Index, context);
        }

        long indexValue;
        if (!long.TryParse(index.Span, out indexValue))
        {
            throw new QueryParserException(Error.Format(SRResources.QueryParser_InvalidIntegerValue, index.Span.ToString(), QueryStringConstants.Index));
        }

        result.Index = indexValue;
    }

    protected virtual async ValueTask ParseSearchAsync(ReadOnlyMemory<char> search, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.Search != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.Search, context);
        //}

        //ISearchOptionParser searchParser = _serviceProvider?.GetService<ISearchOptionParser>() ?? new SearchOptionParser();
        //queryOption.Search = await searchParser.ParseAsync(search.Span.ToString(), context);
    }

    protected virtual async ValueTask ParseSkipTokenAsync(ReadOnlyMemory<char> skipToken, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.SkipToken != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.SkipToken, context);
        //}

        //queryOption.SkipToken = skipToken.Span.ToString();
        //await Task.CompletedTask;
    }

    protected virtual async ValueTask ParseDeltaTokenAsync(ReadOnlyMemory<char> deltaToken, QueryParserContext context, QueryParsedResult result)
    {
        //if (queryOption.DeltaToken != null)
        //{
        //    ThrowQueryParameterMoreThanOnce(QueryStringConstants.DeltaToken, context);
        //}

        //queryOption.DeltaToken = deltaToken.Span.ToString();
        //await Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual async Task ParseCustomizedAsync(string key, ReadOnlyMemory<char> value, QueryParserContext context, QueryParsedResult result)
    {
        // by default, nothing here
        await Task.CompletedTask;
    }

    private static void ThrowQueryParameterMoreThanOnce(string queryName, QueryParserContext context)
    {
        throw new QueryParserException(Error.Format(SRResources.QueryOptionParser_QueryParameterMustBeSpecifiedOnce,
            queryName,
            context.EnableCaseInsensitive ? "Enabled" : "Disabled",
            context.EnableNoDollarPrefix ? "Enabled" : "Disabled"));
    }

    private static string NormalizeQueryOption(string queryOptionName, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(queryOptionName))
        {
            return queryOptionName;
        }

        string changedQueryOptionName = queryOptionName;
        if (context.EnableNoDollarPrefix && !queryOptionName.StartsWith("$", StringComparison.Ordinal))
        {
            changedQueryOptionName = $"${changedQueryOptionName}";
        }

        return context.EnableCaseInsensitive ? changedQueryOptionName.ToLowerInvariant() : changedQueryOptionName;
    }
}