//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query;

public class SearchClause { }
public class SelectClause { }
public class ExpandClause { }
public class OrderByClause
{
    public OrderByClause(SingleValueNode expression, OrderByDirection direction, RangeVariable rangeVariable)
    {
        Expression = expression;
        Direction = direction;
        RangeVariable = rangeVariable;
    }
    /// <summary>
    /// Gets the next orderby to perform after performing this orderby, can be null in the case of only a single orderby expression.
    /// </summary>
    public OrderByClause ThenBy { get; set; }

    /// <summary>
    /// Gets the order-by expression.
    /// </summary>
    public SingleValueNode Expression { get; }

    /// <summary>
    /// Gets the direction to order.
    /// </summary>
    public OrderByDirection Direction { get; }

    /// <summary>
    /// Gets the rangeVariable for the expression which represents a single value from the collection we iterate over.
    /// </summary>
    public RangeVariable RangeVariable { get; }
}

public class ComputeClause { }

public class ODataQueryOption
{
    public ApplyClause Apply { get; set; }

    public FilterClause Filter { get; set; }

    public SearchClause Search { get; set; }

    public SelectClause Select { get; set; }

    public ExpandClause Expand { get; set; }

    public ComputeClause Compute { get; set; }

    public OrderByClause OrderBy { get; set; }
}