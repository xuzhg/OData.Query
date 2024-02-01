//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing an Aggregate token.
/// </summary>
public sealed class AggregateToken : ApplyTransformationToken
{
    private readonly IEnumerable<AggregateTokenBase> _expressions;

    /// <summary>
    /// Create an AggregateTransformationToken.
    /// </summary>
    /// <param name="expressions">The aggregate expressions.</param>
    public AggregateToken(IEnumerable<AggregateTokenBase> expressions)
    {
        _expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
    }

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Aggregate;

    /// <summary>
    /// Gets the expressions of this token.
    /// </summary>
    public IEnumerable<AggregateTokenBase> AggregateExpressions => _expressions;
}
