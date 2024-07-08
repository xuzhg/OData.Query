//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Paths;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// This represents one level of expansion for a particular expansion tree.
/// $expand=Nav
/// </summary>
public class ExpandedNavigationItem : ExpandedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandedNavigationItem" /> class.
    /// </summary>
    /// <param name="expandPath"></param>
    public ExpandedNavigationItem(ExpandPathItem expandPath)
        : this(expandPath, null, null)
    {
    }

    public ExpandedNavigationItem(ExpandPathItem expandPath, SelectClause select)
    : this(expandPath, null, null)
    {
    }

    public ExpandedNavigationItem(ExpandPathItem expandPath, ExpandClause expand)
        : this(expandPath, null, null)
    {
    }

    public ExpandedNavigationItem(ExpandPathItem expandPath, SelectClause select, ExpandClause expand)
        : this(expandPath, null, null, select, expand, null, null, null, null, null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandedNavigationItem" /> class.
    /// </summary>
    public ExpandedNavigationItem(
            ExpandPathItem expandedPath,
            ApplyClause apply,
            ComputeClause compute,
            SelectClause select,
            ExpandClause expand,
            FilterClause filter,
            OrderByClause orderBy,
            ConstantLongNode top,
            ConstantLongNode skip,
            ConstantBoolNode count,
            SearchClause search,
            LevelsClause levels)
           : base(expandedPath, apply, compute, filter, orderBy, top, skip, count, search)
    {
        Select = select;
        Expand = expand;
        Levels = levels;
    }

    /// <summary>
    /// The select clause for this expanded navigation.
    /// </summary>
    public SelectClause Select { get; }

    /// <summary>
    /// The expand clause for this expanded navigation.
    /// </summary>
    public ExpandClause Expand { get; }

    /// <summary>
    /// Gets the levels clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
    /// </summary>
    public LevelsClause Levels { get; }
}
