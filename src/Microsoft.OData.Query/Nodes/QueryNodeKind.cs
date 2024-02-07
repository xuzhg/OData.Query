//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Public enumeration of kinds of query nodes.
/// </summary>
public enum QueryNodeKind
{
    /// <summary>
    /// No query node kind...  the default value.
    /// </summary>
    None,

    /// <summary>
    /// Node used to represent the OData query.
    /// </summary>
    ODataQuery,

    /// <summary>
    /// A constant value.
    /// </summary>
    Constant,

    /// <summary>
    /// A node that represents conversion from one type to another.
    /// </summary>
    Convert,

    /// <summary>
    /// Node used to represent a binary operator.
    /// </summary>
    BinaryOperator,

    /// <summary>
    /// Node used to represent a unary operator.
    /// </summary>
    UnaryOperator,

    /// <summary>
    /// All query.
    /// </summary>
    All,

    /// <summary>
    /// Any query.
    /// </summary>
    Any,

    /// <summary>
    /// In operator node.
    /// </summary>
    In,

    /// <summary>
    /// Node describing count of a collection contains primitive or enum or complex or entity type.
    /// </summary>
    DollarCount,

    /// <summary>
    /// Node describing access to a property which is a single (non-collection) non-entity value.
    /// </summary>
    SingleValuePropertyAccess,

    /// <summary>
    /// Node describing access to a property which is a non-entity collection value.
    /// </summary>
    CollectionValuePropertyAccess,

    /// <summary>
    /// Function call returning a single value.
    /// </summary>
    SingleValueFunctionCall,

    /// <summary>
    /// Resource node referencing a range variable.
    /// </summary>
    ResourceRangeVariableReference
}
