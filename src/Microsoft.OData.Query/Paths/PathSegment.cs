//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// The semantic representation of a segment in a path.
/// </summary>
public abstract class PathSegment
{
    /// <summary>
    /// Gets the <see cref="Type"/> of this <see cref="PathSegment"/>.
    /// </summary>
    public abstract Type SegmentType { get; }

    /// <summary>
    /// Gets the identifier for this segment i.e. string part without the keys.
    /// </summary>
    public string Identifier { get; set; }
}
