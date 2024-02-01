//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;

internal static class BinaryOperatorNodeHelpers
{
    /// <summary>
    /// Compute the result type of a binary operator based on the type of its operands and the operator kind.
    /// </summary>
    /// <param name="left">The type reference on the left hand.</param>
    /// <param name="right">The type reference on the right hand.</param>
    /// <param name="operatorKind">The kind of operator.</param>
    /// <returns>The result type reference of the binary operator.</returns>
    internal static Type GetBinaryOperatorResultType(SingleValueNode left, SingleValueNode right, BinaryOperatorKind operatorKind)
    {
        if (left == null || right == null || left.NodeType == null || right.NodeType == null)
        {
            return null;
        }

        bool isNullable = left.NodeType.IsNullable();
        Type leftType = left.NodeType.GetUnderlyingTypeOrSelf();
        Type rightType = right.NodeType.GetUnderlyingTypeOrSelf();

        if (operatorKind == BinaryOperatorKind.Add)
        {
            if ((leftType == typeof(DateTimeOffset) && rightType == typeof(TimeSpan)) ||
                (leftType == typeof(TimeSpan) && rightType == typeof(DateTimeOffset)))
            {
                return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            }

            if ((leftType == typeof(DateOnly) && rightType == typeof(TimeSpan)) ||
                (leftType == typeof(TimeSpan) && rightType == typeof(DateOnly)))
            {
                return isNullable ? typeof(DateOnly?) : typeof(DateOnly);
            }

            if (leftType == typeof(TimeSpan) && rightType == typeof(TimeSpan))
            {
                return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            }
        }
        else if (operatorKind == BinaryOperatorKind.Subtract)
        {
            if (leftType == typeof(DateTimeOffset) && rightType == typeof(TimeSpan))
            {
                return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            }

            if (leftType == typeof(DateTimeOffset) && rightType == typeof(DateTimeOffset))
            {
                return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            }

            if (leftType == typeof(DateOnly) && rightType == typeof(TimeSpan))
            {
                return isNullable ? typeof(DateOnly?) : typeof(DateOnly);
            }

            if (leftType == typeof(DateOnly) && rightType == typeof(DateOnly))
            {
                return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            }

            if (leftType == typeof(TimeSpan) && rightType == typeof(TimeSpan))
            {
                return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            }
        }

        switch (operatorKind)
        {
            case BinaryOperatorKind.Or:                 // fall through
            case BinaryOperatorKind.And:                // fall through
            case BinaryOperatorKind.Equal:              // fall through
            case BinaryOperatorKind.NotEqual:           // fall through
            case BinaryOperatorKind.GreaterThan:        // fall through
            case BinaryOperatorKind.GreaterThanOrEqual: // fall through
            case BinaryOperatorKind.LessThan:           // fall through
            case BinaryOperatorKind.LessThanOrEqual:
            case BinaryOperatorKind.Has:
                return isNullable ? typeof(bool?) : typeof(bool);

            case BinaryOperatorKind.Add:        // fall through
            case BinaryOperatorKind.Subtract:   // fall through
            case BinaryOperatorKind.Multiply:   // fall through
            case BinaryOperatorKind.Divide:     // fall through
            case BinaryOperatorKind.Modulo:
            default:
                return left.NodeType;

           // default:
            //    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath));
        }
    }
}