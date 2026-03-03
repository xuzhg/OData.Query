//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Expressions;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Binders;

public class QueryRawValues
{

}

/// <summary>
/// This defines a compositer for OData query options that can be used to perform query composition.
/// </summary>
public class QueryCompositer : IQueryCompositer
{
    private QueryParsedResult _parsedResult = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCompositer"/> class based on the incoming query and some metadata information from <see cref="QueryCompositerContext"/>.
    /// </summary>
    /// <param name="query">The OData query string.</param>
    /// <param name="context">The context containing metadata and services for query composition.</param>
    public QueryCompositer(ReadOnlyMemory<char> query, QueryCompositerContext context)
    {
        Query = query;
        Context = context;
    }

    public QueryCompositer(IDictionary<string, string> query, QueryCompositerContext context)
    {
        //Query = query;
        Context = context;
    }

    public ReadOnlyMemory<char> Query { get; }

    public QueryCompositerContext Context { get; }

    public virtual async ValueTask<IQueryable> CompositeAsync(IQueryable source, QueryCompositerSettings settings)
    {
        if (_parsedResult == null)
        {
            _parsedResult = await Context.GetParser().ParseAsync(Query, new QueryParserContext(source.ElementType));
        }
        
        // Check the parsedResult
        if (_parsedResult == null)
        {
            throw new InvalidOperationException("Failed to parse the query options.");
        }

        // First apply $apply
        source = await CompositeApplyAsync(_parsedResult.Apply, source);

        // $filter
        source = await CompositeFilterAsync(_parsedResult.Filter, source);

        // If both $search and $filter are specified in the same request, only those items satisfying both criteria are returned
        // apply $search
        source = await CompositeSearchAsync(_parsedResult.Search, source);

        // $count=ture/false?
        // ...

        // $orderby?
        source = await CompositeOrderByAsync(_parsedResult.OrderBy, source);

        // $skiptoken?
        source = await CompositeSkipTokenAsync(_parsedResult.SkipToken, source);

        // $select and $expand?
        source = await CompositeSelectAndExpandAsync(_parsedResult.Select, _parsedResult.Expand, source);

        // $skip
        source = await CompositeSkipAsync(_parsedResult.Skip, source);

        // $top
        source = await CompositeTopAsync(_parsedResult.Top, source);

        // paging
        source = await CompositePagingAsync(source);

        return source;
    }

    protected virtual async ValueTask<IQueryable> CompositeApplyAsync(ApplyClause apply, IQueryable query)
    {
        // bind apply to expression
        // var applyBinder = new ApplyBinder(new ApplyTokenizer(), new QueryNodeBinder());
        // Expression applyExpression = await applyBinder.BindAsync(apply, context);
        // return query.Apply((Expression<Func<T, TResult>>)applyExpression);
        return query;
    }


    protected virtual async ValueTask<IQueryable> CompositeFilterAsync(FilterClause filter, IQueryable query)
    {
        // bind filter to expression
        // var filterBinder = new FilterBinder(new FilterTokenizer(), new QueryNodeBinder());
        // Expression filterExpression = await filterBinder.BindAsync(filter, context);
        // return query.Where((Expression<Func<T, bool>>)filterExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeSearchAsync(SearchClause search, IQueryable query)
    {
        // bind search to expression
        // var searchBinder = new SearchBinder(new SearchTokenizer(), new QueryNodeBinder());
        // Expression searchExpression = await searchBinder.BindAsync(search, context);
        // return query.Where((Expression<Func<T, bool>>)searchExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeOrderByAsync(OrderByClause orderBy, IQueryable query)
    {
        // bind order by to expression
        // var orderByBinder = new OrderByBinder(new OrderByTokenizer(), new QueryNodeBinder());
        // Expression orderByExpression = await orderByBinder.BindAsync(orderBy, context);
        // return query.OrderBy((Expression<Func<T, TResult>>)orderByExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeSkipTokenAsync(string skipToken, IQueryable query)
    {
        // bind skip token to expression
        // var skipTokenBinder = new SkipTokenBinder(new SkipTokenTokenizer(), new QueryNodeBinder());
        // Expression skipTokenExpression = await skipTokenBinder.BindAsync(skipToken, context);
        // return query.SkipToken((Expression<Func<T, TResult>>)skipTokenExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeSelectAndExpandAsync(SelectClause select, ExpandClause expand, IQueryable query)
    {
        // bind select and expand to expression
        // var selectAndExpandBinder = new SelectAndExpandBinder(new SelectAndExpandTokenizer(), new QueryNodeBinder());
        // Expression selectAndExpandExpression = await selectAndExpandBinder.BindAsync(select, expand, context);
        // return query.SelectAndExpand((Expression<Func<T, TResult>>)selectAndExpandExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeSkipAsync(long? skip,IQueryable query)
    {
        // bind select and expand to expression
        // var selectAndExpandBinder = new SelectAndExpandBinder(new SelectAndExpandTokenizer(), new QueryNodeBinder());
        // Expression selectAndExpandExpression = await selectAndExpandBinder.BindAsync(select, expand, context);
        // return query.SelectAndExpand((Expression<Func<T, TResult>>)selectAndExpandExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositeTopAsync(long? top, IQueryable query)
    {
        // bind select and expand to expression
        // var selectAndExpandBinder = new SelectAndExpandBinder(new SelectAndExpandTokenizer(), new QueryNodeBinder());
        // Expression selectAndExpandExpression = await selectAndExpandBinder.BindAsync(select, expand, context);
        // return query.SelectAndExpand((Expression<Func<T, TResult>>)selectAndExpandExpression);
        return query;
    }

    protected virtual async ValueTask<IQueryable> CompositePagingAsync(IQueryable query)
    {
        // bind select and expand to expression
        // var selectAndExpandBinder = new SelectAndExpandBinder(new SelectAndExpandTokenizer(), new QueryNodeBinder());
        // Expression selectAndExpandExpression = await selectAndExpandBinder.BindAsync(select, expand, context);
        // return query.SelectAndExpand((Expression<Func<T, TResult>>)selectAndExpandExpression);
        return query;
    }

    public virtual async ValueTask ApplyToAsync(QueryParsedResult query, QueryApplierContext context)
    {
        // First apply $apply
        if (query.Apply != null)
        {
            // apply $apply to context.Source
        }

        // apply filter
        if (query.Filter != null)
        {
            // bind filter to expression
            // var filterBinder = new FilterBinder(new FilterTokenizer(), new QueryNodeBinder());
            // Expression filterExpression = await filterBinder.BindAsync(query.Filter, new QueryBinderContext());
            // context.Source = context.Source.Where((Expression<Func<T, bool>>)filterExpression);
        }

        // If both $search and $filter are specified in the same request, only those items satisfying both criteria are returned
        // apply $search
        if (query.Search != null)
        {
            // bind search to expression
            // var searchBinder = new SearchBinder(new SearchTokenizer(), new QueryNodeBinder());
            // Expression searchExpression = await searchBinder.BindAsync(query.Search, new QueryBinderContext());
            // context.Source = context.Source.Where((Expression<Func<T, bool>>)searchExpression);
        }

        await ValueTask.CompletedTask;
    }
}
