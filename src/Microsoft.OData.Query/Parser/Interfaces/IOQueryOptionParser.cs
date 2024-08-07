﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public interface IOQueryOptionParser
{
 //   FilterClause ParseFilter(string filter, QueryOptionParserContext context);

    IQueryToken ParseQuery(string query, QueryParserContext context);

 //   QueryToken ParseQuery(QueryOptionParserContext context);
}
