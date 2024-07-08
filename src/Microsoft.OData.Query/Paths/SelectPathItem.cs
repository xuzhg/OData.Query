//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A specific type of <see cref="PathItem"/> which can only contain instances of <see cref="TypeCastSegment"/>, <see cref="NavigationSegment"/>,
/// <see cref="PropertySegment"/>.
/// </summary>
public class SelectPathItem : PathItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="SelectPathItem"/> class.
    /// </summary>
    /// <param name="segments">The segments.</param>
    public SelectPathItem(IEnumerable<PathSegment> segments)
        : base(segments)
    {
        Verify();
    }

    /// <summary>
    /// Creates a new instance of <see cref="SelectPathItem"/> containing the given segments.
    /// </summary>
    /// <param name="segments">The segments that make up the path.</param>
    public SelectPathItem(params PathSegment[] segments)
        : this((IEnumerable<PathSegment>)segments)
    {
    }

    private void Verify()
    {
        int total = Segments.Length;
        if (total == 1 && Segments[0] is TypeCastSegment)
        {
            throw new PathException(Error.Format(SRResources.SelectPath_CannotOnlyHaveTypeSegment, Segments[0].Identifier));
        }

        for (int i = 0; i < total; ++i)
        {
            PathSegment segment = Segments[i];
            if (segment is NavigationSegment && i != total - 1)
            {
                throw new PathException(Error.Format(SRResources.SelectPath_NavPropSegmentCanOnlyBeLastSegment, segment.Identifier));
            }
            //else if (segment is OperationSegment)
            //{
            //    if (index != this.Count - 1)
            //    {
            //        throw new ODataException(ODataErrorStrings.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
            //    }
            //}
            else if (/*segment is DynamicPathSegment ||*/ segment is PropertySegment || segment is TypeCastSegment /*|| segment is AnnotationSegment*/)
            {
                continue;
            }
            else
            {
                throw new PathException(Error.Format(SRResources.SelectPath_InvalidSelectPathSegmentType, segment.Identifier, segment.GetType().Name));
            }
        }
    }
}
