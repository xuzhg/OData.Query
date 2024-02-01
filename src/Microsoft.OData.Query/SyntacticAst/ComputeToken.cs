//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing an Compute token.
/// </summary>
public sealed class ComputeToken : ApplyTransformationToken
{
    private readonly IEnumerable<ComputeExpressionToken> _expressions;

    /// <summary>
    /// Create an ComputeToken.
    /// </summary>
    /// <param name="expressions">The list of ComputeExpressionToken.</param>
    public ComputeToken(IEnumerable<ComputeExpressionToken> expressions)
    {
        _expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));
    }

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Compute;

    /// <summary>
    /// Gets the list of ComputeExpressionToken.
    /// </summary>
    public IEnumerable<ComputeExpressionToken> Expressions => _expressions;
}
