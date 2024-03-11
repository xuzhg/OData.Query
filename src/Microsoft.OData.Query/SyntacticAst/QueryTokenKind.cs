//-----------------------------------------------------------------------
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
    /// The unknown token.
    /// </summary>
    Unknown,

    /// <summary>
    /// The binary operator.
    /// </summary>
    BinaryOperator,

    /// <summary>
    /// The unary operator.
    /// </summary>
    UnaryOperator,

    /// <summary>
    /// The literal value.
    /// </summary>
    Literal,

    /// <summary>
    /// The function call.
    /// </summary>
    FunctionCall,

    /// <summary>
    /// The property access.
    /// </summary>
    EndPath,

    /// <summary>
    /// The order by operation.
    /// </summary>
    OrderBy,

    /// <summary>
    /// A query option.
    /// </summary>
    CustomQueryOption,

    /// <summary>
    /// The Select query.
    /// </summary>
    Select,

    /// <summary>
    /// The *.
    /// </summary>
    Star,

    /// <summary>
    /// The Expand query.
    /// </summary>
    Expand,

    /// <summary>
    /// Type segment.
    /// </summary>
    TypeSegment,

    /// <summary>
    /// Any query.
    /// </summary>
    Any,

    /// <summary>
    /// Non root segment.
    /// </summary>
    InnerPath,

    /// <summary>
    /// type segment.
    /// </summary>
    DottedIdentifier,

    /// <summary>
    /// Parameter token.
    /// </summary>
    RangeVariable,

    /// <summary>
    /// All query.
    /// </summary>
    All,

    /// <summary>
    /// FunctionParameterToken
    /// </summary>
    FunctionParameter,

    /// <summary>
    /// FunctionParameterAlias
    /// </summary>
    FunctionParameterAlias,

    /// <summary>
    /// the string literal for search query
    /// </summary>
    StringLiteral,

    /// <summary>
    /// $apply aggregate token
    /// </summary>
    Aggregate,

    /// <summary>
    /// $apply aggregate statement to a property token
    /// </summary>
    AggregateExpression,

    /// <summary>
    /// $apply group by token
    /// </summary>
    AggregateGroupBy,

    /// <summary>
    /// $compute token
    /// </summary>
    Compute,

    /// <summary>
    /// $compute item token
    /// </summary>
    ComputeItem,

    /// <summary>
    /// $apply aggregate statement to a entity set token
    /// </summary>
    EntitySetAggregateExpression,

    /// <summary>
    /// In operator.
    /// </summary>
    In,

    /// <summary>
    /// SelectItem Token
    /// </summary>
    SelectItem,

    /// <summary>
    /// ExpandItem Token
    /// </summary>
    ExpandItem,

    /// <summary>
    /// $count segment
    /// </summary>
    CountSegment,

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