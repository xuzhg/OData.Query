//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;

namespace Microsoft.OData.Query.Binders;

public static class QueryCompositerContextExtensions
{
    public static IQueryParser GetParser(this QueryCompositerContext context)
    {
        // return the parser based on the context
        return context.Parser ?? new QueryParser();
    }
}

public interface IQueryComposerFactory
{
    ValueTask<IFilterComposer> CreateFilterComposerAsync(ReadOnlyMemory<char> filter, QueryCompositerContext context);
}

public class QueryComposerFactory : IQueryComposerFactory
{
    public virtual async ValueTask<IFilterComposer> CreateFilterComposerAsync(ReadOnlyMemory<char> filter, QueryCompositerContext context)
    {
        return new FilterComposer(filter, context);
    }
}
