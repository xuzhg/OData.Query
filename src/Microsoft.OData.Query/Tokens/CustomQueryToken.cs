//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a customized (non-odata built in) query option.
    /// </summary>
    public class CustomQueryToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomQueryToken" /> class.
        /// </summary>
        /// <param name="name">The name of the query option.</param>
        /// <param name="value">The value of the query option.</param>
        public CustomQueryToken(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The name of the query option.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the query option.
        /// </summary>
        public string Value { get; }
    }
}
