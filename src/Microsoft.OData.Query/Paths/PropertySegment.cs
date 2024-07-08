//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Reflection;

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A segment representing a structural property.
/// </summary>
public class PropertySegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySegment" /> class.
    /// </summary>
    /// <param name="property">The structural property that this segment represents.</param>
    public PropertySegment(PropertyInfo property)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));

        Identifier = property.Name;
    }

    /// <summary>
    /// Gets the structural property that this segment represents.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> of this <see cref="PropertySegment"/>.
    /// </summary>
    public override Type SegmentType => Property.PropertyType;
}
