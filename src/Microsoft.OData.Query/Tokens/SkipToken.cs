//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $skip query.
    /// </summary>
    public class SkipToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkipToken" /> class.
        /// </summary>
        /// <param name="value">The skip value.</param>
        public SkipToken(long value)
        {
            Value = value;
        }

        /// <summary>
        /// The skip value.
        /// </summary>
        public long Value { get; }
    }
}
