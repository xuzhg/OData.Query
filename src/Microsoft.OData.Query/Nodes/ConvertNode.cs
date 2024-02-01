//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing a conversion of primitive type to another type.
/// </summary>
public class ConvertNode : SingleValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertNode" /> class.
    /// </summary>
    /// <param name="source">The node to convert.</param>
    /// <param name="type"> The type to convert the node to</param>
    public ConvertNode(SingleValueNode source, Type type)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        NodeType = type ?? throw new ArgumentNullException(nameof(type));
    }

    /// <summary>
    /// Get the source value to convert.
    /// </summary>
    public SingleValueNode Source { get; }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.Convert;

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType { get; }
}
