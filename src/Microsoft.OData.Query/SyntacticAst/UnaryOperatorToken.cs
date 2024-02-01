//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a unary operator.
/// </summary>
public class UnaryOperatorToken : QueryToken
{
    /// <summary>
    /// Create a new UnaryOperatorToken given the operator and operand
    /// </summary>
    /// <param name="operatorKind">The operator represented by this node.</param>
    /// <param name="operand">The operand.</param>
    public UnaryOperatorToken(UnaryOperatorKind operatorKind, QueryToken operand)
    {
        OperatorKind = operatorKind;
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.UnaryOperator;

    /// <summary>
    /// The operator represented by this node.
    /// </summary>
    public UnaryOperatorKind OperatorKind { get; }

    /// <summary>
    /// The operand.
    /// </summary>
    public QueryToken Operand { get; }
}
