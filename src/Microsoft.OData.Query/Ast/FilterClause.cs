//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Ast;

/// <summary>
/// The result of parsing a $filter query option.
/// </summary>
public class FilterClause
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterClause" /> class.
    /// </summary>
    /// <param name="expression">The filter expression - this should evaluate to a single boolean value. Cannot be null.</param>
    /// <param name="rangeVariable">The parameter for the expression which represents a single value from the collection. Cannot be null.</param>
    /// <exception cref="System.ArgumentNullException">Throws if the input expression or rangeVariable is null.</exception>
    public FilterClause(SingleValueNode expression, RangeVariable rangeVariable)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        RangeVariable = rangeVariable ?? throw new ArgumentNullException(nameof(rangeVariable));
    }

    /// <summary>
    /// Gets the filter expression - this should evaluate to a single boolean value.
    /// </summary>
    public SingleValueNode Expression { get; }

    /// <summary>
    /// Gets the parameter for the expression which represents a single value from the collection.
    /// </summary>
    public RangeVariable RangeVariable { get; }
}