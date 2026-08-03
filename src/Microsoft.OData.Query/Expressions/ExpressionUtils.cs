//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.OData.Query.Expressions;

internal class ExpressionUtils
{
    private static MethodInfo _whereMethod = GenericMethodOf(_ => Queryable.Where<int>(default(IQueryable<int>), default(Expression<Func<int, bool>>)));

    private static MethodInfo _queryableSelectMethod = GenericMethodOf(_ => Queryable.Select<int, int>(default(IQueryable<int>), i => i));

    private static MethodInfo _skipMethod = GenericMethodOf(_ => Queryable.Skip<int>(default(IQueryable<int>), default(int)));

    private static MethodInfo _takeMethod = GenericMethodOf(_ => Queryable.Take<int>(default(IQueryable<int>), default(int)));

    private static MethodInfo _orderByMethod = GenericMethodOf(_ => Queryable.OrderBy<int, int>(default(IQueryable<int>), i => i));

    private static MethodInfo _orderByDescendingMethod = GenericMethodOf(_ => Queryable.OrderByDescending<int, int>(default(IQueryable<int>), i => i));

    private static MethodInfo _thenByMethod = GenericMethodOf(_ => Queryable.ThenBy<int, int>(default(IOrderedQueryable<int>), i => i));

    private static MethodInfo _thenByDescendingMethod = GenericMethodOf(_ => Queryable.ThenByDescending<int, int>(default(IOrderedQueryable<int>), i => i));

    public static readonly Expression NullConstant = Expression.Constant(null);

    public static readonly Expression FalseConstant = Expression.Constant(false, typeof(bool));

    public static readonly Expression TrueConstant = Expression.Constant(true, typeof(bool));

    public static MethodInfo QueryableWhereGeneric => _whereMethod;
    public static MethodInfo QueryableSelectGeneric => _queryableSelectMethod;

    public static IQueryable Where(IQueryable query, Expression where, Type type)
    {
        MethodInfo whereMethod = QueryableWhereGeneric.MakeGenericMethod(type);
        return (IQueryable)whereMethod.Invoke(null, new object[] { query, where });
    }

    public static IQueryable Select(IQueryable query, LambdaExpression expression, Type type)
    {
        MethodInfo selectMethod = QueryableSelectGeneric.MakeGenericMethod(type, expression.Body.Type);
        return (IQueryable)selectMethod.Invoke(null, new object[] { query, expression });
    }

    public static IQueryable Skip(IQueryable query, long count, Type type)
    {
        MethodInfo skipMethod = _skipMethod.MakeGenericMethod(type);
        return (IQueryable)skipMethod.Invoke(null, new object[] { query, checked((int)count) });
    }

    public static IQueryable Take(IQueryable query, long count, Type type)
    {
        MethodInfo takeMethod = _takeMethod.MakeGenericMethod(type);
        return (IQueryable)takeMethod.Invoke(null, new object[] { query, checked((int)count) });
    }

    public static IQueryable OrderBy(IQueryable query, LambdaExpression expression, bool descending, bool thenBy)
    {
        MethodInfo method = (thenBy, descending) switch
        {
            (false, false) => _orderByMethod,
            (false, true) => _orderByDescendingMethod,
            (true, false) => _thenByMethod,
            (true, true) => _thenByDescendingMethod
        };

        method = method.MakeGenericMethod(query.ElementType, expression.Body.Type);
        return (IQueryable)method.Invoke(null, new object[] { query, expression });
    }

    private static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
    {
        return GenericMethodOf(expression as Expression);
    }

    private static MethodInfo GenericMethodOf(Expression expression)
    {
        LambdaExpression lambdaExpression = expression as LambdaExpression;

        Contract.Assert(expression.NodeType == ExpressionType.Lambda);
        Contract.Assert(lambdaExpression != null);
        Contract.Assert(lambdaExpression.Body.NodeType == ExpressionType.Call);

        return (lambdaExpression.Body as MethodCallExpression).Method.GetGenericMethodDefinition();
    }
}