//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

internal static class QueryParserContextExtensions
{
    public static IUriResolver GetOrCreateUriResolver(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IUriResolver>() ?? new DefaultUriResolver();

    public static IApplyParser GetOrCreateApplyParser(this QueryParserContext context)
    => context?.ServiceProvider?.GetService<IApplyParser>() ?? new ApplyParser();

    public static IApplyTokenizer GetOrCreateApplyTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IApplyTokenizer>() ?? new ApplyTokenizer();
    public static IComputeParser GetOrCreateComputeParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IComputeParser>() ?? new ComputeParser();

    public static IComputeTokenizer GetOrCreateComputeTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IComputeTokenizer>() ?? new ComputeTokenizer();

    public static IFilterParser GetOrCreateFilterParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IFilterParser>() ?? new FilterParser();

    public static IFilterTokenizer GetOrCreateFilterTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IFilterTokenizer>() ?? new FilterTokenizer();

    public static IOrderByParser GetorCreateOrderByParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IOrderByParser>() ?? new OrderByParser();

    public static ICountParser GetOrCreateCountParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ICountParser>() ?? new CountParser();

    public static ITopParser GetOrCreateTopParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ITopParser>() ?? new TopParser();

    public static ISkipParser GetOrCreateSkipParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ISkipParser>() ?? new SkipParser();

    public static IIndexParser GetOrCreateIndexParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IIndexParser>() ?? new IndexParser();

    public static IOrderByTokenizer GetOrCreateOrderByTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IOrderByTokenizer>() ?? new OrderByTokenizer();

    public static IExpandParser GetOrCreateExpandParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IExpandParser>() ?? new ExpandParser();

    public static IExpandTokenizer GetOrCreateExpandTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IExpandTokenizer>() ?? new ExpandTokenizer();

    public static ISelectParser GetOrCreateSelectParser(this QueryParserContext context)
    => context?.ServiceProvider?.GetService<ISelectParser>() ?? new SelectParser();

    public static ISelectTokenizer GetOrCreateSelectTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ISelectTokenizer>() ?? new SelectTokenizer();

    public static ISearchParser GetOrCreateSearchParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ISearchParser>() ?? new SearchParser();

    public static ISearchTokenizer GetOrCreateSearchTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<ISearchTokenizer>() ?? new SearchTokenizer();
}