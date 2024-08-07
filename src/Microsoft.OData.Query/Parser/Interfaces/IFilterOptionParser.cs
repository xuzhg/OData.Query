﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query.Parser;

public interface IFilterOptionParser
{
    /// <summary>
    /// Parses the $filter expression like "Name eq 'Sam'" to a search tree.
    /// </summary>
    /// <param name="filter">The $filter expression string to parse.</param>
    /// <returns>The filter token.</returns>
    ValueTask<FilterClause> ParseAsync(string filter, QueryParserContext context);

}
