//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Enumeration of binary operators.
/// </summary>
public enum BinaryOperatorKind
{
    /// <summary>
    /// The logical or operator.
    /// </summary>
    Or,

    /// <summary>
    /// The logical and operator.
    /// </summary>
    And,

    /// <summary>
    /// The eq operator.
    /// </summary>
    Equal,

    /// <summary>
    /// The ne operator.
    /// </summary>
    NotEqual,

    /// <summary>
    /// The gt operator.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The ge operator.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// The lt operator.
    /// </summary>
    LessThan,

    /// <summary>
    /// The le operator.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// The add operator.
    /// </summary>
    Add,

    /// <summary>
    /// The sub operator.
    /// </summary>
    Subtract,

    /// <summary>
    /// The mul operator.
    /// </summary>
    Multiply,

    /// <summary>
    /// The div operator.
    /// </summary>
    Divide,

    /// <summary>
    /// The mod operator.
    /// </summary>
    Modulo,

    /// <summary>
    /// The has operator.
    /// </summary>
    Has,

    /// <summary>
    /// The customized operator for extend.
    /// </summary>
    Customized
}
