//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// An item that has been computed by the query at the current level of the tree.
/// </summary>
public sealed class ComputeItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeItem" /> class.
    /// </summary>
    /// <param name="expression">The compute expression.</param>
    /// <param name="alias">The compute alias.</param>
    /// <param name="type">The <see cref="Type"/> of this aggregate expression.</param>
    public ComputeItem(SingleValueNode expression, string alias, Type type)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Alias = alias ?? throw new ArgumentNullException(nameof(alias));

        //// TypeReference is null for dynamic properties
        TypeReference = type;
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
    /// Gets the <see cref="Type"/> of this aggregate expression.
    /// </summary>
    public Type TypeReference { get; }
}
