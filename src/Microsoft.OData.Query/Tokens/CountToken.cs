//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $count query.
    /// </summary>
    public class CountToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountToken" /> class.
        /// </summary>
        /// <param name="value">The $count value.</param>
        public CountToken(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// The top value.
        /// </summary>
        public bool Value { get; }
    }
}
