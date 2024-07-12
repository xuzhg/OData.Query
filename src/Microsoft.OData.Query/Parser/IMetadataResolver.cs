//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Data;
using System.Reflection;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// The Assembly resolver interface.
/// </summary>
public interface IAssemblyResolver
{
    /// <summary>
    /// Gets a list of assemblies available for the application.
    /// </summary>
    /// <returns>A list of assemblies available for the application. </returns>
    IEnumerable<Assembly> Assemblies { get; }
}

public interface IMetadataResolver
{
    Type ResolveType(string fullTypeName);

    PropertyInfo ResolveProperty(Type type, string propertyName);
}

public class MetadataResolver : IMetadataResolver
{
    public virtual Type ResolveType(string fullTypeName)
    {
        return null;
    }

    public virtual PropertyInfo ResolveProperty(Type type, string propertyName)
    {
        // Search case-sensitive first
        PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (propertyInfo != null)
        {
            return propertyInfo;
        }

        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        foreach (var candidate in properties )
        {
            if (propertyInfo == null)
            {
                propertyInfo = candidate;
            }
            else
            {
                throw new Exception("Strings.UriParserMetadata_MultipleMatchingPropertiesFound(propertyName, type.FullTypeName())");
            }
        }

        return propertyInfo;
    }
}
