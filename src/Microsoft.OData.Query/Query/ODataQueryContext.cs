//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Microsoft.OData.Query;

public class ODataQueryContext
{
    public ODataQueryContext(Type elementClrType)
    {
        
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the element.
    /// </summary>
    //[ComplexType]
    public Type ElementType { get; }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NavigationPropertyAttribute : Attribute
{
    // Without any configuration, all properties are treated as structural properties.
    // Customer can use this to decorate a property if such property behaviors like a navigation property.
    // Without this property, all properties are treated as structural properties.

    // Attribute wins.
    // If customer can't modify the model class, he can use the IMetadataProvider to specify the property kind.
}

// How to define a type or property
public interface IMetadataProvider
{

}

public class MetadataProvider : IMetadataProvider
{
    public bool IsNavigationProperty(PropertyInfo propertyInfo)
    {
        return true;
    }

    public Type GetType(string fullTypeName)
    {
        return null;
    }
}