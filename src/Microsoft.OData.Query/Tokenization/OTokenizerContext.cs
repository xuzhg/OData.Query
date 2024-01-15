//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Represents a lexical expression token.
/// </summary>
public class OTokenizerContext
{
    internal static OTokenizerContext Default = new OTokenizerContext();

    /// <summary>
    /// Ignores whitespace.
    /// What's whitespace?
    /// There are characters which belong to UnicodeCategory.Control but are considered as white spaces besides ' ':
    /// U+0009 = HORIZONTAL TAB, '\t'
    /// U+000a = LINE FEED, '\n'
    /// U+000b = VERTICAL TAB, '\v'
    /// U+000c = FORM FEED, '\f'
    /// U+000d = CARRIAGE RETURN, '\r'
    /// U+0085 = NEXT LINE
    /// U+00a0 = NO-BREAK SPACE
    /// </summary>
    public bool IgnoreWhitespace { get; set; } = true;

    /// <summary>
    /// flag to indicate whether to delimit on a semicolon.
    /// </summary>
    public bool UseSemicolonDelimiter { get; set; } = false;

    public bool ParsingFunctionParameters { get; set; } = false;
}
