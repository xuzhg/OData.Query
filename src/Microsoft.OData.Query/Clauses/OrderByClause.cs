//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Clauses;

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