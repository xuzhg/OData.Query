//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Expressions;

public class OQueryExpressionBinder : IOQueryExpressionBinder
{
    ValueTask<Expression> IOQueryExpressionBinder.BindQuery(QueryNode queryNode)
    {
        throw new NotImplementedException();
    }
}
