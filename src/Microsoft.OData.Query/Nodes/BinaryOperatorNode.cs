//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Query node representing a binary operator.
/// </summary>
public class BinaryOperatorNode : SingleValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryOperatorNode" /> class.
    /// </summary>
    /// <param name="operatorKind">The binary operator kind.</param>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public BinaryOperatorNode(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right)
        : this(operatorKind, left, right, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryOperatorNode" /> class.
    /// </summary>
    /// <param name="operatorKind">The binary operator kind.</param>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="resultType">The result type of node.</param>
    public BinaryOperatorNode(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right, Type resultType)
    {
        OperatorKind = operatorKind;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));

        // Set the NodeType if explicitly given, otherwise based on the Operands.
        NodeType = resultType != null ?
            resultType :
            BinaryOperatorNodeHelpers.GetBinaryOperatorResultType(left, right, operatorKind);
    }

    /// <summary>
    /// Gets the operator kind.
    /// </summary>
    public BinaryOperatorKind OperatorKind { get; }

    /// <summary>
    /// Gets the left operand.
    /// </summary>
    public SingleValueNode Left { get; }

    /// <summary>
    /// Gets the right operand.
    /// </summary>
    public SingleValueNode Right { get; }

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType { get; }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.BinaryOperator;
}
