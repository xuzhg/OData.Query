//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a binary operator.
/// </summary>
public class BinaryOperatorToken : QueryToken
{
    /// <summary>
    /// Create a new BinaryOperatorToken given the operator, left and right query.
    /// </summary>
    /// <param name="operatorKind">The operator represented by this node.</param>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public BinaryOperatorToken(BinaryOperatorKind operatorKind, QueryToken left, QueryToken right)
    {
        OperatorKind = operatorKind;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.BinaryOperator;

    /// <summary>
    /// The operator represented by this node.
    /// </summary>
    public BinaryOperatorKind OperatorKind { get; }

    /// <summary>
    /// The left operand.
    /// </summary>
    public QueryToken Left { get; }

    /// <summary>
    /// The right operand.
    /// </summary>
    public QueryToken Right { get; }
}
