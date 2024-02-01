//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base class for all semantic metadata bound nodes.
/// It's node for abstract search tree (AST).
/// </summary>
public abstract class QueryNode
{
    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public abstract QueryNodeKind Kind { get; }

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public abstract Type NodeType { get; }
}
