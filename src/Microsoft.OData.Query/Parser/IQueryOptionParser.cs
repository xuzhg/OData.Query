//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query.Parser
{
    public interface IQueryOptionParser
    {
        FilterClause ParseFilter(string filter, ParserContext context);
    }
}
