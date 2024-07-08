//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// An item that has been computed by the query at the current level of the tree.
/// </summary>
public class ComputedItem
{
    /// <summary>
    /// Create a ComputeExpression.
    /// </summary>
    /// <param name="expression">The compute expression.</param>
    /// <param name="alias">The compute alias.</param>
    public ComputedItem(SingleValueNode expression, string alias, Type typeReference)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Alias = alias;
    }

    /// <summary>
    /// Gets the aggregation expression.
    /// </summary>
    public SingleValueNode Expression { get; }

    /// <summary>
    /// Gets the aggregation alias.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> of this computed expression.
    /// </summary>
    public Type ExpressionType { get; }
}
