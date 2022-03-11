//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a binary Query.
    /// </summary>
    public sealed class BinaryOperatorToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperatorToken" /> class.
        /// </summary>
        /// <param name="kind">The operator represented by this node.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        public BinaryOperatorToken(BinaryOperatorKind kind, QueryToken left, QueryToken right)
        {
            OperatorKind = kind;
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

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
}
