//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Default Lexical lexer to convert a text into meaningful lexical tokens.
/// </summary>
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class ExpressionLexer : IExpressionLexer
{
    /// <summary>
    /// Token being processed, it should be updated every time after calling NextToken().
    /// </summary>
    private ExpressionToken _token;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer. Throws exception if null.</param>
    public ExpressionLexer(string text)
        : this(text, LexerOptions.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer.Throws exception if null.</param>
    /// <param name="options">The lexer options.Throws exception if null.</param>
    public ExpressionLexer(string text, LexerOptions options)
        : this(text.AsMemory(), options)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer.</param>
    public ExpressionLexer(ReadOnlyMemory<char> text)
        : this(text, LexerOptions.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer.</param>
    /// <param name="options">The lexer options.Throws exception if null.</param>
    public ExpressionLexer(ReadOnlyMemory<char> text, LexerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        ExpressionText = text;
        TextLen = text.Length;
        Options = options;
        _token = new ExpressionToken(ExpressionKind.None, 0);
        SetTextPos(0);
    }

    /// <summary>
    /// Gets the whole expression source (memory) text to lexer.
    /// </summary>
    public ReadOnlyMemory<char> ExpressionText { get; }

    /// <summary>
    /// Gets the whole expression source (read-only span) text to lexer.
    /// </summary>
    public ReadOnlySpan<char> Source
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ExpressionText.Span;
    }

    /// <summary>
    /// Gets the current token processed.
    /// </summary>
    public ExpressionToken CurrentToken => _token;

    /// <summary>
    /// Gets the lexer options.
    /// </summary>
    public LexerOptions Options { get; }

    /// <summary>
    /// Gets the whole expression source text length.
    /// </summary>
    protected int TextLen { get; }

    /// <summary>
    /// Position on text being processed since last iteration.
    /// Call SetTextPos to set this value.
    /// </summary>
    protected int CurrPos { get; private set; }

    /// <summary>
    /// Character being processed since last iteration.
    /// Call SetTextPos to set this value.
    /// </summary>
    protected char? CurrChar { get; private set; }

    /// <summary>
    /// Gets a boolean indicating whether current processing is valid or not.
    /// </summary>
    protected bool IsCurrValid => CurrPos >= 0 && CurrPos < TextLen;

    /// <summary>
    /// Gets a boolean indicating whether current char is a whitespace or not.
    /// </summary>
    protected bool IsCurrWhitespace => CurrChar.HasValue && char.IsWhiteSpace(CurrChar.Value);

    /// <summary>
    /// Try to peek next token.
    /// </summary>
    /// <param name="token">The output next token.</param>
    /// <returns>True if contains next token, false no next token.</returns>
    public virtual bool PeekNextToken(out ExpressionToken token)
    {
        int savedTextPos = CurrPos;
        char? savedChar = CurrChar;
        ExpressionToken savedToken = _token;

        bool hasNext = false;
        try
        {
            hasNext = NextToken();
            token = hasNext ? CurrentToken : default;
        }
        finally
        {
            CurrPos = savedTextPos;
            CurrChar = savedChar;
            _token = savedToken;
        }

        return hasNext;
    }

    /// <summary>
    /// Reads the next token, advancing the Lexer.
    /// </summary>
    /// <returns>Boolean value indicating read the next token succussed or not, and there maybe more tokens.</returns>
    public virtual bool NextToken()
    {
        // If ignore whitespace, skip the whitespace at the beginning.
        if (Options.IgnoreWhitespace)
        {
            SkipWhitespace();
        }
        else if (TryWhiteSpaceToken())
        {
            return true;
        }

        if (CurrPos >= TextLen)
        {
            SetCurrentTokenState(ExpressionKind.EndOfInput, CurrPos, false);
            return false;
        }

        // Try the special characters, for example: '(', ')' ...
        if (TrySpecialCharToken())
        {
            return true;
        }

        // Try the '-' starting
        if (TryMinusToken())
        {
            return true;
        }

        // Try the string literal, a string literal is wrapped using single quote or double quote
        if (TryStringLiteralToken())
        {
            return true;
        }

        // Try the simple identifier
        if (TryIdentifierToken())
        {
            return true;
        }

        // Try the digit
        if (TryDigitToken())
        {
            return true;
        }

        if (TryAnnotationToken())
        {
            return true;
        }

        if (TryOtherTokens())
        {
            return true;
        }

        throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, CurrChar, CurrPos, ExpressionText));
    }

    /// <summary>
    /// Try to tokenize the whitespace token.
    /// </summary>
    /// <returns>true means tokenized and a new token found, false means doesn't.</returns>
    protected virtual bool TryWhiteSpaceToken()
    {
        if (IsCurrWhitespace)
        {
            int startPos = CurrPos;
            do
            {
                NextChar();
            }
            while (IsCurrWhitespace);

            SetCurrentTokenState(ExpressionKind.Whitespace, startPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Special characters;
    /// </summary>
    private static readonly IDictionary<char, ExpressionKind> SpecialCharacters = new Dictionary<char, ExpressionKind>
    {
        { '(', ExpressionKind.OpenParen },
        { ')', ExpressionKind.CloseParen },
        { '[', ExpressionKind.OpenBracket },
        { ']', ExpressionKind.CloseBracket },
        { '{', ExpressionKind.OpenCurly },
        { '}', ExpressionKind.CloseCurly },
        { ',', ExpressionKind.Comma },
        { '=', ExpressionKind.Equal },
        { '/', ExpressionKind.Slash },
        { '?', ExpressionKind.Question },
        { '.', ExpressionKind.Dot },
        { '*', ExpressionKind.Star },
        { ':', ExpressionKind.Colon },
        { '&', ExpressionKind.Ampersand },
        { ';', ExpressionKind.SemiColon },
        { '$', ExpressionKind.Dollar },
        { '%', ExpressionKind.Percent },

    };

    /// <summary>
    /// Try to tokenize the special characters, for example, '(', ')', ',', '*', etc.
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TrySpecialCharToken()
    {
        if (CurrChar.HasValue && SpecialCharacters.TryGetValue(CurrChar.Value, out ExpressionKind kind))
        {
            int startPos = CurrPos;
            NextChar();
            SetCurrentTokenState(kind, startPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize the minus token, for example '-', or -3.14
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryMinusToken()
    {
        if (CurrChar.HasValue && CurrChar.Value == '-')
        {
            int tokenPos = CurrPos;
            bool hasNext = CurrPos + 1 < TextLen;
            if (hasNext && char.IsDigit(ExpressionText.Span[CurrPos + 1]))
            {
                // don't separate '-' and its following digits : -2147483648 is valid int.MinValue, but 2147483648 is long.
                ExpressionKind t = ParseFromDigit();
                if (ExpressionLexerUtils.IsNumericTokenKind(t))
                {
                    SetCurrentTokenState(t, tokenPos, true);
                    return true;
                }

                // If it looked like a numeric but wasn't, let's rewind and fall through to a simple '-' token.
                SetTextPos(tokenPos);
            }
            else if (hasNext && ExpressionText.Span[CurrPos + 1] == 'I') // could be -INF
            {
                NextChar();
                ParseIdentifier();
                ReadOnlySpan<char> currentIdentifier = ExpressionText.Slice(tokenPos + 1, CurrPos - tokenPos - 1).Span;

                if (ExpressionLexerUtils.IsInfinity(currentIdentifier))
                {
                    SetCurrentTokenState(ExpressionKind.DoubleLiteral, tokenPos);
                    return true;
                }
                else if (ExpressionLexerUtils.IsSingleInfinity(currentIdentifier))
                {
                    SetCurrentTokenState(ExpressionKind.SingleLiteral, tokenPos);
                    return true;
                }

                // TODO: process the NaN ?
                // If it looked like '-INF' but wasn't we'll rewind and fall through to a simple '-' token.
                SetTextPos(tokenPos);
            }

            NextChar();
            SetCurrentTokenState(ExpressionKind.Minus, tokenPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize the string literal token, for example: 'any string value'
    /// String literal can be: Single quoted, or Double quoted.
    /// For single quoted: double quote between is allowed, and single quoted between should be escaped as "\\'"
    /// For double quoted: single quote between is allowed, and double quoted between should be escaped as "\\""
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryStringLiteralToken()
    {
        if (CurrChar == '\'' || CurrChar == '"')
        {
            int tokenPos = CurrPos;
            char? quote = CurrChar;
            char? previous;

            do
            {
                previous = CurrChar;
                NextChar();
            }
            while (CurrPos < TextLen && (CurrChar != quote || previous == '\\'));

            if (CurrPos == TextLen)
            {
                throw new ExpressionLexerException(Error.Format(SRResources.Tokenization_UnterminatedStringLiteral, tokenPos, ExpressionText));
            }

            NextChar(); // remember to read the ending quote
            SetCurrentTokenState(ExpressionKind.StringLiteral, tokenPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize the identifier token, for example 'select'
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryIdentifierToken()
    {
        if (IsValidStartingCharForIdentifier)
        {
            int tokenPos = CurrPos;
            ParseIdentifier(includingDots: false);

            // Guids will have '-' in them
            // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG
            if (CurrChar == '-' && TryParseGuid(tokenPos))
            {
                SetCurrentTokenState(ExpressionKind.GuidLiteral, tokenPos, true);
                return true;
            }

            SetCurrentTokenState(ExpressionKind.Identifier, tokenPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize the digit token, for example '3.14'
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryDigitToken()
    {
        if (CurrChar.HasValue && char.IsDigit(CurrChar.Value))
        {
            int tokenPos = CurrPos;
            ExpressionKind kind = ParseFromDigit();
            SetCurrentTokenState(kind, tokenPos);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize the "@" starting token, for example '@my.name'
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryAnnotationToken()
    {
        if (CurrChar.HasValue && CurrChar.Value == '@')
        {
            int tokenPos = CurrPos;
            NextChar();

            if (CurrPos == TextLen)
            {
                SetCurrentTokenState(ExpressionKind.At, tokenPos, true);
                return true;
            }

            if (!IsValidStartingCharForIdentifier)
            {
                throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, CurrChar, CurrPos, ExpressionText));
            }

            // Include dots for the case of annotation.
            ParseIdentifier(includingDots: true);

            SetCurrentTokenState(ExpressionKind.AnnotationIdentifier, tokenPos, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize other token. It's for derived type to override
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryOtherTokens()
    {
        return false;
    }

    /// <summary>
    /// Is the current char a valid starting char for an identifier.
    /// Valid starting chars for identifier include all that are supported by EDM ([\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]) and '_'.
    /// </summary>
    private bool IsValidStartingCharForIdentifier =>
        CurrChar.HasValue &&
       (char.IsLetter(CurrChar.Value) ||       // IsLetter covers: Ll, Lu, Lt, Lo, Lm
        CurrChar.Value == '_' ||
        CurrChar.Value == '$' ||
        CharUnicodeInfo.GetUnicodeCategory(CurrChar.Value) == UnicodeCategory.LetterNumber);

    /// <summary>
    /// Is the current char a valid non-starting char for an identifier.
    /// Valid non-starting chars for identifier include all that are supported
    /// by EDM  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}].
    /// This list includes '_', which is ConnectorPunctuation (Pc)
    /// </summary>
    private bool IsValidNonStartingCharForIdentifier
        =>
        CurrChar.HasValue &&
        (char.IsLetterOrDigit(CurrChar.Value) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
        AdditionalUnicodeCategoriesForIdentifier.Contains(CharUnicodeInfo.GetUnicodeCategory(CurrChar.Value)));  // covers the rest

    /// <summary>
    /// For an identifier, EMD supports chars that match the regex  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
    /// IsLetterOrDigit covers Ll, Lu, Lt, Lo, Lm, Nd, this set covers the rest
    /// </summary>
    private static readonly HashSet<UnicodeCategory> AdditionalUnicodeCategoriesForIdentifier = new()
    {
            UnicodeCategory.LetterNumber,
            UnicodeCategory.NonSpacingMark,
            UnicodeCategory.SpacingCombiningMark,
            UnicodeCategory.ConnectorPunctuation, // covers "_"
            UnicodeCategory.Format
    };

    private void HandleTypePrefixedLiterals(ref ExpressionKind kind, int start)
    {
        if (kind != ExpressionKind.Identifier)
        {
            return;
        }

        int end = CurrPos;
        ReadOnlySpan<char> tokenText = ExpressionText[start..end].Span;

        // Get literal of quoted values
        if (CurrChar == '\'')
        {
            kind = GetBuiltInTokenKind(tokenText);

            int tokenPos = CurrPos;
            do
            {
                do
                {
                    this.NextChar();
                }
                while (CurrChar != '\'');

                if (CurrPos == TextLen)
                {
                    throw new ExpressionLexerException(Error.Format(SRResources.Tokenization_UnterminatedStringLiteral, tokenPos, ExpressionText));
                }

                this.NextChar();
            }
            while (CurrChar.HasValue && CurrChar.Value == '\'');
            return;
        }

        if (ExpressionLexerUtils.IsInfinityOrNaN(tokenText))
        {
            kind = ExpressionKind.DoubleLiteral;
        }
        else if (ExpressionLexerUtils.IsSingleInfinityOrNaN(tokenText))
        {
            kind = ExpressionKind.SingleLiteral;
        }
        else if (ExpressionLexerUtils.IsBoolean(tokenText))
        {
            kind = ExpressionKind.BooleanLiteral;
        }
        else if (ExpressionLexerUtils.IsNull(tokenText))
        {
            kind = ExpressionKind.NullLiteral;
        }
    }

    /// <summary>
    /// Get type-prefixed literals with quoted values duration, binary and spatial types.
    /// </summary>
    /// <param name="tokenText">Token text</param>
    /// <returns>ExpressionTokenKind</returns>
    /// <example>geometry'POINT (79 84)'. 'geometry' is the tokenText </example>
    private ExpressionKind GetBuiltInTokenKind(ReadOnlySpan<char> tokenText)
    {
        if (tokenText.Equals("duration", StringComparison.Ordinal))
        {
            return ExpressionKind.DurationLiteral;
        }
        else if (tokenText.Equals("binary", StringComparison.Ordinal))
        {
            return ExpressionKind.BinaryLiteral;
        }
        else if (tokenText.Equals("geography", StringComparison.Ordinal))
        {
            return ExpressionKind.GeographyLiteral;
        }
        else if (tokenText.Equals("geometry", StringComparison.Ordinal))
        {
            return ExpressionKind.GeometryLiteral;
        }
        else if (tokenText.Equals("null", StringComparison.Ordinal))
        {
            // typed null literals are not supported.
            throw new ExpressionLexerException(Error.Format(SRResources.Tokenization_SyntaxError, CurrPos, ExpressionText));
        }
        else
        {
            // treat as quoted literal
            return ExpressionKind.QuotedLiteral;
        }
    }

    /// <summary>
    /// Parses a token that starts with a digit or '-'.
    /// </summary>
    /// <returns>The kind of token recognized.</returns>
    private ExpressionKind ParseFromDigit()
    {
        Debug.Assert(CurrChar.HasValue && (char.IsDigit(CurrChar.Value) || (CurrChar == '-')), "IsValidDigit || (CurrChar == '-')");
        ExpressionKind result;
        int tokenPos = CurrPos;
        char startChar = CurrChar.Value;

        NextChar(); // move to next char

        // 0x....
        if (startChar == '0' && (CurrChar.HasValue && (CurrChar.Value == 'x' || CurrChar.Value == 'X')))
        {
            result = ExpressionKind.BinaryLiteral;
            do
            {
                NextChar();
            }
            while (IsCurrValid && CharUtils.IsCharHexDigit(CurrChar.Value));
        }
        else
        {
            result = ExpressionKind.IntegerLiteral;
            while (IsCurrValid && char.IsDigit(CurrChar.Value))
            {
                NextChar();
            }

            // DateTimeOffset, DateOnly and Guids will have '-' in them
            if (CurrChar.HasValue && CurrChar.Value == '-')
            {
                // TODO: do a refactor later to improve the performance here. (at least not do ParseIdentifier again and again)
                if (TryParseDate(tokenPos))
                {
                    return ExpressionKind.DateOnlyLiteral;
                }
                else if (TryParseDateTimeOffset(tokenPos))
                {
                    return ExpressionKind.DateTimeOffsetLiteral;
                }
                else if (TryParseGuid(tokenPos))
                {
                    return ExpressionKind.GuidLiteral;
                }
            }

            // TimeOnly will have ":" in them
            if (CurrChar.HasValue && CurrChar.Value == ':' && TryParseTimeOnly(tokenPos))
            {
                return ExpressionKind.TimeOnlyLiteral;
            }

            // Guids will have alpha-numeric characters along with '-', so if a letter is encountered
            // try to see if this is Guid or not.
            if (CurrChar.HasValue && char.IsLetter(CurrChar.Value))
            {
                if (TryParseGuid(tokenPos))
                {
                    return ExpressionKind.GuidLiteral;
                }
            }

            if (CurrChar.HasValue && CurrChar.Value == '.')
            {
                result = ExpressionKind.DoubleLiteral;
                NextChar();
                ValidateDigit();

                do
                {
                    NextChar();
                }
                while (CurrChar.HasValue && char.IsDigit(CurrChar.Value));
            }

            if (CurrChar.HasValue && (CurrChar == 'E' || CurrChar == 'e'))
            {
                result = ExpressionKind.DoubleLiteral;
                this.NextChar();
                if (CurrChar.HasValue && (CurrChar == '+' || CurrChar == '-'))
                {
                    NextChar();
                }

                this.ValidateDigit();
                do
                {
                    NextChar();
                }
                while (CurrChar.HasValue && char.IsDigit(CurrChar.Value));
            }

            if (CurrChar == 'M' || CurrChar == 'm')
            {
                result = ExpressionKind.DecimalLiteral;
                NextChar();
            }
            else if (CurrChar == 'd' || CurrChar == 'D')
            {
                result = ExpressionKind.DoubleLiteral;
                NextChar();
            }
            else if (CurrChar == 'L' || CurrChar == 'l')
            {
                result = ExpressionKind.Int64Literal;
                NextChar();
            }
            else if (CurrChar == 'f' || CurrChar == 'F')
            {
                result = ExpressionKind.SingleLiteral;
                NextChar();
            }
            else
            {
                ReadOnlySpan<char> valueStr = ExpressionText[tokenPos..CurrPos].Span;
                result = MakeBestGuessOnNoSuffixStr(valueStr, result);
            }
        }

        return result;
    }

    /// <summary>
    /// Read and skip white spaces
    /// </summary>
    protected virtual void SkipWhitespace()
    {
        while (CurrChar.HasValue && char.IsWhiteSpace(CurrChar.Value))
        {
            NextChar();
        }
    }

    /// <summary>
    /// Advanced to the next character.
    /// </summary>
    protected virtual void NextChar()
    {
        if (CurrPos < TextLen)
        {
            ++CurrPos;
            if (CurrPos >= 0 && CurrPos < TextLen)
            {
                CurrChar = ExpressionText.Span[CurrPos];
                return;
            }
        }

        CurrChar = null;
    }

    /// <summary>
    /// Set current token state.
    /// </summary>
    /// <param name="kind">The current token kind.</param>
    /// <param name="start">The starting position of current token.</param>
    /// <param name="needText">Whether the text of the current token is needed.</param>
    protected virtual void SetCurrentTokenState(ExpressionKind kind, int start, bool needText = true)
    {
        HandleTypePrefixedLiterals(ref kind, start);

        _token.Kind = kind;
        _token.Position = start;

        if (needText)
        {
            _token.Text = ExpressionText.Slice(start, CurrPos - start);
        }
        else
        {
            _token.Text = default;
        }
    }

    /// <summary>
    /// Sets the text position.
    /// </summary>
    /// <param name="pos">The new text position.</param>
    protected void SetTextPos(int pos)
    {
        CurrPos = pos < 0 ? 0 : pos;
        CurrChar = CurrPos < TextLen ? ExpressionText.Span[CurrPos] : null;
    }

    /// <summary>
    /// Tries to parse DateTimeOffset from current text. If it's not DateTimeOffset, then reset
    /// </summary>
    /// <param name="startPos">Start index</param>
    /// <returns>True if the substring that starts from tokenPos is a Guid, false otherwise</returns>
    private bool TryParseDateTimeOffset(int startPos)
    {
        int initialIndex = CurrPos;

        ReadOnlySpan<char> datetimeOffsetStr = ParseLiteral(startPos);

        if (DateTimeOffset.TryParse(datetimeOffsetStr, out _))
        {
            return true;
        }
        else
        {
            CurrPos = initialIndex;
            CurrChar = ExpressionText.Span[initialIndex];
            return false;
        }
    }

    /// <summary>
    /// Tries to parse DateOnly from current text, If it's not Guid, then reset
    /// </summary>
    /// <param name="startPos">Start index</param>
    /// <returns>True if the substring that starts from startPos is a Guid, false otherwise</returns>
    private bool TryParseDate(int startPos)
    {
        int initialIndex = CurrPos;

        ReadOnlySpan<char> dateStr = ParseLiteral(startPos);
        if (DateOnly.TryParse(dateStr, out _))
        {
            return true;
        }
        else
        {
            CurrPos = initialIndex;
            CurrChar = ExpressionText.Span[initialIndex];
            return false;
        }
    }

    /// <summary>
    /// Tries to parse TimeOnly from current text
    /// If it's not TimeOnly, then this.textPos and this.ch are reset
    /// </summary>
    /// <param name="startPos">Start index</param>
    /// <returns>True if the substring that starts from tokenPos is a TimeOnly, false otherwise</returns>
    private bool TryParseTimeOnly(int startPos)
    {
        int initialIndex = CurrPos;

        ReadOnlySpan<char> timeOnlyStr = ParseLiteral(startPos);
        if (TimeOnly.TryParse(timeOnlyStr, out _))
        {
            return true;
        }
        else
        {
            CurrPos = initialIndex;
            CurrChar = ExpressionText.Span[initialIndex];
            return false;
        }
    }

    /// <summary>
    /// Tries to parse Guid from current text, If it's not Guid, then reset
    /// </summary>
    /// <param name="startPos">Start index</param>
    /// <returns>True if the substring that starts from startPos is a Guid, false otherwise</returns>
    private bool TryParseGuid(int startPos)
    {
        int initialIndex = CurrPos;

        ReadOnlySpan<char> guidStr = ParseLiteral(startPos);
        if (Guid.TryParse(guidStr, out _))
        {
            return true;
        }
        else
        {
            CurrPos = initialIndex;
            CurrChar = ExpressionText.Span[initialIndex];
            return false;
        }
    }

    private ReadOnlySpan<char> ParseLiteral(int tokenPos)
    {
        do
        {
            NextChar();
        }
        while (CurrChar.HasValue && CurrChar.Value != ',' && CurrChar.Value != ')' && CurrChar.Value != ' ');

        var numericStr = ExpressionText[tokenPos..CurrPos].Span;
        return numericStr;
    }

    /// <summary>
    /// Makes best guess on numeric string without trailing letter like L, F, M, D
    /// </summary>
    /// <param name="numericStr">The numeric string.</param>
    /// <param name="guessedKind">The possible kind (IntegerLiteral or DoubleLiteral) from ParseFromDigit() method.</param>
    /// <returns>A more accurate ExpressionTokenKind</returns>
    private static ExpressionKind MakeBestGuessOnNoSuffixStr(ReadOnlySpan<char> numericStr, ExpressionKind guessedKind)
    {
        // no suffix, so
        // (1) make a best guess (note: later we support promoting each to later one: int32->int64->single->double->decimal).
        // look at value:       "2147483647" may be Int32/long, "2147483649" must be long.
        // look at precision:   "3258.67876576549" may be single/double/decimal, "3258.678765765489753678965390" must be decimal.
        // (2) then let MetadataUtilsCommon.CanConvertPrimitiveTypeTo() method does further promotion when knowing expected semantics type.
        if (guessedKind == ExpressionKind.IntegerLiteral)
        {
            if (int.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                return ExpressionKind.IntegerLiteral;
            }

            if (long.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                return ExpressionKind.Int64Literal;
            }
        }

        bool canBeSingle = float.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float tmpFloat);
        bool canBeDouble = double.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double tmpDouble);
        bool canBeDecimal = decimal.TryParse(numericStr, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal tmpDecimal);

        // 1. try high precision -> low precision
        if (canBeDouble && canBeDecimal)
        {
            // To keep the full precision of the current value, which if necessary is all 17 digits of precision supported by the Double type.
            bool doubleCanBeDecimalR = decimal.TryParse(tmpDouble.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal doubleToDecimalR);

            // To cover the scientific notation case, such as 1e+19 in the tmpDouble
            bool doubleCanBeDecimalN = decimal.TryParse(tmpDouble.ToString("N29", CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal doubleToDecimalN);

            if ((doubleCanBeDecimalR && doubleToDecimalR != tmpDecimal) || (!doubleCanBeDecimalR && doubleCanBeDecimalN && doubleToDecimalN != tmpDecimal))
            {
                // losing precision as double, so choose decimal
                return ExpressionKind.DecimalLiteral;
            }
        }

        // here can't use normal casting like the above double VS decimal.
        // prevent losing precision in float -> double, e.g. (double)1.234f will be 1.2339999675750732d not 1.234d
        if (canBeSingle && canBeDouble && (double.Parse(tmpFloat.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) != tmpDouble))
        {
            // losing precision as single, so choose double
            return ExpressionKind.DoubleLiteral;
        }

        // 2. try most compatible -> least compatible
        if (canBeSingle)
        {
            return ExpressionKind.SingleLiteral;
        }

        if (canBeDouble)
        {
            return ExpressionKind.DoubleLiteral;
        }

        throw new ExpressionLexerException(Error.Format(SRResources.Tokenization_InvalidNumericString, numericStr.ToString()));
    }

    /// <summary>
    /// Parses an identifier by advancing the current character.
    /// </summary>
    /// <param name="includingDots">Optional flag for whether to include dots as part of the identifier.</param>
    private void ParseIdentifier(bool includingDots = false)
    {
        Debug.Assert(IsValidStartingCharForIdentifier || CurrChar == '@', "Expected valid starting char for identifier");
        do
        {
            NextChar();
        }
        while (IsValidNonStartingCharForIdentifier || (includingDots && CurrChar == '.'));
    }

    /// <summary>
    /// Validates the current character is a digit.
    /// </summary>
    private void ValidateDigit()
    {
        if (CurrChar == null || !char.IsDigit(CurrChar.Value))
        {
            throw new ExpressionLexerException(Error.Format(SRResources.Tokenization_DigitExpected, CurrPos, ExpressionText));
        }
    }

    private string DebuggerToString()
    {
        return $"Current: {CurrentToken.ToString()}, Next: {CurrPos}: \"{CurrChar}\" at {ExpressionText}.";
    }
}
