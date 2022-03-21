//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a $apply query.
    /// </summary>
    public class ApplyToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyToken" /> class.
        /// </summary>
        /// <param name="transformTokens">The apply transform tokens.</param>
        public ApplyToken(IEnumerable<QueryToken> transformTokens)
        {
            TransformTokens = transformTokens ?? throw new ArgumentNullException(nameof(transformTokens));
        }

        /// <summary>
        /// The expression according to which to apply the results.
        /// </summary>
        public IEnumerable<QueryToken> TransformTokens { get; }
    }
}
