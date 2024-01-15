//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Default Lexical tokenizer to convert a text into meaningful lexical tokens.
/// </summary>
[DebuggerDisplay("{CurrPos}, '{CurrChar}', [{_token}])")]
public class OTokenizer : IOTokenizer
{
    protected OToken _token; // Token being processed

    /// <summary>
    /// Initializes a new instance of the <see cref="OTokenizer" /> class.
    /// </summary>
    /// <param name="text">The text expression to tokenize.</param>
    public OTokenizer(string text)
        : this(text, OTokenizerContext.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OTokenizer" /> class.
    /// </summary>
    /// <param name="text">The text expression to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    public OTokenizer(string text, OTokenizerContext context)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        Text = text;
        TextLen = text.Length;
        Context = context ?? throw new ArgumentNullException(nameof(context));
        SetTextPos(0);
    }

    /// <summary>
    /// Gets the text being tokenized.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the length of text.
    /// </summary>
    public int TextLen { get; }

    /// <summary>
    /// Gets the tokenizer context.
    /// </summary>
    public OTokenizerContext Context { get; }

    /// <summary>
    /// Current token being processed.
    /// </summary>
    public OToken CurrentToken => _token;

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
    protected bool IsCurrValid => CurrPos >= 0 && CurrPos < TextLen;

    /// <summary>
    /// Reads the next token, advancing the tokenization Lexer.
    /// </summary>
    /// <returns>Boolean value indicating read the next token succussed or not, and there maybe more tokens.</returns>
    public virtual bool NextToken()
    {
        _token.Reset(OTokenKind.Unknown, string.Empty, -1);

        // If ignore whitespace, skip the whitespace at the beginning.
        if (Context.IgnoreWhitespace)
        {
            SkipWhitespace();
        }
        else if (TryWhiteSpaceToken())
        {
            return true;
        }

        if (CurrPos == TextLen)
        {
            _token.Reset(OTokenKind.EndOfInput, "\0", CurrPos);
            return false;
        }

        if (TrySpecialCharToken())
        {
            return true;
        }

        if (TryMinusToken())
        {
            return true;
        }

        if (TryStringLiteralToken())
        {
            return true;
        }

        if (TryIdentifierToken())
        {
            return true;
        }

        if (TryDigitToken())
        {
            return true;
        }

        if (TryParseAnnotation())
        {
            return true;
        }

        throw new OTokenizationException(Error.Format(SRResources.Tokenization_InvalidCharacter, CurrChar, CurrPos, Text));
    }

    private bool TryParseAnnotation()
    {
        int tokenPos = CurrPos;

        if (CurrChar == '@')
        {
            NextChar();

            if (CurrPos == TextLen)
            {
                _token.Reset(OTokenKind.At, "@", tokenPos);
                return true;
            }

            if (!this.IsValidStartingCharForIdentifier)
            {
                throw new OTokenizationException(Error.Format(SRResources.Tokenization_InvalidCharacter, CurrChar, CurrPos, Text));
            }

            int start = CurrPos;

            // Include dots for the case of annotation.
            this.ParseIdentifier(true /*includingDots*/);

            // Extract the identifier from expression.
            ReadOnlySpan<char> leftToken = Text.AsSpan().Slice(start, CurrPos - start);

            OTokenKind t = Context.ParsingFunctionParameters && !leftToken.Contains(".", StringComparison.Ordinal)
                ? OTokenKind.ParameterAlias
                : OTokenKind.Identifier;

            _token.Reset(t, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
            return true;
        }

        return false;
    }

    private bool TryDigitToken()
    {
        int tokenPos = CurrPos;
        if (char.IsDigit(CurrChar))
        {
            OTokenKind kind = ParseFromDigit();
            _token.Reset(kind, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
            return true;
        }

        return false;
    }

    private bool TryIdentifierToken()
    {
        int tokenPos = CurrPos;

        if (IsValidStartingCharForIdentifier)
        {
            ParseIdentifier();

            // Guids will have '-' in them
            // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG
            if (CurrChar == '-' && TryParseGuid(tokenPos))
            {
                _token.Reset(OTokenKind.GuidLiteral, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
                return true;
            }

            _token.Reset(OTokenKind.Identifier, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
            return true;
        }

        return false;
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

            string whitespaces = Text.Substring(startPos, CurrPos - startPos);

            _token.Reset(OTokenKind.Whitespace, whitespaces, startPos);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Special characters;
    /// </summary>
    private static IDictionary<char, OTokenKind> SpecialCharacters = new Dictionary<char, OTokenKind>
    {
        { '(', OTokenKind.OpenParen },
        { ')', OTokenKind.CloseParen },
        { ',', OTokenKind.Comma },
        { '=', OTokenKind.Equal },
        { '/', OTokenKind.Slash },
        { '?', OTokenKind.Question },
        { '.', OTokenKind.Dot },
        { '*', OTokenKind.Star },
        { ':', OTokenKind.Colon },
        { '&', OTokenKind.Ampersand },
        { ';', OTokenKind.SemiColon },
        { '$', OTokenKind.Dollar },
    };

    /// <summary>
    /// Try to tokenize the special characters, for example, '(', ')', ',', '*', etc.
    /// </summary>
    /// <returns>true means parsed, false means doesn't parse</returns>
    protected virtual bool TrySpecialCharToken()
    {
        if (SpecialCharacters.TryGetValue(CurrChar, out OTokenKind kind))
        {
            // Consider: Maybe we don't need to set the Token.Text since it's special character?
            _token.Reset(kind, null, CurrPos);
            NextChar();
            return true;
        }

        return false;
    }

    private bool TryMinusToken()
    {
        int tokenPos = CurrPos;
        if (CurrChar == '-')
        {
            bool hasNext = CurrPos + 1 < this.TextLen;
            if (hasNext && char.IsDigit(this.Text[CurrPos + 1]))
            {
                // don't separate '-' and its following digits : -2147483648 is valid int.MinValue, but 2147483648 is long.
                OTokenKind t = this.ParseFromDigit();
                if (OTokenizationUtils.IsNumericTokenKind(t))
                {
                    _token.Reset(t, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
                    return true;
                }

                // If it looked like a numeric but wasn't (because it was a binary 0x... value for example),
                // we'll rewind and fall through to a simple '-' token.
                this.SetTextPos(tokenPos);
            }
            else if (hasNext && Text[CurrPos + 1] == 'I') // could be -INF
            {
                NextChar();
                ParseIdentifier();
                ReadOnlySpan<char> currentIdentifier = Text.AsSpan().Slice(tokenPos + 1, CurrPos - tokenPos - 1);

                if (OTokenizationUtils.IsInfinityLiteralDouble(currentIdentifier))
                {
                    _token.Reset(OTokenKind.DoubleLiteral, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
                    return true;
                }
                else if (OTokenizationUtils.IsInfinityLiteralSingle(currentIdentifier))
                {
                    _token.Reset(OTokenKind.SingleLiteral, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
                    return true;
                }

                // If it looked like '-INF' but wasn't we'll rewind and fall through to a simple '-' token.
                this.SetTextPos(tokenPos);
            }

            NextChar();
            _token.Reset(OTokenKind.Minus, "-", tokenPos);
            return true;
        }

        return false;
    }

    private bool TryStringLiteralToken()
    {
        int tokenPos = CurrPos;
        if (CurrChar == '\'')
        {
            char quote = CurrChar;

            do
            {
                AdvanceToNextOccurrenceOf(quote);

                if (CurrPos == TextLen)
                {
                    throw new OTokenizationException(Error.Format(SRResources.Tokenization_UnterminatedStringLiteral, tokenPos, Text));
                }

                NextChar();
            }
            while (CurrChar == quote);

            _token.Reset(OTokenKind.StringLiteral, Text.Substring(tokenPos, CurrPos - tokenPos), tokenPos);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Advance the pointer to the next occurrence of the given value, swallowing all characters in between.
    /// </summary>
    /// <param name="endingValue">the ending delimiter.</param>
    private void AdvanceToNextOccurrenceOf(char endingValue)
    {
        NextChar();
        while (CurrChar != endingValue)
        {
            NextChar();
        }
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
    private static readonly HashSet<UnicodeCategory> AdditionalUnicodeCategoriesForIdentifier = new HashSet<UnicodeCategory>
        {
            UnicodeCategory.LetterNumber,
            UnicodeCategory.NonSpacingMark,
            UnicodeCategory.SpacingCombiningMark,
            UnicodeCategory.ConnectorPunctuation, // covers "_"
            UnicodeCategory.Format
        };

    /// <summary>
    /// Parses an identifier by advancing the current character.
    /// </summary>
    /// <param name="includingDots">Optional flag for whether to include dots as part of the identifier.</param>
    private void ParseIdentifier(bool includingDots = false)
    {
        Debug.Assert(this.IsValidStartingCharForIdentifier || CurrChar == '@', "Expected valid starting char for identifier");
        do
        {
            this.NextChar();
        }
        while (this.IsValidNonStartingCharForIdentifier || (includingDots && CurrChar == '.'));
    }

    /// <summary>
    /// Parses a token that starts with a digit.
    /// </summary>
    /// <returns>The kind of token recognized.</returns>
    private OTokenKind ParseFromDigit()
    {
        Debug.Assert(char.IsDigit(CurrChar) || (CurrChar == '-'), "IsValidDigit || (CurrChar == '-')");
        OTokenKind result;
        int tokenPos = CurrPos;
        char startChar = CurrChar;
        NextChar();

        // 0x....
        if (startChar == '0' && (CurrPos == 'x' || CurrPos == 'X'))
        {
            result = OTokenKind.BinaryLiteral;
            do
            {
                NextChar();
            }
            while (IsCurrValid && CharUtils.IsCharHexDigit(CurrChar));
        }
        else
        {
            result = OTokenKind.IntegerLiteral;
            while (IsCurrValid && char.IsDigit(CurrChar))
            {
                NextChar();
            }

            // DateTimeOffset, DateOnly and Guids will have '-' in them
            if (CurrChar == '-')
            {
                if (TryParseDate(tokenPos))
                {
                    return OTokenKind.DateOnlyLiteral;
                }
                else if (TryParseDateTimeOffset(tokenPos))
                {
                    return OTokenKind.DateTimeOffsetLiteral;
                }
                else if (TryParseGuid(tokenPos))
                {
                    return OTokenKind.GuidLiteral;
                }
            }

            // TimeOfDay will have ":" in them
            if (CurrChar == ':' && TryParseTimeOfDay(tokenPos))
            {
                return OTokenKind.TimeOnlyLiteral;
            }

            // Guids will have alpha-numeric characters along with '-', so if a letter is encountered
            // try to see if this is Guid or not.
            if (char.IsLetter(CurrChar))
            {
                if (TryParseGuid(tokenPos))
                {
                    return OTokenKind.GuidLiteral;
                }
            }

            if (CurrChar == '.')
            {
                result = OTokenKind.DoubleLiteral;
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
                result = OTokenKind.DoubleLiteral;
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
                result = OTokenKind.DecimalLiteral;
                NextChar();
            }
            else if (CurrChar == 'd' || CurrChar == 'D')
            {
                result = OTokenKind.DoubleLiteral;
                NextChar();
            }
            else if (CurrChar == 'L' || CurrChar == 'l')
            {
                result = OTokenKind.Int64Literal;
                NextChar();
            }
            else if (CurrChar == 'f' || CurrChar == 'F')
            {
                result = OTokenKind.SingleLiteral;
                NextChar();
            }
            else
            {
                ReadOnlySpan<char> valueStr = Text.AsSpan().Slice(tokenPos, CurrPos - tokenPos);
                result = MakeBestGuessOnNoSuffixStr(valueStr, result);
            }
        }

        return result;
    }

    /// <summary>
    /// Read and skip white spaces
    /// </summary>
    protected void SkipWhitespace()
    {
        while (char.IsWhiteSpace(CurrChar))
        {
            NextChar();
        }
    }

    /// <summary>
    /// Advanced to the next character.
    /// </summary>
    protected void NextChar()
    {
        if (CurrPos < TextLen)
        {
            ++CurrPos;
            if (CurrPos >= 0 && CurrPos < Text.Length)
            {
                CurrChar = Text[CurrPos];
                return;
            }
        }

        CurrChar = '\0';
    }

    /// <summary>
    /// Sets the text position.
    /// </summary>
    /// <param name="pos">New text position.</param>
    protected void SetTextPos(int pos)
    {
        CurrPos = pos < 0 ? 0 : pos;
        CurrChar = CurrPos < TextLen ? Text[CurrPos] : '\0';
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
            CurrChar = this.Text[initialIndex];
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
            CurrChar = this.Text[initialIndex];
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
            CurrChar = this.Text[initialIndex];
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
            CurrChar = this.Text[initialIndex];
            return false;
        }
    }

    /// <summary>
    /// Parses a literal be checking for delimiting characters '\0', ',',')' and ' '
    /// </summary>
    /// <param name="tokenPos">Index from which the substring starts</param>
    /// <returns>Substring from this.text that has parsed the literal and ends in one of above delimiting characters</returns>
    private string ParseLiteral1(int tokenPos)
    {
        do
        {
            NextChar();
        }
        while (CurrPos != '\0' && CurrPos != ',' && CurrPos != ')' && CurrPos != ' ');

        if (CurrPos == CurrPos)
        {
            NextChar();
        }

        string numericStr = Text.Substring(tokenPos, CurrPos - tokenPos);
        return numericStr;
    }

    private ReadOnlySpan<char> ParseLiteral(int tokenPos)
    {
        do
        {
            NextChar();
        }
        while (CurrPos != '\0' && CurrPos != ',' && CurrPos != ')' && CurrPos != ' ');

        if (CurrPos == CurrPos)
        {
            NextChar();
        }

        var numericStr = Text.AsSpan().Slice(tokenPos, CurrPos - tokenPos);
        return numericStr;
    }

    /// <summary>
    /// Makes best guess on numeric string without trailing letter like L, F, M, D
    /// </summary>
    /// <param name="numericStr">The numeric string.</param>
    /// <param name="guessedKind">The possible kind (IntegerLiteral or DoubleLiteral) from ParseFromDigit() method.</param>
    /// <returns>A more accurate ExpressionTokenKind</returns>
    private static OTokenKind MakeBestGuessOnNoSuffixStr(ReadOnlySpan<char> numericStr, OTokenKind guessedKind)
    {
        // no suffix, so
        // (1) make a best guess (note: later we support promoting each to later one: int32->int64->single->double->decimal).
        // look at value:       "2147483647" may be Int32/long, "2147483649" must be long.
        // look at precision:   "3258.67876576549" may be single/double/decimal, "3258.678765765489753678965390" must be decimal.
        // (2) then let MetadataUtilsCommon.CanConvertPrimitiveTypeTo() method does further promotion when knowing expected semantics type.
        int tmpInt = 0;
        long tmpLong = 0;
        float tmpFloat = 0;
        double tmpDouble = 0;
        decimal tmpDecimal = 0;

        if (guessedKind == OTokenKind.IntegerLiteral)
        {
            if (int.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpInt))
            {
                return OTokenKind.IntegerLiteral;
            }

            if (long.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpLong))
            {
                return OTokenKind.Int64Literal;
            }
        }

        bool canBeSingle = float.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpFloat);
        bool canBeDouble = double.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpDouble);
        bool canBeDecimal = decimal.TryParse(numericStr, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tmpDecimal);

        // 1. try high precision -> low precision
        if (canBeDouble && canBeDecimal)
        {
            decimal doubleToDecimalR;
            decimal doubleToDecimalN;

            // To keep the full precision of the current value, which if necessary is all 17 digits of precision supported by the Double type.
            bool doubleCanBeDecimalR = decimal.TryParse(tmpDouble.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out doubleToDecimalR);

            // To cover the scientific notation case, such as 1e+19 in the tmpDouble
            bool doubleCanBeDecimalN = decimal.TryParse(tmpDouble.ToString("N29", CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleToDecimalN);

            if ((doubleCanBeDecimalR && doubleToDecimalR != tmpDecimal) || (!doubleCanBeDecimalR && doubleCanBeDecimalN && doubleToDecimalN != tmpDecimal))
            {
                // losing precision as double, so choose decimal
                return OTokenKind.DecimalLiteral;
            }
        }

        // here can't use normal casting like the above double VS decimal.
        // prevent losing precision in float -> double, e.g. (double)1.234f will be 1.2339999675750732d not 1.234d
        if (canBeSingle && canBeDouble && (double.Parse(tmpFloat.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) != tmpDouble))
        {
            // losing precision as single, so choose double
            return OTokenKind.DoubleLiteral;
        }

        // 2. try most compatible -> least compatible
        if (canBeSingle)
        {
            return OTokenKind.SingleLiteral;
        }

        if (canBeDouble)
        {
            return OTokenKind.DoubleLiteral;
        }

        throw new OTokenizationException(Error.Format(SRResources.Tokenization_InvalidNumericString, numericStr.ToString()));
    }

    /// <summary>
    /// Validates the current character is a digit.
    /// </summary>
    private void ValidateDigit()
    {
        if (!char.IsDigit(CurrChar))
        {
            throw new OTokenizationException(Error.Format(SRResources.Tokenization_DigitExpected, CurrPos, Text));
        }
    }
}
