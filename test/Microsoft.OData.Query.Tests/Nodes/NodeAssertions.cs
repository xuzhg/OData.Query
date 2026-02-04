//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------


using Microsoft.OData.Query.Nodes;
using System;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Query.Tests.Nodes;

internal static class NodeAssertions
{
    public static ConstantNode ShouldBeConstantQueryNode<TValue>(this QueryNode node, TValue expectedValue)
    {
        Assert.NotNull(node);
        var constantNode = Assert.IsType<ConstantNode>(node);
        if (expectedValue == null)
        {
            Assert.Null(constantNode.Value);
        }
        else
        {
            Type nodeValueType = constantNode.Value.GetType();
            Assert.True(typeof(TValue).IsAssignableFrom(nodeValueType));
            Assert.Equal(expectedValue, constantNode.Value);
        }

        return constantNode;
    }

    public static BinaryOperatorNode ShouldBeBinaryOperatorNode(this QueryNode node, BinaryOperatorKind expectedOperatorKind)
    {
        Assert.NotNull(node);
        var orderByQueryNode = Assert.IsType<BinaryOperatorNode>(node);
        Assert.Equal(expectedOperatorKind, orderByQueryNode.OperatorKind);
        return orderByQueryNode;
    }

    public static SingleValuePropertyAccessNode ShouldBeSingleValuePropertyAccessQueryNode(this QueryNode node, PropertyInfo expectedProperty)
    {
        Assert.NotNull(node);
        var propertyAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(node);
        Assert.Same(expectedProperty, propertyAccessNode.Property);
        return propertyAccessNode;
    }
}
