//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokens
{
    /// <summary>
    /// Lexical token representing a string literal value.
    /// </summary>
    public sealed class StringLiteralToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLiteralToken" /> class.
        /// </summary>
        /// <param name="text">The text value for this tokene.</param>
        public StringLiteralToken(string text)
        {
            Text = text;
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        public string Text { get; }
    }
}
