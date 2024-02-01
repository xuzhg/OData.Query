//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing a constant value, can either be primitive, complex, entity, or collection value.
/// </summary>
public class ConstantNode<T> : SingleValueNode where T: struct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantNode" /> class.
    /// </summary>
    /// <param name="constantValue">This node's primitive value.</param>
    public ConstantNode(T constantValue)
        : this(constantValue, constantValue.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantNode" /> class.
    /// </summary>
    /// <param name="source">The node to ConstantNode.</param>
    /// <param name="literalText">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
    public ConstantNode(T constantValue, string literalText)
    {
        Value = constantValue;
        LiteralText = literalText;
    }

    /// <summary>
    /// Gets the primitive constant value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules.
    /// May be null if the text was not provided at construction time.
    /// </summary>
    public string LiteralText { get; }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.Constant;

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType => typeof(T);
}