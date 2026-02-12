//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Extension methods for <see cref="IExpressionLexer"/>.
/// </summary>
public static class LexerExtensions
{
    /// <summary>
    /// Checks that the current token has the specified identifier.
    /// </summary>
    /// <param name="id">Identifier to check.</param>
    /// <returns>true if the current token is an identifier with the specified text.</returns>
    public static bool IsCurrentTokenIdentifier(this IExpressionLexer lexer, string id)
        => lexer.IsCurrentTokenIdentifier(id, false);

    /// <summary>
    /// Checks that the current token has the specified identifier.
    /// </summary>
    /// <param name="id">Identifier to check.</param>
    /// <param name="enableCaseSensitive">bool value to enable case sensitive.</param>
    /// <returns>true if the current token is an identifier with the specified text.</returns>
    public static bool IsCurrentTokenIdentifier(this IExpressionLexer lexer, string id, bool enableCaseInsensitive)
    {
        if (lexer == null)
        {
            throw new ArgumentNullException(nameof(lexer));
        }

        ExpressionToken token = lexer.CurrentToken;

        return token.Kind == ExpressionKind.Identifier
            && token.Span.Equals(id, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
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
    /// Starting from an identifier, reads a sequence of dots and
    /// identifiers, and returns the text for it, with whitespace
    /// stripped.
    /// </summary>
    /// <param name="acceptStar">do we allow a star in this identifier</param>
    /// <returns>The dotted identifier starting at the current identifier.</returns>
    public static ReadOnlyMemory<char> ReadDottedIdentifier(this IExpressionLexer lexer, bool acceptStar)
    {
        if (lexer == null)
        {
            throw new ArgumentNullException(nameof(lexer));
        }

        int position = lexer.CurrentToken.Position;

        if (lexer.CurrentToken.Kind != ExpressionKind.Identifier)
        {
            throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_ExpressionKindExpected, "Identifier", position, lexer.CurrentToken.Kind));
        }

        int length = lexer.CurrentToken.Text.Length;
        int oldLength = length;

        ReadOnlyMemory<char> identifier = lexer.CurrentToken.Text;

        lexer.NextToken(); // move over the identifier
        while (lexer.CurrentToken.Kind == ExpressionKind.Dot)
        {
            lexer.NextToken(); // consume the dot

            if (lexer.CurrentToken.Kind != ExpressionKind.Identifier &&
                lexer.CurrentToken.Kind != ExpressionKind.QuotedLiteral)
            {
                if (lexer.CurrentToken.Kind == ExpressionKind.Star)
                {
                    bool hasNextToken = lexer.PeekNextToken(out ExpressionToken peekedToken);

                    // if we accept a star and this is the last token in the identifier, then we're ok... otherwise we throw.
                    if (!acceptStar || (hasNextToken && peekedToken.Kind != ExpressionKind.EndOfInput && peekedToken.Kind != ExpressionKind.Comma))
                    {
                        throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_ExpressionKindExpected, "EndOfInput|Comma", peekedToken.Position, peekedToken.Kind));
                    }
                }
                else
                {
                    throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_ExpressionKindExpected, "Identifier|QuotedLiteral", lexer.CurrentToken.Position, lexer.CurrentToken.Kind));
                }
            }

            length += 1 + lexer.CurrentToken.Text.Length; // one '.' and next current token length
            lexer.NextToken();
        }

        if (oldLength != length)
        {
            return lexer.ExpressionText.Slice(position, length);
        }

        return identifier;
    }

    /// <summary>
    /// Gets the current identifier text.
    /// </summary>
    /// <returns>The current identifier text.</returns>
    public static ReadOnlySpan<char> GetIdentifier(this IExpressionLexer lexer)
    {
        lexer.ValidateToken(ExpressionKind.Identifier);
        return lexer.CurrentToken.Span;
    }
}
