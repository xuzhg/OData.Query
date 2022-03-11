//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing the 'in' Query.
    /// </summary>
    public sealed class InToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InToken" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public InToken(QueryToken left, QueryToken right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

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
