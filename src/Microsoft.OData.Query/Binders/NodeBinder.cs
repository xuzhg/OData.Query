//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Binders;

/// <summary>
/// Default implementation for Query Node binder.
/// </summary>
public abstract class NodeBinder : IQueryNodeBinder
{
    /// <summary>
    /// Binds a <see cref="QueryNode"/> to create a LINQ <see cref="Expression"/> that represents the semantics of the <see cref="QueryNode"/>.
    /// </summary>
    /// <param name="node">The query node to bind.</param>
    /// <param name="context">The query binder context.</param>
    /// <returns>The LINQ <see cref="Expression"/> created.</returns>
    public virtual Expression Bind(QueryNode node, QueryBinderContext context)
    {
        if (node is CollectionValueNode collectionNode)
        {
            return BindCollectionNode(collectionNode, context);
        }
        else if (node is SingleValueNode singleValueNode)
        {
            return BindSingleValueNode(singleValueNode, context);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Binds a <see cref="CollectionNode"/> to create a LINQ <see cref="Expression"/> that represents the semantics of the <see cref="CollectionNode"/>.
    /// </summary>
    /// <param name="node">The query node to bind.</param>
    /// <param name="context">The query binder context.</param>
    /// <returns>The LINQ <see cref="Expression"/> created.</returns>
    public virtual Expression BindCollectionNode(CollectionValueNode node, QueryBinderContext context)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Binds a <see cref="SingleValueNode"/> to create a LINQ <see cref="Expression"/> that represents the semantics of the <see cref="SingleValueNode"/>.
    /// </summary>
    /// <param name="node">The node to bind.</param>
    /// <param name="context">The query binder context.</param>
    /// <returns>The LINQ <see cref="Expression"/> created.</returns>
    public virtual Expression BindSingleValueNode(SingleValueNode node, QueryBinderContext context)
    {
        switch (node)
        {
            case BinaryOperatorNode binNode:
                return BindBinaryOperatorNode(binNode, context);

            case SingleValuePropertyAccessNode san:
                return BindPropertyAccessQueryNode(san, context);

            case ConstantNode constantNode:
                return BindConstantNode(constantNode, context);

            case RangeVariableReferenceNode rangeValue:
                return BindRangeVariable(rangeValue, context);

            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Binds a <see cref="RangeVariable"/> to create a LINQ <see cref="Expression"/> that
    /// represents the semantics of the <see cref="RangeVariable"/>.
    /// </summary>
    /// <param name="rangeVariable">The range variable to bind.</param>
    /// <param name="context">The query binder context.</param>
    /// <returns>The LINQ <see cref="Expression"/> created.</returns>
    public virtual Expression BindRangeVariable(RangeVariableReferenceNode rangeVariable, QueryBinderContext context)
    {
        if (rangeVariable.Name == "$it")
        {
            return context.CurrentParameter;
        }

        throw new NotSupportedException();

    }

    public virtual Expression BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode, QueryBinderContext context)
    {
        Expression left = Bind(binaryOperatorNode.Left, context);

        Expression right = Bind(binaryOperatorNode.Right, context);

        // just for sample;
        return Expression.MakeBinary(ExpressionType.Equal, left, right);
    }

    public virtual Expression BindPropertyAccessQueryNode(SingleValuePropertyAccessNode propertyAccessNode, QueryBinderContext context)
    {
        Expression source = Bind(propertyAccessNode.Source, context);

        return Expression.Property(source, propertyAccessNode.Property);
    }

    public virtual Expression BindConstantNode(ConstantNode constantNode, QueryBinderContext context)
    {
        return Expression.Constant(constantNode.Value);
    }
}
