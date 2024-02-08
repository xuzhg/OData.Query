//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Lexers;

/// <summary>
/// Default Lexical lexer to convert a text into meaningful lexical tokens.
/// </summary>
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class ExpressionLexer : IExpressionLexer
{
    /// <summary>Token kind being processed.</summary>
    private ExpressionKind _tokenKind;

    /// <summary>Starting position of token being processed.</summary>
    private int _tokenPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer.</param>
    public ExpressionLexer(string text)
        : this(text, LexerOptions.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionLexer" /> class.
    /// </summary>
    /// <param name="text">The text expression to lexer.</param>
    /// <param name="options">The lexer options.</param>
    public ExpressionLexer(string text, LexerOptions options)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        Options = options ?? LexerOptions.Default;

        ExpressionText = text;
        _tokenKind = ExpressionKind.Unknown;
        _tokenPosition = 0;
        SetTextPos(0);
    }

    /// <summary>
    /// Gets the whole expression text to lexer.
    /// </summary>
    public string ExpressionText { get; }

    /// <summary>
    /// Gets the length of expression text.
    /// </summary>
    public int Length => ExpressionText.Length;

    /// <summary>
    /// Gets the lexer options.
    /// </summary>
    public LexerOptions Options { get; }

    /// <summary>
    /// Gets the current token processed.
    /// Since ExpressionToken is ref struct, it's stack-only instance and do the member copy if customer calls this property.
    /// </summary>
    public ExpressionToken CurrentToken
        => new ExpressionToken(_tokenKind, ExpressionText.AsSpan()[_tokenPosition..CurrPos], _tokenPosition);

    /// <summary>
    /// Position on text being processed.
    /// </summary>
    protected int CurrPos { get; private set; }

    /// <summary>
    /// Character being processed.
    /// </summary>
    protected char CurrChar { get; private set; }

    /// <summary>
    /// Gets a boolean indicating whether current processing is valid or not.
    /// </summary>
    protected bool IsCurrValid => CurrPos >= 0 && CurrPos < Length;

    /// <summary>
    /// Try to peek next token.
    /// </summary>
    /// <param name="token">The output next token.</param>
    /// <returns>True if contains next token, false no next token.</returns>
    public virtual bool TryPeekNextToken(out ExpressionToken token)
    {
        int savedTextPos = CurrPos;
        char savedChar = CurrChar;
        ExpressionKind savedKind = _tokenKind;
        int savedTokenPosition = _tokenPosition;

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
            _tokenKind = savedKind;
            _tokenPosition = savedTokenPosition;
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

        if (CurrPos == Length)
        {
            SetCurrentTokenState(ExpressionKind.EndOfInput, CurrPos);
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

        if (TryOtherToken())
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
        if (char.IsWhiteSpace(CurrChar))
        {
            int startPos = CurrPos;
            do
            {
                NextChar();
            }
            while (char.IsWhiteSpace(CurrChar));

            SetCurrentTokenState(ExpressionKind.Whitespace, startPos);
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
    };

    /// <summary>
    /// Try to tokenize the special characters, for example, '(', ')', ',', '*', etc.
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TrySpecialCharToken()
    {
        if (SpecialCharacters.TryGetValue(CurrChar, out ExpressionKind kind))
        {
            int startPos = CurrPos;
            NextChar();
            SetCurrentTokenState(kind, startPos);
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
        if (CurrChar == '-')
        {
            int tokenPos = CurrPos;
            bool hasNext = CurrPos + 1 < Length;
            if (hasNext && char.IsDigit(ExpressionText[CurrPos + 1]))
            {
                // don't separate '-' and its following digits : -2147483648 is valid int.MinValue, but 2147483648 is long.
                ExpressionKind t = ParseFromDigit();
                if (ExpressionLexerUtils.IsNumericTokenKind(t))
                {
                    SetCurrentTokenState(t, tokenPos);
                    return true;
                }

                // If it looked like a numeric but wasn't, let's rewind and fall through to a simple '-' token.
                SetTextPos(tokenPos);
            }
            else if (hasNext && ExpressionText[CurrPos + 1] == 'I') // could be -INF
            {
                NextChar();
                ParseIdentifier();
                ReadOnlySpan<char> currentIdentifier = ExpressionText.AsSpan().Slice(tokenPos + 1, CurrPos - tokenPos - 1);

                if (OTokenizationUtils.IsInfinity(currentIdentifier))
                {
                    SetCurrentTokenState(ExpressionKind.DoubleLiteral, tokenPos);
                    return true;
                }
                else if (OTokenizationUtils.IsSingleInfinity(currentIdentifier))
                {
                    SetCurrentTokenState(ExpressionKind.SingleLiteral, tokenPos);
                    return true;
                }

                // If it looked like '-INF' but wasn't we'll rewind and fall through to a simple '-' token.
                SetTextPos(tokenPos);
            }

            NextChar();
            SetCurrentTokenState(ExpressionKind.Minus, tokenPos);
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
            char quote = CurrChar;
            char previous;

            do
            {
                previous = CurrChar;
                NextChar();
            }
            while (CurrPos < Length && (CurrChar != quote || previous == '\\'));

            if (CurrPos == Length)
            {
                throw new OTokenizationException(Error.Format(SRResources.Tokenization_UnterminatedStringLiteral, tokenPos, ExpressionText));
            }

            NextChar(); // remember to read the ending quote
            SetCurrentTokenState(ExpressionKind.StringLiteral, tokenPos);
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
                SetCurrentTokenState(ExpressionKind.GuidLiteral, tokenPos);
                return true;
            }

            SetCurrentTokenState(ExpressionKind.Identifier, tokenPos);
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
        if (char.IsDigit(CurrChar))
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
        if (CurrChar == '@')
        {
            int tokenPos = CurrPos;
            NextChar();

            if (CurrPos == Length)
            {
                SetCurrentTokenState(ExpressionKind.At, tokenPos);
                return true;
            }

            if (!IsValidStartingCharForIdentifier)
            {
                throw new ExpressionLexerException(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, CurrChar, CurrPos, ExpressionText));
            }

            // Include dots for the case of annotation.
            ParseIdentifier(includingDots: true);

            SetCurrentTokenState(ExpressionKind.AnnotationIdentifier, tokenPos);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try to tokenize other token. It's for derived type to override
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse.</returns>
    protected virtual bool TryOtherToken()
    {
        return false;
    }

    /// <summary>
    /// Is the current char a valid starting char for an identifier.
    /// Valid starting chars for identifier include all that are supported by EDM ([\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]) and '_'.
    /// </summary>
    private bool IsValidStartingCharForIdentifier =>
        char.IsLetter(CurrChar) ||       // IsLetter covers: Ll, Lu, Lt, Lo, Lm
        CurrChar == '_' ||
        CurrChar == '$' ||
        CharUnicodeInfo.GetUnicodeCategory(CurrChar) == UnicodeCategory.LetterNumber;

    /// <summary>
    /// Is the current char a valid non-starting char for an identifier.
    /// Valid non-starting chars for identifier include all that are supported
    /// by EDM  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}].
    /// This list includes '_', which is ConnectorPunctuation (Pc)
    /// </summary>
    private bool IsValidNonStartingCharForIdentifier
        =>
        char.IsLetterOrDigit(CurrChar) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
        AdditionalUnicodeCategoriesForIdentifier.Contains(CharUnicodeInfo.GetUnicodeCategory(CurrChar));  // covers the rest

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
        ReadOnlySpan<char> tokenText = ExpressionText.AsSpan()[start..end];

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

                if (CurrPos == Length)
                {
                    throw new OTokenizationException(Error.Format(SRResources.Tokenization_UnterminatedStringLiteral, tokenPos, ExpressionText));
                }

                this.NextChar();
            }
            while (CurrChar == '\'');
            return;
        }

        if (OTokenizationUtils.IsInfinityOrNaN(tokenText))
        {
            kind = ExpressionKind.DoubleLiteral;
        }
        else if (OTokenizationUtils.IsSingleInfinityOrNaN(tokenText))
        {
            kind = ExpressionKind.SingleLiteral;
        }
        else if (OTokenizationUtils.IsBoolean(tokenText))
        {
            kind = ExpressionKind.BooleanLiteral;
        }
        else if (OTokenizationUtils.IsNull(tokenText))
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
            throw new OTokenizationException(Error.Format(SRResources.Tokenization_SyntaxError, CurrPos, ExpressionText));
        }
        else
        {
            // treat as quoted literal
            return ExpressionKind.QuotedLiteral;
        }
    }

    /// <summary>
    /// Parses a token that starts with a digit.
    /// </summary>
    /// <returns>The kind of token recognized.</returns>
    private ExpressionKind ParseFromDigit()
    {
        Debug.Assert(char.IsDigit(CurrChar) || (CurrChar == '-'), "IsValidDigit || (CurrChar == '-')");
        ExpressionKind result;
        int tokenPos = CurrPos;
        char startChar = CurrChar;
        NextChar();

        // 0x....
        if (startChar == '0' && (CurrPos == 'x' || CurrPos == 'X'))
        {
            result = ExpressionKind.BinaryLiteral;
            do
            {
                NextChar();
            }
            while (IsCurrValid && CharUtils.IsCharHexDigit(CurrChar));
        }
        else
        {
            result = ExpressionKind.IntegerLiteral;
            while (IsCurrValid && char.IsDigit(CurrChar))
            {
                NextChar();
            }

            // DateTimeOffset, DateOnly and Guids will have '-' in them
            if (CurrChar == '-')
            {
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

            // TimeOfDay will have ":" in them
            if (CurrChar == ':' && TryParseTimeOfDay(tokenPos))
            {
                return ExpressionKind.TimeOnlyLiteral;
            }

            // Guids will have alpha-numeric characters along with '-', so if a letter is encountered
            // try to see if this is Guid or not.
            if (char.IsLetter(CurrChar))
            {
                if (TryParseGuid(tokenPos))
                {
                    return ExpressionKind.GuidLiteral;
                }
            }

            if (CurrChar == '.')
            {
                result = ExpressionKind.DoubleLiteral;
                NextChar();
                ValidateDigit();

                do
                {
                    NextChar();
                }
                while (char.IsDigit(CurrChar));
            }

            if (CurrChar == 'E' || CurrChar == 'e')
            {
                result = ExpressionKind.DoubleLiteral;
                this.NextChar();
                if (CurrChar == '+' || CurrChar == '-')
                {
                    NextChar();
                }

                this.ValidateDigit();
                do
                {
                    NextChar();
                }
                while (char.IsDigit(CurrChar));
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
                ReadOnlySpan<char> valueStr = ExpressionText.AsSpan()[tokenPos..CurrPos];
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
        while (char.IsWhiteSpace(CurrChar))
        {
            NextChar();
        }
    }

    /// <summary>
    /// Advanced to the next character.
    /// </summary>
    protected virtual void NextChar()
    {
        if (CurrPos < Length)
        {
            ++CurrPos;
            if (CurrPos >= 0 && CurrPos < Length)
            {
                CurrChar = ExpressionText[CurrPos];
                return;
            }
        }

        CurrChar = '\0';
    }

    /// <summary>
    /// Set current token state.
    /// </summary>
    /// <param name="kind">The current token kind.</param>
    /// <param name="start">The starting position of current token.</param>
    protected virtual void SetCurrentTokenState(ExpressionKind kind, int start)
    {
        HandleTypePrefixedLiterals(ref kind, start);

        _tokenKind = kind;
        _tokenPosition = start;
    }

    /// <summary>
    /// Sets the text position.
    /// </summary>
    /// <param name="pos">New text position.</param>
    protected void SetTextPos(int pos)
    {
        CurrPos = pos < 0 ? 0 : pos;
        CurrChar = CurrPos < Length ? ExpressionText[CurrPos] : '\0';
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
            CurrChar = ExpressionText[initialIndex];
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
            CurrChar = ExpressionText[initialIndex];
            return false;
        }
    }

    /// <summary>
    /// Tries to parse TimeOfDay from current text
    /// If it's not TimeOfDay, then this.textPos and this.ch are reset
    /// </summary>
    /// <param name="startPos">Start index</param>
    /// <returns>True if the substring that starts from tokenPos is a TimeOfDay, false otherwise</returns>
    private bool TryParseTimeOfDay(int startPos)
    {
        int initialIndex = CurrPos;

        ReadOnlySpan<char> timeOfDayStr = ParseLiteral(startPos);

        if (TimeOnly.TryParse(timeOfDayStr, out _))
        {
            return true;
        }
        else
        {
            CurrPos = initialIndex;
            CurrChar = ExpressionText[initialIndex];
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
            CurrChar = ExpressionText[initialIndex];
            return false;
        }
    }

    private ReadOnlySpan<char> ParseLiteral(int tokenPos)
    {
        do
        {
            NextChar();
        }
        while (CurrChar != '\0' && CurrChar != ',' && CurrChar != ')' && CurrChar != ' ');

        var numericStr = ExpressionText.AsSpan()[tokenPos..CurrPos];
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

        throw new OTokenizationException(Error.Format(SRResources.Tokenization_InvalidNumericString, numericStr.ToString()));
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
        if (!char.IsDigit(CurrChar))
        {
            throw new OTokenizationException(Error.Format(SRResources.Tokenization_DigitExpected, CurrPos, ExpressionText));
        }
    }

    private string DebuggerToString()
    {
        string currentChar = CurrChar == '\0' ? "\\0" : CurrChar.ToString();
        return $"Current: {CurrentToken.ToString()}, Next: {CurrPos}: \"{currentChar}\" at {ExpressionText}.";
    }
}
