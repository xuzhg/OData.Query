//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing an access to a property value.
/// </summary>
public class SingleValueFunctionCallNode : SingleValueNode
{
    /// <summary>
    /// List of arguments to this function call.
    /// </summary>
    private readonly IEnumerable<QueryNode> _parameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleValueFunctionCallNode" /> class.
    /// </summary>
    /// <param name="source">The node to convert.</param>
    /// <param name="name">The name of the function to call</param>
    /// <param name="parameters">the list of arguments to this function</param>
    /// <param name="returnType"> The type to convert the node to</param>
    public SingleValueFunctionCallNode(QueryNode source, string name, IEnumerable<QueryNode> parameters, Type returnType)
    {
        Source = source;
        Name = name ?? throw new ArgumentNullException(nameof(name));

        _parameters = parameters ?? Enumerable.Empty<QueryNode>();

        NodeType = returnType;
    }

    /// <summary>
    /// Gets the name of the function to call.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the list of arguments to this function call.
    /// </summary>
    public IEnumerable<QueryNode> Parameters => _parameters;

    /// <summary>
    /// Get the source value containing the property.
    /// </summary>
    public QueryNode Source { get; }

    /// <summary>
    /// Gets the semantically bound parent of this function.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.SingleValueFunctionCall;

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType { get; }
}
