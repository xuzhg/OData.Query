//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Parser;
using System;

namespace Microsoft.OData.Query;

public class ODataQueryOptions<T> : ODataQueryOptions
{
    public ODataQueryOptions(string query)
        : base(typeof(T), query, null)
    {
    }
}

public class ODataQueryOptions : IODataQueryOptionApplier
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ODataQueryOptionParser" /> class.
    /// </summary>
    /// <param name="serviceProvider">The required tokenizer.</param>
    public ODataQueryOptions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ODataQueryOptions(Type elementType, string query, IServiceProvider serviceProvider)
    {
        
    }

    public async ValueTask<IQueryable> ApplyAsync(IQueryable source, string query, QueryParserContext context)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        IODataQueryOptionParser parser = _serviceProvider.GetService<IODataQueryOptionParser>();
        ODataQueryOption queryOption = await parser.ParseAsync(query, context);

        if (queryOption.Apply != null)
        {
            source = await ApplyApply(source, context);
        }

        if (queryOption.Filter != null)
        {
            source = await ApplyFilter(source, context);
        }

        if (queryOption.Count != null && queryOption.Count.Value)
        {

        }

        // Then Orderby

        // Then SkipToken

        // Then DeltaToken?

        // Then Select & Expand

        // Then $skip

        // Then $top

        return source;
    }

    protected virtual ValueTask<IQueryable> ApplyApply(IQueryable source, QueryParserContext context)
    {
        throw new NotImplementedException();
    }

    protected virtual ValueTask<IQueryable> ApplyFilter(IQueryable source, QueryParserContext context)
    {
        throw new NotImplementedException();
    }
}

public class OrderByQueryOption
{
    public OrderByQueryOption(Type elementType, string rawOrderBy)
    {
         
    }
}
