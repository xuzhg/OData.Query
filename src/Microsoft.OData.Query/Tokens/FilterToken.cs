//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $filter query.
    /// </summary>
    public class FilterToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterToken" /> class.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        public FilterToken(QueryToken filterExpression)
        {
            Expression = filterExpression ?? throw new ArgumentNullException(nameof(filterExpression));
        }

        /// <summary>
        /// The expression according to which to filter the results.
        /// </summary>
        public QueryToken Expression { get; }
    }
}
