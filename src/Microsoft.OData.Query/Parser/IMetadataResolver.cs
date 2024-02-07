//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.VisualBasic;
using System.Data;
using System.Reflection;

namespace Microsoft.OData.Query.Parser;

public interface IMetadataResolver
{
    PropertyInfo ResolveProperty(Type type, string propertyName);
}

public class MetadataResolver : IMetadataResolver
{
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
