//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $expand query.
    /// $expand=Orders,Customers($select=Street)
    /// </summary>
    public class ExpandToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandToken" /> class.
        /// </summary>
        /// <param name="items">The expand items.</param>
        public ExpandToken(IEnumerable<ExpandItemToken> items)
        {
            ExpandItems = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        public IEnumerable<ExpandItemToken> ExpandItems { get; }
    }
}
