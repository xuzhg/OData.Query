//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// The factory to create <see cref="IExpressionLexer"/>.
/// </summary>
public interface ILexerFactory
{
    /// <summary>
    /// Create the <see cref="IExpressionLexer"/>.
    /// </summary>
    /// <param name="text">The expression text.</param>
    /// <param name="options">The lexer options.</param>
    /// <returns>The created expression lexer.</returns>
    IExpressionLexer CreateLexer(ReadOnlyMemory<char> text, LexerOptions options);
}
