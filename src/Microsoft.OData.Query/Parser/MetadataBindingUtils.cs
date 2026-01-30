//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using System.Diagnostics;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Helper methods for metadata binding.
/// </summary>
internal static class MetadataBindingUtils
{
    /// <summary>
    /// If the source node is not of the specified type, then we check if type promotion is possible and inject a convert node.
    /// If the source node is the same type as the target type (or if the target type is null), we just return the source node as is.
    /// </summary>
    /// <param name="source">The source node to apply the conversion to.</param>
    /// <param name="targetTypeReference">The target primitive type. May be null - this method will do nothing in that case.</param>
    /// <returns>The converted query node, or the original source node unchanged.</returns>
    internal static SingleValueNode ConvertToTypeIfNeeded(SingleValueNode source, Type targetTypeReference)
    {
        Debug.Assert(source != null, "source != null");

        if (targetTypeReference == null)
        {
            return source;
        }

        if (source.NodeType != null)
        {
            if (source.NodeType.IsEquivalentTo(targetTypeReference))
            {
                // For source is type definition, if source's underlying type == target type.
                // We create a conversion node from source to its underlying type (target type)
                // so that the service can convert value of source clr type to underlying clr type.
                //if (source.TypeReference.IsTypeDefinition())
                //{
                //    return new ConvertNode(source, targetTypeReference);
                //}

                return source;
            }

            // Structured type in url will be translated into a node with raw string value.
            // We create a conversion node from string to structured type.
            if (targetTypeReference.IsStructured() || targetTypeReference.IsStructuredCollection())
            {
                return new ConvertNode(source, targetTypeReference);
            }

            ConstantNode constantNode = source as ConstantNode;
            // Check if the source node is a constant node, not null, and the source type is either string or integral
            // and the target type is an enum.
            if (constantNode != null && constantNode.Value != null && (source.NodeType == typeof(string) || source.NodeType.IsIntegral()) && targetTypeReference.IsEnum)
            {
                //string memberName = constantNode.Value.ToString();
                //IEdmEnumType enumType = targetTypeReference.Definition as IEdmEnumType;
                //if (enumType.ContainsMember(memberName, StringComparison.Ordinal))
                //{
                //    string literalText = ODataUriUtils.ConvertToUriLiteral(constantNode.Value, default(ODataVersion));
                //    return new ConstantNode(new ODataEnumValue(memberName, enumType.ToString()), literalText, targetTypeReference);
                //}

                // If the member name is an integral value, we should try to convert it to the enum member name and find the enum member with the matching integral value
                //if (long.TryParse(memberName, out long memberIntegralValue) && enumType.TryParse(memberIntegralValue, out IEdmEnumMember enumMember))
                //{
                //    string literalText = ODataUriUtils.ConvertToUriLiteral(enumMember.Name, default(ODataVersion));
                //    return new ConstantNode(new ODataEnumValue(enumMember.Name, enumType.ToString()), literalText, targetTypeReference);
                //}

                //throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, memberName));
            }

            if (!TypePromotionUtils.CanConvertTo(source, source.NodeType, targetTypeReference))
            {
                throw new QueryParserException("Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName())");
            }
            else
            {
                //if (source.NodeType.IsEnum && constantNode != null)
                //{
                //    return new ConstantNode(constantNode.Value, ODataUriUtils.ConvertToUriLiteral(constantNode.Value, ODataVersion.V4), targetTypeReference);
                //}

                //    object originalPrimitiveValue;
                //    if (TypePromotionUtils.TryGetConstantNodePrimitiveLDMF(source, out originalPrimitiveValue) && (originalPrimitiveValue != null))
                //    {
                //        // L F D M types : directly create a ConvertNode.
                //        // 1. NodeToExpressionTranslator.cs won't allow implicitly converting single/double to decimal, which should be done here at Node tree level.
                //        // 2. And prevent losing precision in float -> double, e.g. (double)1.234f => 1.2339999675750732d not 1.234d
                //        object targetPrimitiveValue = ODataUriConversionUtils.CoerceNumericType(originalPrimitiveValue, targetTypeReference.AsPrimitive().Definition as IEdmPrimitiveType);

                //        if (string.IsNullOrEmpty(constantNode.LiteralText))
                //        {
                //            return new ConstantNode(targetPrimitiveValue);
                //        }

                //        var candidate = new ConstantNode(targetPrimitiveValue, constantNode.LiteralText);
                //        var decimalType = candidate.TypeReference as IEdmDecimalTypeReference;
                //        if (decimalType != null)
                //        {
                //            var targetDecimalType = (IEdmDecimalTypeReference)targetTypeReference;
                //            return decimalType.Precision == targetDecimalType.Precision &&
                //                   decimalType.Scale == targetDecimalType.Scale ?
                //                   (SingleValueNode)candidate :
                //                   (SingleValueNode)(new ConvertNode(candidate, targetTypeReference));
                //        }
                //        else
                //        {
                //            return candidate;
                //        }
                //    }
                //    else
                //    {
                //        // other type conversion : ConvertNode
                //        return new ConvertNode(source, targetTypeReference);
                //    }
                //}

                throw new QueryParserException("TODO: Sam Xu, handle other type conversion");
            }
        }
        else
        {
            // If the source doesn't have a type (possibly an open property), then it's possible to convert it
            // cause we don't know for sure.
            return new ConvertNode(source, targetTypeReference);
        }
    }
}