//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using System.Data;
using System.Dynamic;
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

public interface IUriResolver
{
    void PromoteBinaryOperandTypes(
             BinaryOperatorKind binaryOperatorKind,
             ref SingleValueNode leftNode,
             ref SingleValueNode rightNode,
             out Type typeReference);

}

public class  DefaultUriResolver : IUriResolver
{
    public virtual void PromoteBinaryOperandTypes(
              BinaryOperatorKind binaryOperatorKind,
              ref SingleValueNode leftNode,
              ref SingleValueNode rightNode,
              out Type typeReference)
    {
        typeReference = null;
        PromoteOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode/*, typeFacetsPromotionRules*/);
    }

    /// <summary>
    /// Promote the left and right operand types
    /// </summary>
    /// <param name="binaryOperatorKind">the operator kind</param>
    /// <param name="left">the left operand</param>
    /// <param name="right">the right operand</param>
    /// <param name="facetsPromotionRules">Promotion rules for type facets.</param>
    internal static void PromoteOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode left, ref SingleValueNode right/*, TypeFacetsPromotionRules facetsPromotionRules*/)
    {
        Type leftType;
        Type rightType;
        if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorKind, left, right, out leftType, out rightType/*, facetsPromotionRules*/))
        {
            string leftTypeName = left.NodeType == null ? "<null>" : left.NodeType.FullName;
            string rightTypeName = right.NodeType == null ? "<null>" : right.NodeType.FullName;
            throw new QueryParserException("Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, leftTypeName, rightTypeName, binaryOperatorKind)");
        }

        left = MetadataBindingUtils.ConvertToTypeIfNeeded(left, leftType);
        right = MetadataBindingUtils.ConvertToTypeIfNeeded(right, rightType);
    }
}

public abstract class AssemblyResolver : IAssemblyResolver
{
    /// <summary>
    /// Gets a list of assemblies available for the application.
    /// </summary>
    /// <returns>A list of assemblies available for the application. </returns>
    public abstract IEnumerable<Assembly> Assemblies { get; }
}
