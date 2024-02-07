//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Extension method for <see cref="IExpressionLexer"/>.
/// </summary>
public static class ILexerExtensions
{
    /// <summary>
    /// Checks that the current token has the specified identifier.
    /// </summary>
    /// <param name="id">Identifier to check.</param>
    /// <returns>true if the current token is an identifier with the specified text.</returns>
    public static bool IsCurrentTokenIdentifier(this IExpressionLexer lexer, string id)
        => lexer.IsCurrentTokenIdentifier(id, true);

    /// <summary>
    /// Checks that the current token has the specified identifier.
    /// </summary>
    /// <param name="id">Identifier to check.</param>
    /// <param name="enableCaseSensitive">bool value to enable case sensitive.</param>
    /// <returns>true if the current token is an identifier with the specified text.</returns>
    public static bool IsCurrentTokenIdentifier(this IExpressionLexer lexer, string id, bool enableCaseSensitive)
    {
        if (lexer == null)
        {
            throw new ArgumentNullException(nameof(lexer));
        }

        ExpressionToken token = lexer.CurrentToken;

        return token.Kind == ExpressionKind.Identifier
            && token.Text.Equals(id, enableCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates the current token is of the specified kind.
    /// </summary>
    /// <param name="kind">Expected token kind.</param>
    public static void ValidateToken(this IExpressionLexer lexer, ExpressionKind kind)
    {
        if (lexer == null)
        {
            throw new ArgumentNullException(nameof(lexer));
        }

        if (lexer.CurrentToken.Kind != kind)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_TokenKindExpected, kind, lexer.CurrentToken.Kind));
        }
    }

    /// <summary>
    /// Gets the current identifier text.
    /// </summary>
    /// <returns>The current identifier text.</returns>
    public static ReadOnlySpan<char> GetIdentifier(this IExpressionLexer lexer)
    {
        lexer.ValidateToken(ExpressionKind.Identifier);
        return lexer.CurrentToken.Text;
    }
}
