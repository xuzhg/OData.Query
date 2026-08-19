//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Binders;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Expressions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests;

public class QueryCompositerTests
{
    [Fact]
    public async Task CompositeAsyncHonorsAllIgnoredQueryOptions()
    {
        var compositer = new TrackingQueryCompositer("$top=1".AsMemory());
        IQueryable source = Array.Empty<TestEntity>().AsQueryable();
        var settings = new QueryCompositerSettings
        {
            IgnoredQueryOptions = AllowedQueryOptions.All
        };

        IQueryable result = await compositer.CompositeAsync(source, settings);

        Assert.Same(source, result);
        Assert.Equal(0, compositer.ApplyCalls);
        Assert.Equal(0, compositer.FilterCalls);
        Assert.Equal(0, compositer.SearchCalls);
        Assert.Equal(0, compositer.OrderByCalls);
        Assert.Equal(0, compositer.SkipTokenCalls);
        Assert.Equal(0, compositer.SelectAndExpandCalls);
        Assert.Equal(0, compositer.SkipCalls);
        Assert.Equal(0, compositer.TopCalls);
        Assert.Equal(1, compositer.PagingCalls);
    }

    [Fact]
    public async Task CompositeAsyncDispatchesNonIgnoredQueryOptions()
    {
        var compositer = new TrackingQueryCompositer("$top=1".AsMemory());
        IQueryable source = Array.Empty<TestEntity>().AsQueryable();

        await compositer.CompositeAsync(source, new QueryCompositerSettings());

        Assert.Equal(1, compositer.ApplyCalls);
        Assert.Equal(1, compositer.FilterCalls);
        Assert.Equal(1, compositer.SearchCalls);
        Assert.Equal(1, compositer.OrderByCalls);
        Assert.Equal(1, compositer.SkipTokenCalls);
        Assert.Equal(0, compositer.SelectAndExpandCalls);
        Assert.Equal(1, compositer.SkipCalls);
        Assert.Equal(1, compositer.TopCalls);
        Assert.Equal(1, compositer.PagingCalls);
    }

    private sealed class TrackingQueryCompositer : QueryCompositer
    {
        public TrackingQueryCompositer(ReadOnlyMemory<char> query)
            : base(query, new QueryCompositerContext())
        {
        }

        public int ApplyCalls { get; private set; }
        public int FilterCalls { get; private set; }
        public int SearchCalls { get; private set; }
        public int OrderByCalls { get; private set; }
        public int SkipTokenCalls { get; private set; }
        public int SelectAndExpandCalls { get; private set; }
        public int SkipCalls { get; private set; }
        public int TopCalls { get; private set; }
        public int PagingCalls { get; private set; }

        protected override ValueTask<IQueryable> CompositeApplyAsync(ApplyClause apply, IQueryable query)
        {
            ApplyCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeFilterAsync(FilterClause filter, IQueryable query)
        {
            FilterCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeSearchAsync(SearchClause search, IQueryable query)
        {
            SearchCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeOrderByAsync(OrderByClause orderBy, IQueryable query)
        {
            OrderByCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeSkipTokenAsync(string skipToken, IQueryable query)
        {
            SkipTokenCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeSelectAndExpandAsync(
            SelectClause select,
            ExpandClause expand,
            IQueryable query)
        {
            SelectAndExpandCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeSkipAsync(long? skip, IQueryable query)
        {
            SkipCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositeTopAsync(long? top, IQueryable query)
        {
            TopCalls++;
            return ValueTask.FromResult(query);
        }

        protected override ValueTask<IQueryable> CompositePagingAsync(
            IQueryable query,
            QueryCompositerSettings settings)
        {
            PagingCalls++;
            return ValueTask.FromResult(query);
        }
    }

    private sealed class TestEntity
    {
    }
}
