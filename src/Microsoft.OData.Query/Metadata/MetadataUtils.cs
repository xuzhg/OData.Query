//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Metadata;

internal static class MetadataUtils
{
    private static readonly Dictionary<Type, PrimitiveTypeKind> PrimitiveTypeReferenceMap
        = new Dictionary<Type, PrimitiveTypeKind>();

    static MetadataUtils()
    {
        PrimitiveTypeReferenceMap[typeof(bool)] = PrimitiveTypeKind.Boolean;
        PrimitiveTypeReferenceMap[typeof(byte)] = PrimitiveTypeKind.Byte;
        PrimitiveTypeReferenceMap[typeof(decimal)] = PrimitiveTypeKind.Decimal;
        PrimitiveTypeReferenceMap[typeof(double)] = PrimitiveTypeKind.Double;
        PrimitiveTypeReferenceMap[typeof(short)] = PrimitiveTypeKind.Int16;
        PrimitiveTypeReferenceMap[typeof(int)] = PrimitiveTypeKind.Int32;
        PrimitiveTypeReferenceMap[typeof(long)] = PrimitiveTypeKind.Int64;
        PrimitiveTypeReferenceMap[typeof(sbyte)] = PrimitiveTypeKind.SByte;
        PrimitiveTypeReferenceMap[typeof(string)] = PrimitiveTypeKind.String;
        PrimitiveTypeReferenceMap[typeof(float)] = PrimitiveTypeKind.Single;
        PrimitiveTypeReferenceMap[typeof(DateTime)] = PrimitiveTypeKind.Date;
        PrimitiveTypeReferenceMap[typeof(DateTimeOffset)] = PrimitiveTypeKind.DateTimeOffset;
        PrimitiveTypeReferenceMap[typeof(Guid)] = PrimitiveTypeKind.Guid;
        PrimitiveTypeReferenceMap[typeof(TimeSpan)] = PrimitiveTypeKind.Duration;
        PrimitiveTypeReferenceMap[typeof(byte[])] = PrimitiveTypeKind.Binary;
        PrimitiveTypeReferenceMap[typeof(Stream)] = PrimitiveTypeKind.Stream;
        PrimitiveTypeReferenceMap[typeof(DateOnly)] = PrimitiveTypeKind.DateOnly;
        PrimitiveTypeReferenceMap[typeof(TimeOnly)] = PrimitiveTypeKind.TimeOnly;
    }

    /// <summary>
    /// Checks whether a type reference refers to an OData primitive type (i.e., a primitive, non-stream type).
    /// </summary>
    /// <param name="type">The (non-null) <see cref="type"/> to check.</param>
    /// <returns>true if the <paramref name="type"/> is an primitive type reference; otherwise false.</returns>
    public static bool IsPrimitiveTypeKind(this Type type)
    {
        Type underlyingType = type.GetUnderlyingTypeOrSelf();
        return PrimitiveTypeReferenceMap.TryGetValue(underlyingType, out _);
    }

    public static PrimitiveTypeKind GetPrimitiveTypeKind(this Type type)
    {
        Type underlyingType = type.GetUnderlyingTypeOrSelf();
        PrimitiveTypeKind kind;
        if (PrimitiveTypeReferenceMap.TryGetValue(underlyingType, out kind))
        {
            return kind;
        }

        return PrimitiveTypeKind.None;
    }
}
