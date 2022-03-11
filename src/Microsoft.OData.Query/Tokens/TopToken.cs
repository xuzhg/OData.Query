//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $top query.
    /// </summary>
    public class TopToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TopToken" /> class.
        /// </summary>
        /// <param name="value">The top value.</param>
        public TopToken(long value)
        {
            Value = value;
        }

        /// <summary>
        /// The top value.
        /// </summary>
        public long Value { get; }
    }
}
