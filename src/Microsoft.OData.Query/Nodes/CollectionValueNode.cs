//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base class for all semantic metadata bound nodes which represent a composable collection of values.
/// </summary>
public abstract class CollectionValueNode : QueryNode
{
    /// <summary>
    /// The resource type of a single item from the collection represented by this node.
    /// </summary>
    public abstract Type ElementType { get; }
}
