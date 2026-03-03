//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.OData.Query.Expressions;
using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query.Binders;

public class FilterComposer : IFilterComposer
{
    private QueryParsedResult _parsedResult = null;
    public FilterComposer(ReadOnlyMemory<char> filter, QueryCompositerContext context)
    {
        Filter = filter;
        Context = context;
    }
    public ReadOnlyMemory<char> Filter { get; }
    public QueryCompositerContext Context { get; }
    public virtual async ValueTask<IQueryable> CompositeAsync(IQueryable source, QueryCompositerSettings settings)
    {
        if (_parsedResult == null)
        {
            _parsedResult = await Context.GetParser().ParseAsync(Filter, new QueryParserContext(source.ElementType));
        }
        // Check the parsedResult
        if (_parsedResult == null || _parsedResult.Filter == null)
        {
            throw new InvalidOperationException("Failed to parse the filter query option.");
        }
        // bind filter to expression
        var filterBinder = new FilterBinder();
        Expression filterExpression = await filterBinder.BindAsync(_parsedResult.Filter, new QueryBinderContext(source.ElementType));
        source = ExpressionUtils.Where(source, filterExpression, source.ElementType);
        return source;
    }
}
