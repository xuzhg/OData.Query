//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Parser;
using System.Linq.Expressions;

namespace Microsoft.OData.Query;

/// <summary>
/// Parser for query options.
/// </summary>
public interface IODataQueryOptionParser
{
    /// <summary>
    /// Query string must be in escaped and delimited format with a leading '?' character for a non-empty query.
    /// So, as example, a query string looks like:
    /// "?$filter=Name eq 'Sam'&$select=Id,Name&..."
    /// For each key/value, they should be fully encoded.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask<ODataQueryOption> ParseAsync(ReadOnlyMemory<char> query, QueryParserContext context);

    //ODataQueryOption Parse(ReadOnlySpan<char> query, QueryParserContext context);
}



public interface IODataQueryOptionApplier
{
    ValueTask<IQueryable> ApplyAsync(IQueryable source, string query, QueryParserContext context);
}

public interface IFilterQueryApplier
{
    ValueTask<IQueryable> ApplyAsync(IQueryable source, string filter, QueryParserContext context);
}

public interface IOrderByQueryApplier
{
    ValueTask<IQueryable> ApplyAsync(IQueryable source, string orderBy, QueryParserContext context);
}

public interface IFilterQueryBinder
{
    ValueTask<Expression> BindAsync(FilterClause filterClause, Parser.QueryParserContext context);
}