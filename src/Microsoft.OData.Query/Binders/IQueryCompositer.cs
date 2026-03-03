//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query.Binders;

public interface IQueryCompositer
{
    // ValueTask ApplyToAsync(ReadOnlyMemory<char> query, QueryApplierContext context);

    ReadOnlyMemory<char> Query { get; }

    QueryCompositerContext Context { get; }

    ValueTask<IQueryable> CompositeAsync(IQueryable source, QueryCompositerSettings settings);

    ValueTask ApplyToAsync(QueryParsedResult query, QueryApplierContext context);
}

public interface IFilterComposer
{
    ReadOnlyMemory<char> Filter { get; }

    QueryCompositerContext Context { get; }

    ValueTask<IQueryable> CompositeAsync(IQueryable source, QueryCompositerSettings settings);
}

public interface IOrderByComposer
{
    ReadOnlyMemory<char> OrderBy { get; }

    QueryCompositerContext Context { get; }

    ValueTask<IQueryable> CompositeAsync(IQueryable source, QueryCompositerSettings settings);
}