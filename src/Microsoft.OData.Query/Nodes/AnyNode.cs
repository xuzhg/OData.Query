//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Query node representing an Any query.
/// </summary>
public class AnyNode : LambdaNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllNode" /> class.
    /// </summary>
    /// <param name="rangeVariables">The name of the rangeVariables list.</param>
    /// <param name="currentRangeVariable">The new range variable being added by this all node</param>
    public AnyNode(CollectionValueNode source, Collection<RangeVariable> rangeVariables, RangeVariable currentRangeVariable, SingleValueNode body)
        : base(source, rangeVariables, currentRangeVariable, body)
    {
    }

    /// <summary>
    /// The resource type of the single value this node represents.
    /// </summary>
    public override Type NodeType => typeof(bool?);

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.Any;
}