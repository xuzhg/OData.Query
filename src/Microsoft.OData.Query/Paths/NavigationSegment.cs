//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Reflection;

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A segment representing a navigation property.
/// </summary>
public class NavigationSegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationSegment" /> class.
    /// </summary>
    /// <param name="property">The navigation property that this segment represents.</param>
    public NavigationSegment(PropertyInfo property)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));

        Identifier = property.Name;
    }

    /// <summary>
    /// Gets the navigation property that this segment represents.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> of this <see cref="NavigationSegment"/>.
    /// </summary>
    public override Type SegmentType => Property.PropertyType;
}