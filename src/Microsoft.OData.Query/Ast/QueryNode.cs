//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

/// <summary>
/// Base class for all semantic metadata bound nodes.
/// </summary>
public abstract class QueryNode
{
    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public abstract QueryNodeKind Kind { get; }
}
