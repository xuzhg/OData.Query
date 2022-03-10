//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser
{
    public interface IExpressionLexer
    {
        /// <summary>
        /// Token being processed.
        /// </summary>
        ExpressionToken Token { get; }

        /// <summary>
        /// Reads the next token, skipping whitespace as necessary.
        /// Advancing the lexer.
        /// </summary>
        /// <returns></returns>
        bool Next();
    }
}
