//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public ref struct ExpressionToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionToken" /> class.
    /// </summary>
    /// <param name="kind">The ExpressionKind.</param>
    /// <param name="text">The Expression Token Text.</param>
    /// <param name="position">The Expression Token starting positing.</param>
    public ExpressionToken(ExpressionKind kind, ReadOnlySpan<char> text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    /// <summary>
    /// Expression Token kind.
    /// </summary>
    public ExpressionKind Kind { get; }

    /// <summary>
    /// Expression Token text.
    /// </summary>
    public ReadOnlySpan<char> Text { get; }

    /// <summary>
    /// Starting Position of expression token.
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
