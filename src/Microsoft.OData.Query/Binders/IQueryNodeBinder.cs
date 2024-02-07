//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Binders;

public interface IQueryNodeBinder
{
    Expression Bind(QueryNode node, BinderContext context);
}

public interface IOQueryOptionBinder
{
    IQueryable ApplyTo(IQueryable query, OQueryBinderContext context);

    object ApplyTo(object entity, OQueryBinderContext context);
}