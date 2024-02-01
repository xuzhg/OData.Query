//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Parser;
using Microsoft.VisualBasic;
using System.Data;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Extension method for <see cref="IOTokenizer"/>.
/// </summary>
public static class IOTokenizerExtensions
{
    /// <summary>
    /// Gets the current Token in this tokenization lexer.
    /// </summary>
    /// <remarks>
    /// It retrieves the characters from span and create the string.
    /// </remarks>
    //public static OTokenWithValue GetCurrentToken(this IOTokenizer tokenizer)
    //{
    //    if (tokenizer == null)
    //    {
    //        throw new ArgumentNullException(nameof(tokenizer));
    //    }

    //    OToken token = tokenizer.CurrentToken;
    //    return token;
    //}

    /// <summary>
    /// Checks that the current token has the specified identifier.
    /// </summary>
    /// <param name="id">Identifier to check.</param>
    /// <returns>true if the current token is an identifier with the specified text.</returns>
    public static bool IsCurrentTokenIdentifier(this IOTokenizer tokenizer, string id, bool enableCaseInsensitive)
    {
        if (tokenizer == null)
        {
            throw new ArgumentNullException(nameof(tokenizer));
        }

        OToken token = tokenizer.CurrentToken;

        return token.Kind == OTokenKind.Identifier
            && token.Text.Equals(id, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    /// <summary>
    /// Validates the current token is of the specified kind.
    /// </summary>
    /// <param name="kind">Expected token kind.</param>
    public static void ValidateToken(this IOTokenizer tokenizer, OTokenKind kind)
    {
        if (tokenizer == null)
        {
            throw new ArgumentNullException(nameof(tokenizer));
        }

        if (tokenizer.CurrentToken.Kind != kind)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_TokenKindExpected, kind, tokenizer.CurrentToken.Kind));
        }
    }

    /// <summary>
    /// Gets the current identifier text.
    /// </summary>
    /// <returns>The current identifier text.</returns>
    public static ReadOnlySpan<char> GetIdentifier(this IOTokenizer tokenizer)
    {
        tokenizer.ValidateToken(OTokenKind.Identifier);
        return tokenizer.CurrentToken.Text;
    }
}
