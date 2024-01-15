//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public struct OToken
{
    /// <summary>
    /// Token kind.
    /// </summary>
    public OTokenKind Kind;

    /// <summary>
    /// Token text.
    /// </summary>
    public string Text;
    // public ReadOnlySpan<char> Text;

    /// <summary>
    /// Starting Position of token.
    /// </summary>
    public int Position;

    /// <summary>
    /// Reset the token fields.
    /// </summary>
    /// <param name="kind">The new TokenKind.</param>
    /// <param name="text">The new Token Text, it can be 'null'.</param>
    /// <param name="position">The new Token starting positing.</param>
    internal void Reset(OTokenKind kind, string text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    public override string ToString()
    {
        if (Text == null)
        {
            return $"{Kind} at {Position}";
        }

        return $"{Kind}, \"{Text}\" | ({Text.Length}), at {Position}";
    }
}
