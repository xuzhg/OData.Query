//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;

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
    internal static bool IsInfinityLiteralDouble(ReadOnlySpan<char> text)
        => text.Equals("INF", StringComparison.Ordinal);

    /// <summary>
    /// Checks whether <paramref name="text"/> EQUALS to 'INFf' or 'INFF'.
    /// </summary>
    /// <param name="text">Text to look in.</param>
    /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
    internal static bool IsInfinityLiteralSingle(ReadOnlySpan<char> text)
        =>  text.Length == 4 &&
            (text[3] == 'f' || text[3] == 'F') &&
            text.Slice(0, 3).Equals("INF", StringComparison.Ordinal);
}
