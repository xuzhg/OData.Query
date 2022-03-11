//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// exical token representing a $search query.
    /// </summary>
    public class SearchToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchToken" /> class.
        /// </summary>
        /// <param name="searchExpression">The search expression.</param>
        public SearchToken(QueryToken searchExpression)
        {
            Expression = searchExpression ?? throw new ArgumentNullException(nameof(searchExpression));
        }

        /// <summary>
        /// The expression according to which to search the results.
        /// </summary>
        public QueryToken Expression { get; }
    }
}
