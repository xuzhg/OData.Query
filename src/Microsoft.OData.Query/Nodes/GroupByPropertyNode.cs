//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// A node representing a grouping property.
/// </summary>
public sealed class GroupByPropertyNode
{
    private IList<GroupByPropertyNode> childTransformations = new List<GroupByPropertyNode>();

    private Type typeReference;

    /// <summary>
    /// Create a GroupByPropertyNode.
    /// </summary>
    /// <param name="name">The name of this node.</param>
    /// <param name="expression">The <see cref="SingleValueNode"/> of this node.</param>
    public GroupByPropertyNode(string name, SingleValueNode expression)
    {
        //ExceptionUtils.CheckArgumentNotNull(name, "name");

        this.Name = name;
        this.Expression = expression;
    }

    /// <summary>
    /// Create a GroupByPropertyNode.
    /// </summary>
    /// <param name="name">The name of this node.</param>
    /// <param name="expression">The <see cref="SingleValueNode"/> of this node.</param>
    /// <param name="type">The <see cref="IEdmTypeReference"/> of this node.</param>
    public GroupByPropertyNode(string name, SingleValueNode expression, Type type)
        : this(name, expression)
    {
        this.typeReference = type;
    }

    /// <summary>
    /// Gets the name of this node.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the <see cref="SingleValueNode"/> of this node.
    /// </summary>
    public SingleValueNode Expression { get; private set; }

    /// <summary>
    /// Gets the <see cref="IEdmTypeReference"/> of this node.
    /// </summary>
    public Type TypeReference
    {
        get
        {
            if (Expression == null)
            {
                return null;
            }
            else
            {
                return typeReference;
            }
        }
    }

    /// <summary>
    /// Gets or sets the child transformations <see cref="GroupByPropertyNode"/>s of this node.
    /// TODO: remove it
    /// </summary>
    public IList<GroupByPropertyNode> ChildTransformations
    {
        get
        {
            return childTransformations;
        }

        set
        {
            childTransformations = value;
        }
    }
}
