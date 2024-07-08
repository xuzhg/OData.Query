//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A specific type of <see cref="PathItem"/> which can only contain instances of <see cref="TypeCastSegment"/>
/// or <see cref="NavigationSegment"/> or <see cref="PropertySegment"/> of complex.
/// </summary>
public class ExpandPathItem : PathItem
{
    /// <summary>
    /// Creates a new instance of <see cref="ExpandPathItem"/> containing the given segments.
    /// </summary>
    /// <param name="segments">The segments that make up the path.</param>
    public ExpandPathItem(params PathSegment[] segments)
        : this((IEnumerable<PathSegment>)segments)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ExpandPathItem"/> class.
    /// </summary>
    /// <param name="segments">The path segments.</param>
    public ExpandPathItem(IEnumerable<PathSegment> segments)
        : base(segments)
    {
        Verify();
    }

    private void Verify()
    {
        int total = Segments.Length;
        NavigationSegment preNavSegment = null;
        for (int i = 0; i < Segments.Length; ++i)
        {
            PathSegment segment = Segments[i];

            if (segment is PropertySegment && i == total - 1)
            {
                throw new PathException(Error.Format(SRResources.ExpandPath_InvalidLastSegment, segment.Identifier));
            }
            else if (segment is NavigationSegment)
            {
                if (preNavSegment != null)
                {
                    throw new PathException(Error.Format(SRResources.ExpandPath_MultipleNavigationProperties, preNavSegment.Identifier, segment.Identifier));
                }

                preNavSegment = (NavigationSegment)segment;
            }
            else if (!(segment is TypeCastSegment))
            {
                throw new PathException(Error.Format(SRResources.ExpandPath_LastSegmentMustBeNavigationPropertyOrTypeSegment, segment.Identifier, segment.GetType().Name));
            }
        }
    }
}
