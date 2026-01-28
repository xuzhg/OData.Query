//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing a compute item expression token.
/// </summary>
public sealed class ComputeItemToken : IQueryToken
{
    /// <summary>
    /// Create an ComputeExpressionToken.
    /// </summary>
    /// <param name="expression">The computation token.</param>
    /// <param name="alias">The alias for the computation.</param>
    public ComputeItemToken(IQueryToken expression, string alias)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));

        if (string.IsNullOrEmpty(alias))
        {
            throw new ArgumentNullException(nameof(alias));
        }

        Alias = alias;
    }

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.ComputeItem;

    /// <summary>
    /// Gets the QueryToken.
    /// </summary>
    public IQueryToken Expression { get; }

    /// <summary>
    /// Gets the alias of the computation.
    /// </summary>
    public string Alias { get; }
}
