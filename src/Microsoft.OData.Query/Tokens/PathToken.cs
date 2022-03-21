//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a path
    /// </summary>
    public class PathToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathToken" /> class.
        /// </summary>
        /// <param name="segments">The path segments expression.</param>
        public PathToken(IEnumerable<PathSegmentToken> segments)
        {
            Segments = segments ?? throw new ArgumentNullException(nameof(segments));
        }

        /// <summary>
        /// The properties according to which to select in the results.
        /// </summary>
        public IEnumerable<PathSegmentToken> Segments { get; set; }
    }
}
