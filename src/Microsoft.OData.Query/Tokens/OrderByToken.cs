//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing an order by operation.
    /// </summary>
    public sealed class OrderByToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByToken" /> class.
        /// </summary>
        /// <param name="expression">The expression according to which to order the results.</param>
        /// <param name="direction">The direction of the ordering.</param>
        public OrderByToken(QueryToken expression, OrderByDirection direction)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Direction = direction;
        }

        /// <summary>
        /// The direction of the ordering.
        /// </summary>
        public OrderByDirection Direction { get; }

        /// <summary>
        /// The expression according to which to order the results.
        /// </summary>
        public QueryToken Expression { get; }
    }
}
