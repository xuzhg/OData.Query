//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing an order by operation.
/// </summary>
public sealed class OrderByToken : IQueryToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByToken" /> class.
    /// </summary>
    /// <param name="expression">The expression according to which to order the results.</param>
    /// <param name="direction">The direction of the ordering.</param>
    /// <param name="thenBy">The next orderby to perform after performing this orderby, can be null in the case of only a single orderby expression.</param>
    public OrderByToken(IQueryToken expression, OrderByDirection direction)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Direction = direction;
  //      ThenBy = thenBy;
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.OrderBy;

    /// <summary>
    /// The direction of the ordering.
    /// </summary>
    public OrderByDirection Direction { get; }

    /// <summary>
    /// The expression according to which to order the results.
    /// </summary>
    public IQueryToken Expression { get; }

    /// <summary>
    /// Gets/sets the next orderby token.
    /// </summary>
    public OrderByToken ThenBy { get; set; }
}