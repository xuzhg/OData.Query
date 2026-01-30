//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Commons;

/// <summary>
/// The type related helper methods.
/// </summary>
internal static class TypeHelpers
{
    public static bool IsStructured(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        return !type.IsPrimitive
            && !type.IsEnum
            && type != typeof(string)
            && !type.IsValueType
            && Nullable.GetUnderlyingType(type) == null;
    }

    public static bool IsStructuredCollection(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        bool collection = type.IsCollection(out Type elementType);
        if (collection)
        {
            return elementType.IsStructured();
        }

        return false;
    }

    public static bool IsIntegral(this Type type)
    {
        if (type == null)
        {
            return false;
        }

        if (type == typeof(sbyte) ||
            type == typeof(short) ||
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(byte) ||
            type == typeof(ushort) ||
            type == typeof(uint) ||
            type == typeof(ulong))
        { 
                return true;
        }

        return false;
    }

    public static Type GetElementTypeOrSelf(this Type type)
    {
        if (type == null ||
            type == typeof(string) ||
            type.IsPrimitive ||
            type.IsValueType ||
            Nullable.GetUnderlyingType(type) != null)
        {
            return type;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsCollection(out Type elementType))
        {
            return elementType;
        }

        return type;
    }

    /// <summary>
    /// Return the underlying type or itself.
    /// </summary>
    /// <param name="type">The input type.</param>
    /// <returns>The underlying type.</returns>
    public static Type GetUnderlyingTypeOrSelf(this Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    /// Determine if a type is null-able.
    /// </summary>
    /// <param name="clrType">The type to test.</param>
    /// <returns>True if the type is null-able; false otherwise.</returns>
    public static bool IsNullable(this Type clrType)
    {
        if (clrType == null)
        {
            return false;
        }

        if (clrType.IsValueType)
        {
            // value types are only nullable if they are Nullable<T>
            return clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        else
        {
            // reference types are always nullable
            return true;
        }
    }

    /// <summary>
    /// Return the type from a nullable type.
    /// </summary>
    /// <param name="clrType">The type to convert.</param>
    /// <returns>The type from a nullable type.</returns>
    public static Type ToNullable(this Type clrType)
    {
        if (IsNullable(clrType))
        {
            return clrType;
        }
        else
        {
            return typeof(Nullable<>).MakeGenericType(clrType);
        }
    }

    /// <summary>
    /// Determine if a type is a collection.
    /// </summary>
    /// <param name="clrType">The type to test.</param>
    /// <returns>True if the type is an enumeration; false otherwise.</returns>
    public static bool IsCollection(this Type clrType) => clrType.IsCollection(out _);

    /// <summary>
    /// Determine if a type is a collection.
    /// </summary>
    /// <param name="clrType">The type to test.</param>
    /// <param name="elementType">out: the element type of the collection.</param>
    /// <returns>True if the type is an enumeration; false otherwise.</returns>
    public static bool IsCollection(this Type clrType, out Type elementType)
    {
        elementType = clrType;
        if (clrType == null)
        {
            return false;
        }

        // see if this type should be ignored.
        if (clrType == typeof(string))
        {
            return false;
        }

        // Since IDictionary<T,T> is a collection of KeyValuePair<T,T>
        if (clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        {
            return false;
        }

        Type collectionInterface
            = clrType.GetInterfaces()
                .Union(new[] { clrType })
                .FirstOrDefault(
                    t => t.IsGenericType
                         && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                         || t.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)));

        if (collectionInterface != null)
        {
            elementType = collectionInterface.GetGenericArguments().Single();
            return true;
        }

        return false;
    }
}
