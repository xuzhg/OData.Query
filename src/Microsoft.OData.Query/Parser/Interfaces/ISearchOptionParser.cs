﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;

namespace Microsoft.OData.Query.Parser;

public interface ISearchOptionParser
{
   // SearchToken ParseSearch(string search, QueryParserContext context);
    ValueTask<SearchClause> ParseAsync(string search, QueryParserContext context);
}
