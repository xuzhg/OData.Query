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

        /// <summary>
        /// flag to indicate whether to delimit on a semicolon.
        /// The query within $select or $expand are separated using ';'.
        /// </summary>
        public bool UseSemicolonDelimiter { get; set; } = false;

        /// <summary>
        /// Whether the lexer is being used to parse function parameters.
        /// If true, will allow/recognize parameter aliases and typed nulls.
        /// </summary>
        public bool ParsingFunctionParameters { get; set; } = false;
    }
}
