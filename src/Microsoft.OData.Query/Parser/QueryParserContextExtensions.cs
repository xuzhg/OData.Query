//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

internal static class QueryParserContextExtensions
{
    public static IComputeParser GetOrCreateComputeParser(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IComputeParser>() ?? new ComputeParser();

    public static IComputeTokenizer GetOrCreateComputeTokenizer(this QueryParserContext context)
        => context?.ServiceProvider?.GetService<IComputeTokenizer>() ?? new ComputeTokenizer();

}