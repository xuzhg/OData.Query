//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public struct ExpressionToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionToken" /> class.
    /// </summary>
    /// <param name="kind">The Expression Kind.</param>
    /// <param name="position">The Expression Token starting position.</param>
    public ExpressionToken(ExpressionKind kind, int position)
        : this(kind, ReadOnlyMemory<char>.Empty, position)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionToken" /> class.
    /// </summary>
    /// <param name="kind">The Expression Kind.</param>
    /// <param name="text">The Expression Token Text.</param>
    /// <param name="position">The Expression Token starting position.</param>
    public ExpressionToken(ExpressionKind kind, ReadOnlyMemory<char> text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    /// <summary>
    /// Expression Token kind.
    /// </summary>
    public ExpressionKind Kind;

    /// <summary>
    /// Expression Token text.
    /// </summary>
    public ReadOnlyMemory<char> Text;

    /// <summary>
    /// Starting Position of expression token.
    /// </summary>
    public int Position;

    /// <summary>
    /// Gets the token text as <see cref="ReadOnlySpan{T}"/>
    /// Be noted, avoid calling 'ToString()' multiple times since it will create a new string everytime.
    /// </summary>
    public ReadOnlySpan<char> Span => Text.Span;

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