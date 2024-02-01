//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------


using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

public class ConstantNode : SingleValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantNode" /> class.
    /// </summary>
    /// <param name="constantValue"></param>
    /// <param name="literalText"></param>
    public ConstantNode(object constantValue, string literalText)
    {
        Value = constantValue;
        LiteralText = literalText;
    }

    /// <summary>
    /// Gets the primitive constant value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Get or Set the literal text for this node's value, formatted according to the OData URI literal formatting rules.
    /// May be null if the text was not provided at construction time.
    /// </summary>
    public string LiteralText { get; }

    /// <summary>
    /// Gets the kind of the query node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.Constant;
}
