//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Utilities needed by <see cref="IOTokenizer"/> which are relatively simple and standalone.
/// </summary>
internal static class OTokenizationUtils
{
    /// <summary>
    /// Whether the specified token identifier is a numeric literal.
    /// </summary>
    /// <param name="kind">Token to check.</param>
    /// <returns>true if it's a numeric literal; false otherwise.</returns>
    internal static bool IsNumericTokenKind(OTokenKind kind)
        => kind == OTokenKind.IntegerLiteral ||
           kind == OTokenKind.DecimalLiteral ||
           kind == OTokenKind.DoubleLiteral ||
           kind == OTokenKind.Int64Literal ||
           kind == OTokenKind.SingleLiteral;

    /// <summary>
    /// Checks whether <paramref name="text"/> equals to 'INF'
    /// </summary>
    /// <param name="text">Text to look in.</param>
    /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
    internal static bool IsInfinity(ReadOnlySpan<char> text)
        => text.Equals("INF", StringComparison.Ordinal);

    /// <summary>
    /// Checks whether <paramref name="text"/> equals to 'NaN'
    /// </summary>
    /// <param name="text">Text to look in.</param>
    /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
    internal static bool IsNaN(ReadOnlySpan<char> text)
        => text.Equals("NaN", StringComparison.Ordinal);

    /// <summary>
    /// Checks whether <paramref name="text"/> EQUALS to 'INFf' or 'INFF'.
    /// </summary>
    /// <param name="text">Text to look in.</param>
    /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
    internal static bool IsSingleInfinity(ReadOnlySpan<char> text)
        =>  text.Length == 4 &&
            (text[3] == 'f' || text[3] == 'F') &&
            text[..3].Equals("INF", StringComparison.Ordinal);

    /// <summary>
    /// Checks whether <paramref name="text"/> EQUALS to 'NaNF' or 'NaNf'.
    /// </summary>
    /// <param name="text">Text to look in.</param>
    /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
    internal static bool IsSingleNaN(ReadOnlySpan<char> text)
        => text.Length == 4 &&
            (text[3] == 'f' || text[3] == 'F') &&
            text[..3].Equals("NaN", StringComparison.Ordinal);

    /// <summary>
    /// Checks if the <paramref name="tokenText"/> is INF or NaN.
    /// </summary>
    /// <param name="tokenText">Input token.</param>
    /// <returns>true if match found, false otherwise.</returns>
    internal static bool IsInfinityOrNaN(ReadOnlySpan<char> text)
        => IsInfinity(text) || IsNaN(text);

    /// <summary>
    /// Checks if the <paramref name="tokenText"/> is INF{f|F} or NaN{f|F}.
    /// </summary>
    /// <param name="tokenText">Input token.</param>
    /// <returns>true if match found, false otherwise.</returns>
    internal static bool IsSingleInfinityOrNaN(ReadOnlySpan<char> text)
        => IsSingleInfinity(text) || IsSingleNaN(text);

    internal static bool IsBoolean(ReadOnlySpan<char> text)
        => text.Equals("true", StringComparison.Ordinal) || text.Equals("false", StringComparison.Ordinal);

    internal static bool IsNull(ReadOnlySpan<char> text)
        => text.Equals("null", StringComparison.Ordinal);
}
