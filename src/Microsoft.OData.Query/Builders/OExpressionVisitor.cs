//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Builders;

/// <summary>
/// Base visitor class for walking an expression tree bottom up.
/// </summary>
public abstract class OExpressionVisitor
{
    /// <summary>
    /// Main visit method for Expressions
    /// </summary>
    /// <param name="node">The expression to visit</param>
    /// <returns>The visited expression </returns>
    public virtual Expression Visit(Expression node)
    {
        if (node == null)
        {
            return node;
        }

        switch (node.NodeType)
        {
            case ExpressionType.UnaryPlus:
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.ArrayLength:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
                return VisitUnary((UnaryExpression)node);

            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.Power:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.RightShift:
            case ExpressionType.LeftShift:
            case ExpressionType.ExclusiveOr:
                return VisitBinary((BinaryExpression)node);

            case ExpressionType.TypeIs:
                return VisitTypeIs((TypeBinaryExpression)node);
            case ExpressionType.Conditional:
                return VisitConditional((ConditionalExpression)node);
            case ExpressionType.Constant:
                return VisitConstant((ConstantExpression)node);
            case ExpressionType.Parameter:
                return VisitParameter((ParameterExpression)node);
            case ExpressionType.MemberAccess:
                return VisitMemberAccess((MemberExpression)node);
            case ExpressionType.Call:
                return VisitMethodCall((MethodCallExpression)node);
            case ExpressionType.Lambda:
                return VisitLambda((LambdaExpression)node);
            case ExpressionType.New:
                return VisitNew((NewExpression)node);
            case ExpressionType.NewArrayInit:
            case ExpressionType.NewArrayBounds:
                return VisitNewArray((NewArrayExpression)node);
            case ExpressionType.Invoke:
                return VisitInvocation((InvocationExpression)node);
            case ExpressionType.MemberInit:
                return VisitMemberInit((MemberInitExpression)node);
            case ExpressionType.ListInit:
                return VisitListInit((ListInitExpression)node);

            default:
                throw new NotSupportedException(Error.Format(SRResources.ExpressionVisitor_UnsupportedExpression, node.NodeType));
        }
    }

    /// <summary>
    /// visit UnaryExpression
    /// </summary>
    /// <param name="u">The UnaryExpression expression to visit</param>
    /// <returns>The visited UnaryExpression expression </returns>
    protected virtual Expression VisitUnary(UnaryExpression u)
    {
        Expression operand = Visit(u.Operand);
        return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
    }

    /// <summary>
    /// visit BinaryExpression
    /// </summary>
    /// <param name="b">The BinaryExpression expression to visit</param>
    /// <returns>The visited BinaryExpression expression </returns>
    protected virtual Expression VisitBinary(BinaryExpression b)
    {
        Expression left = Visit(b.Left);
        Expression right = Visit(b.Right);
        Expression conversion = Visit(b.Conversion);
        if (left != b.Left || right != b.Right || conversion != b.Conversion)
        {
            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
            {
                return Expression.Coalesce(left, right, conversion as LambdaExpression);
            }

            return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }

        return b;
    }

    /// <summary>
    /// visit LambdaExpression
    /// </summary>
    /// <param name="lambda">The LambdaExpression to visit</param>
    /// <returns>The visited LambdaExpression</returns>
    protected virtual Expression VisitLambda(LambdaExpression lambda)
    {
        Expression body = Visit(lambda.Body);
        if (body != lambda.Body)
        {
            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
        }

        return lambda;
    }

    /// <summary>
    /// TypeBinaryExpression visit method
    /// </summary>
    /// <param name="b">The TypeBinaryExpression expression to visit</param>
    /// <returns>The visited TypeBinaryExpression expression </returns>
    internal virtual Expression VisitTypeIs(TypeBinaryExpression b)
    {
        Expression expr = this.Visit(b.Expression);
        return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
    }

    /// <summary>
    /// ConstantExpression visit method
    /// </summary>
    /// <param name="c">The ConstantExpression expression to visit</param>
    /// <returns>The visited ConstantExpression expression </returns>
    protected virtual Expression VisitConstant(ConstantExpression c)
    {
        return c;
    }

    /// <summary>
    /// ConditionalExpression visit method
    /// </summary>
    /// <param name="c">The ConditionalExpression expression to visit</param>
    /// <returns>The visited ConditionalExpression expression </returns>
    protected virtual Expression VisitConditional(ConditionalExpression c)
    {
        Expression test = this.Visit(c.Test);
        Expression if_true = this.Visit(c.IfTrue);
        Expression if_false = this.Visit(c.IfFalse);
        if (test != c.Test || if_true != c.IfTrue || if_false != c.IfFalse)
        {
            return Expression.Condition(test, if_true, if_false, if_true.Type.IsAssignableFrom(if_false.Type) ? if_true.Type : if_false.Type);
        }

        return c;
    }

    /// <summary>
    /// ParameterExpression visit method
    /// </summary>
    /// <param name="p">The ParameterExpression expression to visit</param>
    /// <returns>The visited ParameterExpression expression </returns>
    protected virtual Expression VisitParameter(ParameterExpression p)
    {
        return p;
    }

    /// <summary>
    /// MemberExpression visit method
    /// </summary>
    /// <param name="m">The MemberExpression expression to visit</param>
    /// <returns>The visited MemberExpression expression </returns>
    protected virtual Expression VisitMemberAccess(MemberExpression m)
    {
        Expression exp = this.Visit(m.Expression);
        return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
    }

    /// <summary>
    /// MethodCallExpression visit method
    /// </summary>
    /// <param name="m">The MethodCallExpression expression to visit</param>
    /// <returns>The visited MethodCallExpression expression </returns>
    protected virtual Expression VisitMethodCall(MethodCallExpression m)
    {
        Expression obj = this.Visit(m.Object);
        IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
        return obj != m.Object || args != m.Arguments ? Expression.Call(obj, m.Method, args) : m;
    }

    /// <summary>
    /// Expression list visit method
    /// </summary>
    /// <param name="original">The expression list to visit</param>
    /// <returns>The visited expression list</returns>
    protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
        List<Expression> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            Expression p = this.Visit(original[i]);
            if (list != null)
            {
                list.Add(p);
            }
            else if (p != original[i])
            {
                list = new List<Expression>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }

                list.Add(p);
            }
        }

        if (list != null)
        {
            return new ReadOnlyCollection<Expression>(list);
        }

        return original;
    }

    /// <summary>
    /// MemberAssignment visit method
    /// </summary>
    /// <param name="assignment">The MemberAssignment to visit</param>
    /// <returns>The visited MemberAssignment</returns>
    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
    {
        Expression e = this.Visit(assignment.Expression);
        return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
    }

    /// <summary>
    /// visit MemberBinding
    /// </summary>
    /// <param name="binding">The MemberBinding expression to visit</param>
    /// <returns>The visited MemberBinding expression </returns>
    protected virtual MemberBinding VisitBinding(MemberBinding binding)
    {
        switch (binding.BindingType)
        {
            case MemberBindingType.Assignment:
                return this.VisitMemberAssignment((MemberAssignment)binding);
            case MemberBindingType.MemberBinding:
                return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
            case MemberBindingType.ListBinding:
                return this.VisitMemberListBinding((MemberListBinding)binding);
            default:
                throw new NotSupportedException(Error.Format(SRResources.ExpressionVisitor_UnsupportedExpression, binding.BindingType));
        }
    }

    /// <summary>
    /// MemberMemberBinding visit method
    /// </summary>
    /// <param name="binding">The MemberMemberBinding to visit</param>
    /// <returns>The visited MemberMemberBinding</returns>
    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
    {
        IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
        return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
    }

    /// <summary>
    /// MemberListBinding visit method
    /// </summary>
    /// <param name="binding">The MemberListBinding to visit</param>
    /// <returns>The visited MemberListBinding</returns>
    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
    {
        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
        return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
    }

    /// <summary>
    /// Binding List visit method
    /// </summary>
    /// <param name="original">The Binding list to visit</param>
    /// <returns>The visited Binding list</returns>
    protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
    {
        List<MemberBinding> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            MemberBinding b = VisitBinding(original[i]);
            if (list != null)
            {
                list.Add(b);
            }
            else if (b != original[i])
            {
                list = new List<MemberBinding>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }

                list.Add(b);
            }
        }

        if (list != null)
        {
            return list;
        }

        return original;
    }

    /// <summary>
    /// visit NewExpression
    /// </summary>
    /// <param name="nex">The NewExpression to visit</param>
    /// <returns>The visited NewExpression</returns>
    protected virtual NewExpression VisitNew(NewExpression nex)
    {
        IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
        if (args != nex.Arguments)
        {
            return nex.Members != null ?
                Expression.New(nex.Constructor, args, nex.Members) :
                Expression.New(nex.Constructor, args);
        }

        return nex;
    }

    /// <summary>
    /// visit MemberInitExpression
    /// </summary>
    /// <param name="init">The MemberInitExpression to visit</param>
    /// <returns>The visited MemberInitExpression</returns>
    protected virtual Expression VisitMemberInit(MemberInitExpression init)
    {
        NewExpression n = VisitNew(init.NewExpression);
        IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
        return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
    }

    /// <summary>
    /// visit ListInitExpression
    /// </summary>
    /// <param name="init">The ListInitExpression to visit</param>
    /// <returns>The visited ListInitExpression</returns>
    protected virtual Expression VisitListInit(ListInitExpression init)
    {
        NewExpression n = VisitNew(init.NewExpression);
        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
        return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
    }

    /// <summary>
    /// visit NewArrayExpression
    /// </summary>
    /// <param name="na">The NewArrayExpression to visit</param>
    /// <returns>The visited NewArrayExpression</returns>
    protected virtual Expression VisitNewArray(NewArrayExpression na)
    {
        IEnumerable<Expression> expressions = this.VisitExpressionList(na.Expressions);
        if (expressions != na.Expressions)
        {
            return na.NodeType == ExpressionType.NewArrayInit ?
                Expression.NewArrayInit(na.Type.GetElementType(), expressions) :
                Expression.NewArrayBounds(na.Type.GetElementType(), expressions);
        }

        return na;
    }

    /// <summary>
    /// visit ElementInit
    /// </summary>
    /// <param name="initializer">The ElementInit expression to visit</param>
    /// <returns>The visited ElementInit expression </returns>
    internal virtual ElementInit VisitElementInitializer(ElementInit initializer)
    {
        ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
        return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
    }

    /// <summary>
    /// visit ElementInit expression list
    /// </summary>
    /// <param name="original">The ElementInit expression list  to visit</param>
    /// <returns>The visited ElementInit expression list </returns>
    protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
    {
        List<ElementInit> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            ElementInit init = VisitElementInitializer(original[i]);
            if (list != null)
            {
                list.Add(init);
            }
            else if (init != original[i])
            {
                list = new List<ElementInit>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }

                list.Add(init);
            }
        }

        if (list != null)
        {
            return list;
        }

        return original;
    }

    /// <summary>
    /// visit InvocationExpression
    /// </summary>
    /// <param name="iv">The InvocationExpression to visit</param>
    /// <returns>The visited InvocationExpression</returns>
    protected virtual Expression VisitInvocation(InvocationExpression iv)
    {
        IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
        Expression expr = this.Visit(iv.Expression);
        return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
    }
}


