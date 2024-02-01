//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Enumeration of binary operators.
/// </summary>
public enum UnaryOperatorKind
{
    /// <summary>
    /// The unary - operator.
    /// </summary>
    Negate = 0,

    /// <summary>
    /// The not operator.
    /// </summary>
    Not = 1,
}
