//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Enumeration values for token kinds.
/// </summary>
public enum OTokenKind
{
    /// <summary>Unknown.</summary>
    Unknown,

    /// <summary>Character: '('.</summary>
    OpenParen,

    /// <summary>Character: ')'.</summary>
    CloseParen,

    /// <summary>Character: ,</summary>
    Comma,

    /// <summary>Character: /</summary>
    Slash,

    /// <summary>Character: =</summary>
    Equal,

    /// <summary>Character: ?</summary>
    Question,

    /// <summary>Character: .</summary>
    Dot,

    /// <summary>Character: :</summary>
    Colon,

    /// <summary>Character: *</summary>
    Star,

    /// <summary>Character: ;</summary>
    SemiColon,

    /// <summary>Character: $</summary>
    Dollar,

    /// <summary>Character: -</summary>
    Minus,

    /// <summary>Character: @.</summary>
    At,

    /// <summary>Character: &</summary>
    Ampersand,

    /// <summary>Whitespace</summary>
    Whitespace,

    /// <summary>IntegerLiteral.</summary>
    IntegerLiteral,

    /// <summary>Binary literal.</summary>
    BinaryLiteral,

    /// <summary>Decimal literal.</summary>
    DecimalLiteral,

    /// <summary>Double literal.</summary>
    DoubleLiteral,

    /// <summary>Int64 literal.</summary>
    Int64Literal,

    /// <summary>Single literal.</summary>
    SingleLiteral,

    /// <summary>GUID literal.</summary>
    GuidLiteral,

    /// <summary>DateOnly literal.</summary>
    DateOnlyLiteral,

    /// <summary>TimeOnly literal.</summary>
    TimeOnlyLiteral,

    /// <summary>DateTimeOffset literal.</summary>
    DateTimeOffsetLiteral,

    /// <summary>StringLiteral.</summary>
    StringLiteral,

    /// <summary>Identifier.</summary>
    Identifier,

    /// <summary>End of input</summary>
    EndOfInput,


    ParameterAlias,


}
