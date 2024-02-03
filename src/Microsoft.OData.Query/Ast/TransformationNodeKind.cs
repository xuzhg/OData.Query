//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Ast;

/// <summary>
/// Enumeration of kinds of transformation nodes.
/// </summary>
public enum TransformationNodeKind
{
    /// <summary>
    /// None
    /// </summary>
    None,

    /// <summary>
    /// Aggregations of values
    /// </summary>
    Aggregate,

    /// <summary>
    /// A grouping of values by properties
    /// </summary>
    GroupBy,

    /// <summary>
    /// A filter clause
    /// </summary>
    Filter,

    /// <summary>
    /// A Compute expressions
    /// </summary>
    Compute,

    /// <summary>
    /// A Expand expressions
    /// </summary>
    Expand,
}
