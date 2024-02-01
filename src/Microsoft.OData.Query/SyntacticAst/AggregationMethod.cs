//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Enumeration of methods used in the aggregation clause
/// </summary>
public enum AggregationMethod
{
    /// <summary>The aggregation method Sum.</summary>
    Sum,

    /// <summary>The aggregation method Min.</summary>
    Min,

    /// <summary>The aggregation method Max.</summary>
    Max,

    /// <summary>The aggregation method Average.</summary>
    Average,

    /// <summary>The aggregation method CountDistinct.</summary>
    CountDistinct,

    /// <summary>The aggregation method Count. Used only internally to represent the virtual property $count.</summary>
    VirtualPropertyCount,

    /// <summary>A custom aggregation method.</summary>
    Custom
}
