//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $select query.
    /// $select=Name,Age,Address($select=Street)
    /// </summary>
    public class SelectToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectToken" /> class.
        /// </summary>
        /// <param name="items">The select expression.</param>
        public SelectToken(IEnumerable<SelectItemToken> items)
        {
            SelectItems = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// The properties according to which to select in the results.
        /// </summary>
        public IEnumerable<SelectItemToken> SelectItems { get; set; }

    }
}
