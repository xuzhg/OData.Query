//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing a groupBy transformation.
/// </summary>
public sealed class GroupByTransformationNode : TransformationNode
{
    private readonly CollectionValueNode source;

    private readonly TransformationNode childTransformations;

    private readonly IEnumerable<GroupByPropertyNode> groupingProperties;

    /// <summary>
    /// Create a GroupByTransformationNode.
    /// </summary>
    /// <param name="groupingProperties">A list of <see cref="GroupByPropertyNode"/>.</param>
    /// <param name="childTransformations">The child <see cref="TransformationNode"/>.</param>
    /// <param name="source">The <see cref="CollectionNode"/> representing the source.</param>
    public GroupByTransformationNode(
        IList<GroupByPropertyNode> groupingProperties,
        TransformationNode childTransformations,
        CollectionValueNode source)
    {
      //  ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");

        this.groupingProperties = groupingProperties;
        this.childTransformations = childTransformations;
        this.source = source;
    }

    /// <summary>
    /// Gets the list of <see cref="GroupByPropertyNode"/>.
    /// </summary>
    public IEnumerable<GroupByPropertyNode> GroupingProperties
    {
        get
        {
            return groupingProperties;
        }
    }

    /// <summary>
    /// Gets the child <see cref="TransformationNode"/>.
    /// </summary>
    public TransformationNode ChildTransformations
    {
        get
        {
            return childTransformations;
        }
    }

    /// <summary>
    /// Gets the <see cref="CollectionNode"/> representing the source.
    /// </summary>
    public CollectionValueNode Source
    {
        get
        {
            return source;
        }
    }

    /// <summary>
    /// Gets the kind of the transformation node.
    /// </summary>
    public override TransformationNodeKind TransformKind => TransformationNodeKind.GroupBy;

  //  public override Type NodeType => throw new NotImplementedException();

    public override QueryNodeKind Kind => throw new NotImplementedException();
}