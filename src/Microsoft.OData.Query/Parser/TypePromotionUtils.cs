//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using System.Diagnostics;

namespace Microsoft.OData.Query.Parser;

internal static class TypePromotionUtils
{
    /// <summary>Checks that the operands (possibly promoted) are valid for the specified operation.</summary>
    /// <param name="operatorKind">The operator kind to promote the operand types for.</param>
    /// <param name="leftNode">The left operand node.</param>
    /// <param name="rightNode">The right operand node.</param>
    /// <param name="left">The left operand type after promotion.</param>
    /// <param name="right">The right operand type after promotion.</param>
    /// <param name="facetsPromotionRules">Promotion rules for type facets.</param>
    /// <returns>True if a valid function signature was found that matches the given types after any necessary promotions are made.
    /// False if there is no binary operators </returns>
    internal static bool PromoteOperandTypes(
        BinaryOperatorKind operatorKind,
        SingleValueNode leftNode,
        SingleValueNode rightNode,
        out Type left,
        out Type right
        /*TypeFacetsPromotionRules facetsPromotionRules*/)
    {
        left = leftNode.NodeType;
        right = rightNode.NodeType;

        // The types for the operands can be null
        // if they (a) represent the null literal or (b) represent an open type/property.
        // If both argument types are null we lack type information on both sides and cannot promote arguments.
        if (left == null && right == null)
        {
            // if we find null literals or open properties on both sides we cannot promote; the result type will also be null
            return true;
        }

        if (left == right && left == typeof(string))
        {
            return true;
        }

        if (operatorKind == BinaryOperatorKind.NotEqual || operatorKind == BinaryOperatorKind.Equal || operatorKind == BinaryOperatorKind.Has)
        {
            if (TryHandleEqualityOperatorForEntityOrComplexTypes(ref left, ref right))
            {
                return true;
            }

            // Comparing an enum with a string or int is valid
            if (left != null && right != null && left.IsEnum && (right == typeof(string) || right.IsIntegral()))
            {
                right = left;
                return true;
            }

            // Comparing an enum with a string or int is valid
            if (left != null && right != null && right.IsEnum && (left == typeof(string) || left.IsIntegral()))
            {
                left = right;
                return true;
            }

            // enum and spatial type support equality operator for null operand:
            // TODO: Sam Xu: support spatial later
            //if ((left == null) && (right != null) && (right.IsEnum || right is IEdmSpatialTypeReference))
            //{
            //    left = right;
            //    return true;
            //}

            //if ((right == null) && (left != null) && (left.IsEnum() || left is IEdmSpatialTypeReference))
            //{
            //    right = left;
            //    return true;
            //}
        }

        // enum support, check type full names
        if (left != null && right != null && left.IsEnum && right.IsEnum)
        {
            return string.Equals(left.FullName, right.FullName, StringComparison.Ordinal);
        }

        // type definition support, treat type definitions as their underlying types.
        //if (left != null && left.IsTypeDefinition())
        //{
        //    left = left.AsPrimitive();
        //}

        //if (right != null && right.IsTypeDefinition())
        //{
        //    right = right.AsPrimitive();
        //}


        // Except for above, binary operators are only supported on primitive types
        if (left != null && !left.IsPrimitive || right != null && !right.IsPrimitive)
        {
            return false;
        }

        return true;

        // The following will match primitive argument types to build in function signatures, choosing the best one possible.
        //FunctionSignature bestMatch;
        //bool success = FindBestSignature(
        //    GetFunctionSignatures(operatorKind),
        //    new[] { leftNode, rightNode },
        //    new[] { left, right },
        //    out bestMatch)
        //    == 1;

        //if (success)
        //{
        //    int? leftPrecision, leftScale;
        //    int? rightPrecision, rightScale;
        //    GetTypeFacets(left, out leftPrecision, out leftScale);
        //    GetTypeFacets(right, out rightPrecision, out rightScale);
        //    var resultPrecision = facetsPromotionRules.GetPromotedPrecision(leftPrecision, rightPrecision);
        //    var resultScale = facetsPromotionRules.GetPromotedScale(leftScale, rightScale);

        //    left = bestMatch.GetArgumentTypeWithFacets(0, resultPrecision, resultScale);
        //    right = bestMatch.GetArgumentTypeWithFacets(1, resultPrecision, resultScale);
        //}

        //return success;
    }

    /// <summary>
    /// Tries to handle the special eq and ne operators, which have a broader definition than the other binary operators.
    /// We try a few special cases and return true if we used one of them. Otherwise we return false, and
    /// allow the regular function matching code to handle the primitive cases.
    /// </summary>
    /// <param name="left">Left type.</param>
    /// <param name="right">Right type.</param>
    /// <returns>True if this function was able to handle the promotion of these types, false otherwise.</returns>
    private static bool TryHandleEqualityOperatorForEntityOrComplexTypes(ref Type left, ref Type right)
    {
        if (left != null && left.IsStructured())
        {
            // When one is null and the other isn't, we use the other's type for the null one
            if (right == null)
            {
                right = left;
                return true;
            }

            // When one is structured but the other primitive, there is no match
            if (!right.IsStructured())
            {
                return false;
            }

            // If they are the same type but have different nullability, we need to choose the nullable one for both
            if (left.IsEquivalentTo(right))
            {
                if (left.IsNullable() && !right.IsNullable())
                {
                    right = left;
                }
                else
                {
                    left = right;
                }

                return true;
            }

            // I think we should just use IsAssignableFrom instead now
            if (CanConvertTo(null, left, right))
            {
                left = right;
                return true;
            }

            if (CanConvertTo(null, right, left))
            {
                right = left;
                return true;
            }

            return false;
        }

        // Left was null or primitive
        if (right != null && (right.IsStructured()))
        {
            if (left == null)
            {
                left = right;
                return true;
            }

            return false;
        }

        return false;
    }

    internal static bool CanConvertTo(SingleValueNode sourceNodeOrNull, Type sourceReference, Type targetReference)
    {
        Debug.Assert(sourceReference != null, "sourceReference != null");
        Debug.Assert(targetReference != null, "targetReference != null");

        //// Copy of the ResourceQueryParser.ExpressionParser.IsCompatibleWith method.

        if (sourceReference.IsEquivalentTo(targetReference))
        {
            return true;
        }

        // We must support conversion from/to untyped.
        if (sourceReference == typeof(object) || targetReference == typeof(object))
        {
            return true;
        }

        if (targetReference.IsStructured())
        {
            // Both targetReference and sourceReference are cast to IEdmStructuredType,
            // therefore we must check both types
            if (!sourceReference.IsStructured())
            {
                return false;
            }

            // for structured types, use IsAssignableFrom
            return targetReference.IsAssignableFrom(sourceReference);
            //return MetadataUtilsCommon.CanConvertStructuredTypeTo(
            //(IEdmStructuredType)targetReference.Definition,
            //    (IEdmStructuredType)sourceReference.Definition);
        }

        //// This rule stops the parser from considering nullable types as incompatible
        //// with non-nullable types. We have however implemented this rule because we just
        //// access underlying rules. C# requires an explicit .Value access, and EDM has no
        //// nullablity on types and (at the model level) implements null propagation.
        ////
        //// if (sourceReference.IsNullable && !targetReference.IsNullable)
        //// {
        ////     return false;
        //// }

        if (sourceReference.IsEnum && targetReference.IsEnum)
        {
            if (sourceReference.IsEquivalentTo(targetReference))
            {
                return targetReference.IsNullable() || (!sourceReference.IsNullable());
            }

            return false;
        }

        if (targetReference.IsEnum && sourceReference == typeof(string))
        {
            return true;
        }

        //IEdmPrimitiveTypeReference sourcePrimitiveTypeReference = sourceReference.AsPrimitiveOrNull();
        //IEdmPrimitiveTypeReference targetPrimitiveTypeReference = targetReference.AsPrimitiveOrNull();

        if (!sourceReference.IsPrimitive || !targetReference.IsPrimitive)
        {
            return false;
        }

        return CanConvertPrimitiveTypeTo(sourceNodeOrNull, sourceReference, targetReference);
    }

    /// <summary>
    /// Determines if a <paramref name="sourcePrimitiveType"/> is convertible according to OData rules to the
    /// <paramref name="targetPrimitiveType"/>.
    /// </summary>
    /// <param name="sourceNodeOrNull">The node which is to be converted.</param>
    /// <param name="sourcePrimitiveType">The type which is to be converted.</param>
    /// <param name="targetPrimitiveType">The type to which we want to convert.</param>
    /// <returns>true if the source type is convertible to the target type; otherwise false.</returns>
    internal static bool CanConvertPrimitiveTypeTo(
        SingleValueNode sourceNodeOrNull,
        Type sourcePrimitiveType,
        Type targetPrimitiveType)
    {
        Debug.Assert(sourcePrimitiveType != null, "sourcePrimitiveType != null");
        Debug.Assert(targetPrimitiveType != null, "targetPrimitiveType != null");

       // EdmPrimitiveTypeKind sourcePrimitiveKind = sourcePrimitiveType.PrimitiveKind;
       // EdmPrimitiveTypeKind targetPrimitiveKind = targetPrimitiveType.PrimitiveKind;

        if (sourcePrimitiveType.IsPrimitive && targetPrimitiveType.IsPrimitive && sourcePrimitiveType == targetPrimitiveType)
        {
            return true;
        }

        if (sourcePrimitiveType == typeof(sbyte))
        {
            if (targetPrimitiveType == typeof(sbyte) ||
                targetPrimitiveType == typeof(short) ||
                targetPrimitiveType == typeof(ushort) ||
                targetPrimitiveType == typeof(int) ||
                targetPrimitiveType == typeof(uint) ||
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(byte))
        {
            if (targetPrimitiveType == typeof(short) ||
                targetPrimitiveType == typeof(ushort) ||
                targetPrimitiveType == typeof(int) ||
                targetPrimitiveType == typeof(uint) ||
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(short))
        {
            if (targetPrimitiveType == typeof(ushort) ||
                targetPrimitiveType == typeof(int) ||
                targetPrimitiveType == typeof(uint) ||
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(ushort))
        {
            if (targetPrimitiveType == typeof(int) ||
                targetPrimitiveType == typeof(uint) ||
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(int))
        {
            if (targetPrimitiveType == typeof(uint) ||
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(uint))
        {
            if (
                targetPrimitiveType == typeof(long) ||
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(long))
        {
            if (
                targetPrimitiveType == typeof(ulong) ||
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(ulong))
        {
            if (
                targetPrimitiveType == typeof(float) ||
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(float))
        {
            if (
                targetPrimitiveType == typeof(double) ||
                targetPrimitiveType == typeof(decimal))
            {
                return true;
            }

            if (targetPrimitiveType == typeof(decimal))
            {
                return TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out _);
            }
        }


        if (sourcePrimitiveType == typeof(double))
        {
            if (targetPrimitiveType == typeof(decimal))
            {
                return TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out _);
            }
        }

        if (sourcePrimitiveType == typeof(DateOnly))
        {
            if (targetPrimitiveType == typeof(DateOnly) ||
                targetPrimitiveType == typeof(DateTimeOffset))
            {
                return true;
            }
        }

        if (sourcePrimitiveType == typeof(TimeOnly))
        {
            if (targetPrimitiveType == typeof(TimeOnly) ||
                targetPrimitiveType == typeof(TimeSpan))
            {
                return true;
            }
        }
        
        if (targetPrimitiveType.IsAssignableFrom(sourcePrimitiveType))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries getting the constant node's primitive L M D F value (which can be converted to other primitive type while primitive AccessPropertyNode can't).
    /// </summary>
    /// <param name="sourceNodeOrNull">The Node</param>
    /// <param name="primitiveValue">THe out parameter if succeeds</param>
    /// <returns>true if the constant node is for long, float, double or decimal type</returns>
    internal static bool TryGetConstantNodePrimitiveLDMF(SingleValueNode sourceNodeOrNull, out object primitiveValue)
    {
        primitiveValue = null;

        ConstantNode tmp = sourceNodeOrNull as ConstantNode;
        if (tmp != null)
        {
            Type type = tmp.NodeType;
            if (type != null)
            {
                if (type == typeof(int) ||
                    type == typeof(long) ||
                    type == typeof(float) ||
                    type == typeof(double) ||
                    type == typeof(decimal))
                {
                        primitiveValue = tmp.Value;
                        return true;
                }
            }
        }

        return false;
    }
}