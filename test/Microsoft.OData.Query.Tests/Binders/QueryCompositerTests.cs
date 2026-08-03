//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Query.Binders;
using Microsoft.OData.Query.Tests.Models;
using Xunit;

namespace Microsoft.OData.Query.Tests.Binders;

public class QueryCompositerTests
{
    private static readonly IQueryable<Customer> Customers = new[]
    {
        new Customer { Id = 1, Name = "Sam" },
        new Customer { Id = 2, Name = "John" },
        new Customer { Id = 3, Name = "Alice" },
        new Customer { Id = 4, Name = "Sam" }
    }.AsQueryable();

    [Fact]
    public async Task CompositeAsync_AppliesFilter()
    {
        var compositer = new QueryCompositer(
            "$filter=Name eq 'Sam'".AsMemory(),
            new QueryCompositerContext());

        IQueryable result = await compositer.CompositeAsync(Customers, new QueryCompositerSettings());

        Assert.Equal(new[] { 1, 4 }, result.Cast<Customer>().Select(c => c.Id));
    }

    [Fact]
    public async Task CompositeAsync_AppliesComparisonFilter()
    {
        var compositer = new QueryCompositer(
            "$filter=Id gt 2".AsMemory(),
            new QueryCompositerContext());

        IQueryable result = await compositer.CompositeAsync(Customers, new QueryCompositerSettings());

        Assert.Equal(new[] { 3, 4 }, result.Cast<Customer>().Select(c => c.Id));
    }

    [Fact]
    public async Task CompositeAsync_AppliesLogicalFilter()
    {
        var compositer = new QueryCompositer(
            "$filter=Name eq 'Sam' and Id gt 1".AsMemory(),
            new QueryCompositerContext());

        IQueryable result = await compositer.CompositeAsync(Customers, new QueryCompositerSettings());

        Customer customer = Assert.Single(result.Cast<Customer>());
        Assert.Equal(4, customer.Id);
    }

    [Fact]
    public async Task CompositeAsync_AppliesSkipBeforeTop()
    {
        var compositer = new QueryCompositer(
            "$skip=1&$top=2".AsMemory(),
            new QueryCompositerContext());

        IQueryable result = await compositer.CompositeAsync(Customers, new QueryCompositerSettings());

        Assert.Equal(new[] { 2, 3 }, result.Cast<Customer>().Select(c => c.Id));
    }

    [Fact]
    public async Task CompositeAsync_HonorsIgnoredQueryOptions()
    {
        var compositer = new QueryCompositer(
            "$filter=Id gt 2&$skip=1&$top=1".AsMemory(),
            new QueryCompositerContext());
        var settings = new QueryCompositerSettings
        {
            IgnoredQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.Skip | AllowedQueryOptions.Top
        };

        IQueryable result = await compositer.CompositeAsync(Customers, settings);

        Assert.Equal(new[] { 1, 2, 3, 4 }, result.Cast<Customer>().Select(c => c.Id));
    }

    [Fact]
    public async Task CompositeAsync_AppliesPageSize()
    {
        var compositer = new QueryCompositer(
            "$top=3".AsMemory(),
            new QueryCompositerContext());
        var settings = new QueryCompositerSettings { PageSize = 2 };

        IQueryable result = await compositer.CompositeAsync(Customers, settings);

        Assert.Equal(new[] { 1, 2 }, result.Cast<Customer>().Select(c => c.Id));
    }

    [Fact]
    public async Task CompositeAsync_AppliesOrderByAndThenBy()
    {
        var compositer = new QueryCompositer(
            "$orderby=Name desc,Id".AsMemory(),
            new QueryCompositerContext());

        IQueryable result = await compositer.CompositeAsync(Customers, new QueryCompositerSettings());

        Assert.Equal(new[] { 1, 4, 2, 3 }, result.Cast<Customer>().Select(c => c.Id));
    }
}
