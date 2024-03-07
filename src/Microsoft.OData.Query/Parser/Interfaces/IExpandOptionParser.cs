//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser;

public interface IExpandOptionParser
{
    ValueTask<ExpandClause> ParseAsync(string expand, QueryParserContext context);
}
