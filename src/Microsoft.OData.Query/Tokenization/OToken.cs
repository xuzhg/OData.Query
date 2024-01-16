//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

public ref struct OToken1 {


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
}

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public struct OToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OToken" /> class.
    /// </summary>
    /// <param name="kind">The OTokenKind.</param>
    /// <param name="text">The Token Text, it can be 'null'.</param>
    /// <param name="position">The Token starting positing.</param>
    public OToken(OTokenKind kind, string text, int position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OToken" /> class.
    /// </summary>
    /// <param name="kind">The OTokenKind.</param>
    /// <param name="text">The Token Text, it can be empty.</param>
    /// <param name="position">The Token starting positing.</param>
    public OToken(OTokenKind kind, ReadOnlySpan<char> text, int position)
    {
        Kind = kind;
        Text = text.IsEmpty ? null : text.ToString();
        Position = position;
    }

    /// <summary>
    /// Token kind.
    /// </summary>
    public OTokenKind Kind;

    /// <summary>
    /// Token text.
    /// </summary>
    public string Text;

    /// <summary>
    /// Starting Position of token.
    /// </summary>
    public int Position;

    public override string ToString()
    {
        if (Text == null)
        {
            return $"{Kind} at {Position}";
        }

        return $"{Kind}, \"{Text}\" | ({Text.Length}), at {Position}";
    }
}
