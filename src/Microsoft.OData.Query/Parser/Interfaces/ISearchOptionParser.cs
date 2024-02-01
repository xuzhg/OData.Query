//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public class SearchParserContext : QueryOptionParserContext
{

}

public interface ISearchOptionParser
{
    SearchToken ParseSearch(string search, QueryOptionParserContext context);
}
