//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Paths;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// An item that has been expanded by the query at the current level of the tree.
/// </summary>
public abstract class ExpandedItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="ExpandedItem"/> class.
    /// </summary>
    /// <param name="expandPath"></param>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="search"></param>
    /// <param name="compute"></param>
    /// <param name="apply"></param>
    protected ExpandedItem(ExpandPathItem expandPath,
        ApplyClause apply = null,
        ComputeClause compute = null,
        FilterClause filter = null,
        OrderByClause orderBy = null,
        ConstantLongNode top = null,
        ConstantLongNode skip = null,
        ConstantBoolNode count = null,
        SearchClause search = null)
    {
        ExpandPath = expandPath ?? throw new ArgumentNullException(nameof(expandPath));
        Apply = apply;
        Compute = compute;
        Filter = filter;
        OrderBy = orderBy;
        Top = top;
        Skip = skip;
        Count = count;
        Search = search;
    }

    /// <summary>
    /// Gets the Path for this expand level.
    /// </summary>
    public ExpandPathItem ExpandPath { get; }

    public ApplyClause Apply { get; }

    public ComputeClause Compute { get; }

    /// <summary>
    /// Gets the filter clause for this expand item.
    /// </summary>
    public FilterClause Filter { get; }

    public SearchClause Search { get; }

    public OrderByClause OrderBy { get; }

    public ConstantLongNode Top { get; }

    public ConstantLongNode Skip { get; }

    public ConstantBoolNode Count { get; }
}
