//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Lexical tokenization is conversion of a text into meaningful lexical tokens.
/// </summary>
public interface IOTokenizer
{
    /// <summary>
    /// Gets the token kind processed.
    /// </summary>
    public OTokenKind CurrentTokenKind { get; }

    /// <summary>
    /// Gets the token text processed, It's partial view of original text expression.
    /// </summary>
    ReadOnlySpan<char> CurrentTokenText { get; }

    /// <summary>
    /// Gets the starting position of token processed.
    /// </summary>
    public int CurrentTokenPosition { get; }

    /// <summary>
    /// Reads the next token, advancing the tokenization Lexer, updating the token processed,
    /// refreshing the processed token from the properties (CurrentTokenKind, CurrentTokenText, CurrentTokenPosition).
    /// </summary>
    /// <returns>Boolean value indicating reading the next token succussed or not.</returns>
    bool NextToken();
}
