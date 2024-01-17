//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public ref struct OToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OToken" /> class.
    /// </summary>
    /// <param name="kind">The OTokenKind.</param>
    /// <param name="text">The Token Text.</param>
    /// <param name="position">The Token starting positing.</param>
    public OToken(OTokenKind kind, ReadOnlySpan<char> text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    /// <summary>
    /// Token kind.
    /// </summary>
    public OTokenKind Kind;

    /// <summary>
    /// Token text.
    /// </summary>
    public ReadOnlySpan<char> Text;

    /// <summary>
    /// Starting Position of token.
    /// </summary>
    public int Position;

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
