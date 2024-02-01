//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing an In operation.
/// </summary>
public class InToken : QueryToken
{
    /// <summary>
    /// Create a new InToken given the left and right query tokens.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public InToken(QueryToken left, QueryToken right)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.In;

    /// <summary>
    /// The left operand.
    /// </summary>
    public QueryToken Left { get; }

    /// <summary>
    /// The right operand.
    /// </summary>
    public QueryToken Right { get; }
}
