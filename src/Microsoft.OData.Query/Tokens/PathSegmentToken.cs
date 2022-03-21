//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a path.
    /// </summary>
    public class PathSegmentToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegmentToken" /> class.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="next"></param>
        public PathSegmentToken(string identifier, PathSegmentToken next = null)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Next = next;
        }

        /// <summary>
        /// The name of the property to access.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Get the NextToken in the path
        /// </summary>
        public PathSegmentToken Next { get; }
    }
}
