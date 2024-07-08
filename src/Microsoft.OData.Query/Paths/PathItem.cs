//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A representation of the path portion of an OData URI which is made up of <see cref="PathSegment"/>s.
/// </summary>
public class PathItem
{
    /// <summary>
    /// Creates a new instance of <see cref="PathItem"/> containing the given segments.
    /// </summary>
    /// <param name="segments">The segments that make up the path.</param>
    /// <exception cref="ArgumentNullException">Throws if input segments is null.</exception>
    public PathItem(params PathSegment[] segments)
        : this((IEnumerable<PathSegment>)segments)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PathItem"/> class.
    /// </summary>
    /// <param name="segments">The segments.</param>
    public PathItem(IEnumerable<PathSegment> segments)
    {
        Segments = segments.ToArray(); // throws if the segments is null
        if (Segments.Any(s => s == null))
        {
            throw new ArgumentNullException(nameof(segments));
        }
    }

    /// <summary>
    /// Gets the segments.
    /// </summary>
    public PathSegment[] Segments { get; }

    /// <summary>
    /// Gets the first segment in the path. Returns null if the path is empty.
    /// </summary>
    public PathSegment FirstSegment => Segments.Length == 0 ? null : Segments[0];

    /// <summary>
    /// Get the last segment in the path. Returns null if the path is empty.
    /// </summary>
    public PathSegment LastSegment => Segments.Length == 0 ? null : Segments[Segments.Length - 1];

    /// <summary>
    /// Get the number of segments in this path.
    /// </summary>
    public int Length => Segments.Length;
}
