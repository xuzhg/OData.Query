//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public ref struct ExpressionToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OToken" /> class.
    /// </summary>
    /// <param name="kind">The OTokenKind.</param>
    /// <param name="text">The Token Text.</param>
    /// <param name="position">The Token starting positing.</param>
    public ExpressionToken(ExpressionKind kind, ReadOnlySpan<char> text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    /// <summary>
    /// Token kind.
    /// </summary>
    public ExpressionKind Kind { get; }

    /// <summary>
    /// Token text.
    /// </summary>
    public ReadOnlySpan<char> Text { get; }

    /// <summary>
    /// Starting Position of token.
    /// </summary>
    public int Position { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        if (Text.IsEmpty)
        {
            return $"{Kind} at {Position}";
        }

        return $"{Kind}, \"{Text}\" | ({Text.Length}), at {Position}";
    }
}
