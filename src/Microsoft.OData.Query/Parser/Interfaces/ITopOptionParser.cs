//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

public interface ITopOptionParser
{
    ValueTask<long> ParseAsync(string top, QueryParserContext context);
}