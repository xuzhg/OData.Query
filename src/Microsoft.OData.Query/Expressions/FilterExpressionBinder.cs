//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Expressions.Interfaces;

namespace Microsoft.OData.Query.Expressions
{
    public class FilterExpressionBinder : IFilterExpressionBinder
    {
        public virtual Expression BindFilter(FilterClause filter)
        {
            return null;
        }
    }
}
