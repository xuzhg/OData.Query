//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Lexer is a conversion of a text into meaningful lexical tokens.
/// </summary>
public interface IExpressionLexer
{
    /// <summary>
    /// Gets the whole expression source text to lexer.
    /// </summary>
    ReadOnlyMemory<char> ExpressionText { get; }

    /// <summary>
    /// Gets the token being processed.
    /// </summary>
    ExpressionToken CurrentToken { get; }

    /// <summary>
    /// Reads the next token, advancing the expression Lexer, refreshing the processed token.
    /// </summary>
    /// <returns>Boolean value indicating reading the next token succussed or not.</returns>
    bool NextToken();

    /// <summary>
    /// Try to peek next token.
    /// </summary>
    /// <param name="token">The output next token.</param>
    /// <returns>True if contains next token, false no next token.</returns>
    bool PeekNextToken(out ExpressionToken token);
}
