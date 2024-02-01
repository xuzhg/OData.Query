//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Query node representing an In operator.
/// </summary>
public class InNode : SingleValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InNode" /> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public InNode(SingleValueNode left, CollectionValueNode right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Gets the left operand.
    /// </summary>
    public SingleValueNode Left { get; }

    /// <summary>
    /// Gets the right operand.
    /// </summary>
    public CollectionValueNode Right { get; }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.In;

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType => typeof(bool?);
}
