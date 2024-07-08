//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Paths;

namespace Microsoft.OData.Query.Clauses;

public sealed class SelectedPropertyItem : SelectedItem
{
    // $select=../../Property
    public SelectedPropertyItem(SelectPathItem selectedPath)
        : this(selectedPath, null, null, null, null, null, null, null, null, null)
    {
    }

    public SelectedPropertyItem(SelectPathItem selectedPath,
        SelectClause select,
        ExpandClause expand,
        FilterClause filter,
        OrderByClause orderBy,
        ConstantLongNode top,
        ConstantLongNode skip,
        ConstantBoolNode count,
        SearchClause search,
        ComputeClause compute)
    {
        SelectedPath = selectedPath ?? throw new ArgumentNullException(nameof(selectedPath));

        Select = select;
        Expand = expand;
        Filter = filter;
        OrderBy = orderBy;
        Top = top;
        Skip = skip;
        Count = count;
        Search = search;
        Compute = compute;
    }

    /// <summary>
    /// Gets the Path for this select level.
    /// </summary>
    public SelectPathItem SelectedPath { get; }

    public SelectClause Select { get; }

    public ExpandClause Expand { get; }

    public ComputeClause Compute { get; }

    public FilterClause Filter { get; }

    public SearchClause Search { get; }

    public OrderByClause OrderBy { get; }

    public ConstantLongNode Top { get; }

    public ConstantLongNode Skip { get; }

    public ConstantBoolNode Count { get; }
}