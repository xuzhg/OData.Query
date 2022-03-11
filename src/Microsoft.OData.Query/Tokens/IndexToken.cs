//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $index query.
    /// </summary>
    public class IndexToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexToken" /> class.
        /// </summary>
        /// <param name="value">The index value.</param>
        public IndexToken(long value)
        {
            Value = value;
        }

        /// <summary>
        /// The index value.
        /// </summary>
        public long Value { get; }
    }
}
