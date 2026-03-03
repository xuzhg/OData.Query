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

    public static readonly Expression NullConstant = Expression.Constant(null);

    public static readonly Expression FalseConstant = Expression.Constant(false, typeof(bool));

    public static readonly Expression TrueConstant = Expression.Constant(true, typeof(bool));

    public static MethodInfo QueryableWhereGeneric => _whereMethod;
    public static MethodInfo QueryableSelectGeneric => _queryableSelectMethod;

    public static IQueryable Where(IQueryable query, Expression where, Type type)
    {
        MethodInfo whereMethod = QueryableWhereGeneric.MakeGenericMethod(type);
        return whereMethod.Invoke(null, new object[] { query, where }) as IQueryable;
    }

    public static IQueryable Select(IQueryable query, LambdaExpression expression, Type type)
    {
        MethodInfo selectMethod = QueryableSelectGeneric.MakeGenericMethod(type, expression.Body.Type);
        return selectMethod.Invoke(null, new object[] { query, expression }) as IQueryable;
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