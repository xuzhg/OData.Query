﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query.Expressions.Interfaces
{
    public interface IFilterExpressionBinder
    {
        Expression BindFilter(FilterClause filter);
    }
}
