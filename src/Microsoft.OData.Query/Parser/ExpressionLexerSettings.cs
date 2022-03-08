//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser
{
    public class ExpressionLexerSettings
    {
        internal static ExpressionLexerSettings Default = new ExpressionLexerSettings();

        /// <summary>
        /// Lexer ignores whitespace
        /// </summary>
        public bool IgnoreWhitespace { get; set; }
    }
}
