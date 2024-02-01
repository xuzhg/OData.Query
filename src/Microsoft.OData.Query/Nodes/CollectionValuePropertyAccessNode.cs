//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Reflection;
using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing an access to a collection property value.
/// </summary>
public class CollectionValuePropertyAccessNode : CollectionValueNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionValuePropertyAccessNode" /> class.
    /// </summary>
    /// <param name="source">The node to convert.</param>
    /// <param name="property">The property.</param>
    public CollectionValuePropertyAccessNode(SingleValueNode source, PropertyInfo property)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Property = property ?? throw new ArgumentNullException(nameof(property));

        if (!property.PropertyType.IsCollection(out Type elementType))
        {
            throw new ArgumentException($"property '{property.Name}' is not a collection.");
        }

        ElementType = elementType;
    }

    /// <summary>
    /// Get the source value containing the property.
    /// </summary>
    public SingleValueNode Source { get; }

    /// <summary>
    /// Gets the EDM property which is to be accessed.
    /// </summary>
    /// <remarks>Only non-entity, collection properties are supported by this node.</remarks>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the kind of this node.
    /// </summary>
    public override QueryNodeKind Kind => QueryNodeKind.CollectionValuePropertyAccess;

    /// <summary>
    /// Gets the type of this node represents.
    /// </summary>
    public override Type NodeType => Property.PropertyType;

    public override Type ElementType { get; }
}
