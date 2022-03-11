//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing an unary operator.
    /// </summary>
    public sealed class UnaryOperatorToken : QueryToken
    {
        /// <summary>
        /// nitializes a new instance of the <see cref="UnaryOperatorToken" /> class.
        /// </summary>
        /// <param name="kind">The operator represented by this node.</param>
        /// <param name="operand">The operand.</param>
        public UnaryOperatorToken(UnaryOperatorKind kind, QueryToken operand)
        {
            OperatorKind = kind;
            Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        }

        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        public UnaryOperatorKind OperatorKind { get; }

        /// <summary>
        /// The operand.
        /// </summary>
        public QueryToken Operand { get; }
    }
}
