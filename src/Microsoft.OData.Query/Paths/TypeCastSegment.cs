//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Paths;

/// <summary>
/// A segment representing a cast on the previous segment to another type.
/// </summary>
public sealed class TypeCastSegment : PathSegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeCastSegment" /> class.
    /// </summary>
    /// <param name="type">The type that this segment represents.</param>
    /// <exception cref="System.ArgumentNullException">Throws if the input property is null.</exception>
    public TypeCastSegment(Type type)
    {
        SegmentType = type ?? throw new ArgumentNullException(nameof(type));

        Identifier = type.FullName;
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of this <see cref="TypeCastSegment"/>.
    /// </summary>
    public override Type SegmentType { get; }
}