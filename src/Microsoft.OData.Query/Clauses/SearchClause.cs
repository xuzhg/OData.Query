//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// The result of parsing a $search query option.
/// </summary>
public class SearchClause
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchClause" /> class.
    /// </summary>
    /// <param name="expression">The filter expression - this should evaluate to a single boolean value. Cannot be null.</param>
    public SearchClause(SingleValueNode expression)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    /// <summary>
    /// Gets the filter expression - this should evaluate to a single boolean value.
    /// </summary>
    public SingleValueNode Expression { get; }
}
