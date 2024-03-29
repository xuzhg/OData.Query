﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Enumeration of kinds of query tokens.
/// </summary>
public enum QueryTokenKind
{
    /// <summary>
    /// The binary operator.
    /// </summary>
    BinaryOperator = 3,

    /// <summary>
    /// The unary operator.
    /// </summary>
    UnaryOperator = 4,

    /// <summary>
    /// The literal value.
    /// </summary>
    Literal = 5,

    /// <summary>
    /// The function call.
    /// </summary>
    FunctionCall = 6,

    /// <summary>
    /// The property access.
    /// </summary>
    EndPath = 7,

    /// <summary>
    /// The order by operation.
    /// </summary>
    OrderBy = 8,

    /// <summary>
    /// A query option.
    /// </summary>
    CustomQueryOption = 9,

    /// <summary>
    /// The Select query.
    /// </summary>
    Select = 10,

    /// <summary>
    /// The *.
    /// </summary>
    Star = 11,

    /// <summary>
    /// The Expand query.
    /// </summary>
    Expand = 13,

    /// <summary>
    /// Type segment.
    /// </summary>
    TypeSegment = 14,

    /// <summary>
    /// Any query.
    /// </summary>
    Any = 15,

    /// <summary>
    /// Non root segment.
    /// </summary>
    InnerPath = 16,

    /// <summary>
    /// type segment.
    /// </summary>
    DottedIdentifier = 17,

    /// <summary>
    /// Parameter token.
    /// </summary>
    RangeVariable = 18,

    /// <summary>
    /// All query.
    /// </summary>
    All = 19,

    /// <summary>
    /// ExpandTerm Token
    /// </summary>
    ExpandTerm = 20,

    /// <summary>
    /// FunctionParameterToken
    /// </summary>
    FunctionParameter = 21,

    /// <summary>
    /// FunctionParameterAlias
    /// </summary>
    FunctionParameterAlias = 22,

    /// <summary>
    /// the string literal for search query
    /// </summary>
    StringLiteral = 23,

    /// <summary>
    /// $apply aggregate token
    /// </summary>
    Aggregate = 24,

    /// <summary>
    /// $apply aggregate statement to a property token
    /// </summary>
    AggregateExpression = 25,

    /// <summary>
    /// $apply group by token
    /// </summary>
    AggregateGroupBy = 26,

    /// <summary>
    /// $compute token
    /// </summary>
    Compute = 27,

    /// <summary>
    /// $compute item token
    /// </summary>
    ComputeItem = 28,

    /// <summary>
    /// $apply aggregate statement to a entity set token
    /// </summary>
    EntitySetAggregateExpression = 29,

    /// <summary>
    /// In operator.
    /// </summary>
    In = 30,

    /// <summary>
    /// SelectItem Token
    /// </summary>
    SelectItem = 31,

    /// <summary>
    /// $count segment
    /// </summary>
    CountSegment = 32,

    /// <summary>
    /// Segment in $select, $expand,...
    /// </summary>
    PathSegment,

    Top,

    Skip,

    Search,
    Apply,
    SkipToken,
    DeltaToken,
}