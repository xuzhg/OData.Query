//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.OData.Query.Builders;

public sealed class ODataQueryProvider : IQueryProvider
{
    public IQueryable CreateQuery(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        Type elementType = expression.Type.GetElementTypeOrSelf();
        Type targetType = typeof(ODataQueryable<>).MakeGenericType(elementType);

        object[] args = new object[] { expression };
        ConstructorInfo ci = targetType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, new Type[] { typeof(Expression) });
        return (IQueryable)ci.Invoke(args);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        return new ODataQueryable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }
}


