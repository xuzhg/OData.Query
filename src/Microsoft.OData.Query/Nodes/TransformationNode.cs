//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Base class for all transformation nodes.
/// </summary>
public abstract class TransformationNode : QueryNode
{
    /// <summary>
    /// Gets kind of transformation: groupby, aggregate, filter etc.
    /// </summary>
    public abstract TransformationNodeKind TransformKind { get; }
}
