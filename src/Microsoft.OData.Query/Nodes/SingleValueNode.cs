//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------


namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base class for all semantic metadata bound nodes which represent a single composable value.
/// </summary>
public abstract class SingleValueNode : QueryNode
{
    /// <summary>
    /// Gets the type of the single value this node represents.
    /// </summary>
    public abstract Type NodeType { get; }
}
