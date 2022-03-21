//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.OData.Query.Parser
{
    /// <summary>
    /// Use this class to parse an expression in to token.
    /// </summary>
    public class ExpressionLexer : IExpressionLexer
    {
        /// <summary>
        /// For an identifier, EMD supports chars that match the regex  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
        /// IsLetterOrDigit covers Ll, Lu, Lt, Lo, Lm, Nd, this set covers the rest
        /// </summary>
        private static readonly HashSet<UnicodeCategory> AdditionalUnicodeCategoriesForIdentifier = new HashSet<UnicodeCategory>(new UnicodeCategoryEqualityComparer())
        {
            UnicodeCategory.LetterNumber,
            UnicodeCategory.NonSpacingMark,
            UnicodeCategory.SpacingCombiningMark,
            UnicodeCategory.ConnectorPunctuation, // covers "_"
            UnicodeCategory.Format
        };

        /// <summary>Length of raw input text being parsed.</summary>
        protected int _length { get; }

        /// <summary>Character being processed.</summary>
        protected char? _ch;

        /// <summary>Position on text being parsed.</summary>
        protected int _textPos;

        protected ExpressionToken _token;

        /// <summary>
        /// Initializes a new <see cref="ExpressionLexer"/>.
        /// </summary>
        /// <param name="expression">The expression to lexer.</param>
        public ExpressionLexer(string expression)
            : this(expression, settings: null)
        { }

        /// <summary>
        /// Initializes a new <see cref="ExpressionLexer"/>.
        /// </summary>
        /// <param name="expression">The expression to lexer.</param>
        /// <param name="settings">The expression settings.</param>
        /// <exception cref="ArgumentNullException">Exception if expresion is null or empty.</exception>
        public ExpressionLexer(string expression, ExpressionLexerSettings settings)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            RawText = expression;
            _length = expression.Length;
            Settings = settings ?? ExpressionLexerSettings.Default;

            SetTextPos(0);
            Next();
        }

        public string RawText { get; }

        public ExpressionLexerSettings Settings { get; }

        protected char? CurrentChar => _ch;

        protected int CurrentPos => _textPos;

        /// <summary>
        /// Tests whether the current char is whitespace.
        /// </summary>
        protected bool IsValidWhitespace => _ch != null && char.IsWhiteSpace(_ch.Value);

        /// <summary>
        /// Tests whether the current char is digit.
        /// </summary>
        protected bool IsValidDigit => _ch != null && char.IsDigit(_ch.Value);

        /// <summary>
        /// Is the current char a valid starting char for an identifier.
        /// Valid starting chars for identifier include all that are supported by EDM ([\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]) and '_'.
        /// </summary>
        private bool IsValidStartingCharForIdentifier => _ch != null && (
            char.IsLetter(_ch.Value) ||       // IsLetter covers: Ll, Lu, Lt, Lo, Lm
            _ch == '_' ||
            _ch == '$' ||
            char.GetUnicodeCategory(_ch.Value) == UnicodeCategory.LetterNumber);

        /// <summary>
        /// Is the current char a valid non-starting char for an identifier.
        /// Valid non-starting chars for identifier include all that are supported
        /// by EDM  [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}].
        /// This list includes '_', which is ConnectorPunctuation (Pc)
        /// </summary>
        private bool IsValidNonStartingCharForIdentifier => _ch != null && (
                    char.IsLetterOrDigit(_ch.Value) ||    // covers: Ll, Lu, Lt, Lo, Lm, Nd
                    AdditionalUnicodeCategoriesForIdentifier.Contains(char.GetUnicodeCategory(_ch.Value)));  // covers the rest

        /// <summary>
        /// Token being processed.
        /// </summary>
        public ExpressionToken Token { get => _token; set => _token = value; }

        public virtual bool Next()
        {
            Exception error = null;
            NextTokenImpl(out error);

            return error != null;
        }

        private ExpressionTokenKind ParseOpenParenthesis()
        {
            NextChar();

            if (Token.Text == "in")
            {
                AdvanceThroughBalancedExpression('(', ')');
                return ExpressionTokenKind.ParenthesesExpression;
            }
            else
            {
                return ExpressionTokenKind.OpenParen;
            }
        }

        private ExpressionTokenKind ParseMinus(int tokenPos)
        {
            bool hasNext = _textPos + 1 < _length;
            if (hasNext && Char.IsDigit(RawText[_textPos + 1]))
            {
                // don't separate '-' and its following digits : -2147483648 is valid int.MinValue, but 2147483648 is long.
                ExpressionTokenKind t = ParseFromDigit();
                if (ExpressionLexerUtils.IsNumeric(t))
                {
                    return t;
                }

                // If it looked like a numeric but wasn't (because it was a binary 0x... value for example),
                // we'll rewind and fall through to a simple '-' token.
                this.SetTextPos(tokenPos);
            }
            else if (hasNext && RawText[_textPos + 1] == 'I') // 'I' is the first charactor in "INF"
            {
                NextChar();
                ParseIdentifier();
                string currentIdentifier = RawText.Substring(tokenPos + 1, _textPos - tokenPos - 1);

                if (ExpressionLexerUtils.IsInfinityLiteral(currentIdentifier))
                {
                    return ExpressionTokenKind.NegativeInfinityLiteral;
                }

                // If it looked like '-INF' but wasn't we'll rewind and fall through to a simple '-' token.
                this.SetTextPos(tokenPos);
            }

            this.NextChar();
            return ExpressionTokenKind.Minus;
        }

        protected virtual ExpressionToken NextTokenImpl(out Exception error)
        {
            error = null;

            if (Settings.IgnoreWhitespace)
            {
                ParseWhitespace();
            }

            ExpressionTokenKind t;

            int tokenPos = _textPos; // save the current token position
            switch (_ch)
            {
                case '(':
                    t = ParseOpenParenthesis();
                    break;

                case ')':
                    NextChar();
                    t = ExpressionTokenKind.CloseParen;
                    break;

                case ',':
                    NextChar();
                    t = ExpressionTokenKind.Comma;
                    break;
                case '=':
                    NextChar();
                    t = ExpressionTokenKind.Equal;
                    break;
                case '/':
                    NextChar();
                    t = ExpressionTokenKind.Slash;
                    break;
                case '?':
                    NextChar();
                    t = ExpressionTokenKind.Question;
                    break;
                case '.':
                    NextChar();
                    t = ExpressionTokenKind.Dot;
                    break;
                case '*':
                    NextChar();
                    t = ExpressionTokenKind.Star;
                    break;
                case ':':
                    NextChar();
                    t = ExpressionTokenKind.Colon;
                    break;
                case '-':
                    t = ParseMinus(tokenPos);
                    break;

                case '\'':
                    char quote = _ch.Value;
                    do
                    {
                        AdvanceToNextOccurenceOf(quote);

                        if (_textPos == _length)
                        {
                            // Create error
                            //error = ParseError(ODataErrorStrings.ExpressionLexer_UnterminatedStringLiteral(this.textPos, this.Text));
                        }

                        NextChar();
                    }
                    while (_ch.HasValue && (_ch.Value == quote));
                    t = ExpressionTokenKind.StringLiteral;
                    break;

                case '{':
                    NextChar();
                    AdvanceThroughBalancedExpression('{', '}');
                    t = ExpressionTokenKind.BracedExpression;
                    break;
                case '[':
                    NextChar();
                    AdvanceThroughBalancedExpression('[', ']');
                    t = ExpressionTokenKind.BracketedExpression;
                    break;

                default:
                    if (IsValidWhitespace)
                    {
                        ParseWhitespace();
                        t = ExpressionTokenKind.Whitespace;
                        break;
                    }

                    if (IsValidNonStartingCharForIdentifier)
                    {
                        ParseIdentifier();

                        // now, the lexer maybe move to hit a '-', so, it could be a GUID
                        if (_ch == '-' && TryParseGuid(tokenPos))
                        {
                            t = ExpressionTokenKind.GuidLiteral;
                            break;
                        }

                        t = ExpressionTokenKind.Identifier;
                        break;
                    }

                    if (IsValidDigit)
                    {
                        t = ParseFromDigit();
                        break;
                    }

                    if (_textPos == _length)
                    {
                        t = ExpressionTokenKind.End;
                        break;
                    }

                    if (Settings.UseSemicolonDelimiter && _ch == ';')
                    {
                        NextChar();
                        t = ExpressionTokenKind.SemiColon;
                        break;
                    }

                    if (_ch == '@')
                    {
                        NextChar();

                        if (_textPos == _length)
                        {
                            // create the error
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        if (!IsValidStartingCharForIdentifier)
                        {
                            // create the error
                            t = ExpressionTokenKind.Unknown;
                            break;
                        }

                        int start = _textPos;

                        ParseIdentifier(includingDots: true);

                        string leftToken = RawText.Substring(start, _textPos - start);

                        t = Settings.ParsingFunctionParameters && !leftToken.Contains('.') ?
                            ExpressionTokenKind.ParameterAlias :
                            ExpressionTokenKind.Identifier;
                        break;
                    }

                    // Create Error
                    t = ExpressionTokenKind.Unknown;
                    break;
            };

            _token.Kind = t;
            _token.Text = RawText.Substring(tokenPos, _textPos - tokenPos);
            _token.Position = tokenPos;

            //this.HandleTypePrefixedLiterals();
            return _token;
        }

        /// <summary>
        /// Move lexer to next character.
        /// </summary>
        protected virtual void NextChar()
        {
            if (_textPos < _length)
            {
                ++_textPos;
                if (_textPos < _length)
                {
                    _ch = RawText[_textPos];
                    return;
                }
            }

            _ch = null;
        }

        /// <summary>
        /// Parses a token that starts with a digit.
        /// </summary>
        /// <returns>The kind of token recognized.</returns>
        private ExpressionTokenKind ParseFromDigit()
        {
            Debug.Assert(IsValidDigit || (_ch == '-'), "this.IsValidDigit || (this.ch == '-')");

            ExpressionTokenKind result;
            int tokenPos = _textPos;
            char startChar = _ch.Value;
            NextChar();

            // 0x...
            if (startChar == '0' && (_ch == 'x' || _ch == 'X'))
            {
                result = ExpressionTokenKind.BinaryLiteral;
                do
                {
                    this.NextChar();
                }
                while (_ch.HasValue && ParserHelper.IsCharHexDigit(_ch.Value));
            }
            else
            {
                result = ExpressionTokenKind.IntegerLiteral;
                while (this.IsValidDigit)
                {
                    this.NextChar();
                }

                // DateTimeOffset, DateOnly and Guids will have '-' in them
                if (_ch == '-')
                {
                    if (TryParseDateOnly(tokenPos))
                    {
                        return ExpressionTokenKind.DateOnlyLiteral;
                    }
                    else if (TryParseDateTimeoffset(tokenPos))
                    {
                        return ExpressionTokenKind.DateTimeOffsetLiteral;
                    }
                    else if (TryParseGuid(tokenPos))
                    {
                        return ExpressionTokenKind.GuidLiteral;
                    }
                }

                // TimeOnly will have ":" in them
                if (_ch == ':')
                {
                    if (TryParseTimeOnly(tokenPos))
                    {
                        return ExpressionTokenKind.TimeOnlyLiteral;
                    }
                }

                // Guids will have alpha-numeric characters along with '-', so if a letter is encountered
                // try to see if this is Guid or not.
                if (_ch.HasValue && char.IsLetter(_ch.Value))
                {
                    if (TryParseGuid(tokenPos))
                    {
                        return ExpressionTokenKind.GuidLiteral;
                    }
                }

                if (_ch == '.')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    NextChar();
                    this.ValidateDigit();

                    do
                    {
                        this.NextChar();
                    }
                    while (this.IsValidDigit);
                }

                if (_ch == 'E' || _ch == 'e')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    this.NextChar();
                    if (_ch == '+' || _ch == '-')
                    {
                        this.NextChar();
                    }

                    this.ValidateDigit();
                    do
                    {
                        this.NextChar();
                    }
                    while (this.IsValidDigit);
                }

                if (_ch == 'M' || _ch == 'm')
                {
                    result = ExpressionTokenKind.DecimalLiteral;
                    this.NextChar();
                }
                else if (_ch == 'd' || _ch == 'D')
                {
                    result = ExpressionTokenKind.DoubleLiteral;
                    this.NextChar();
                }
                else if (_ch == 'L' || _ch == 'l')
                {
                    result = ExpressionTokenKind.Int64Literal;
                    this.NextChar();
                }
                else if (_ch == 'f' || _ch == 'F')
                {
                    result = ExpressionTokenKind.SingleLiteral;
                    this.NextChar();
                }
                else
                {
                    string valueStr = RawText.Substring(tokenPos, _textPos - tokenPos);
                    result = ExpressionLexerUtils.MakeBestGuessOnNoSuffixStr(valueStr, result);
                }
            }

            return result;
        }

        /// <summary>Parses an identifier by advancing the current character.</summary>
        /// <param name="includingDots">Optional flag for whether to include dots as part of the identifier.</param>
        private void ParseIdentifier(bool includingDots = false)
        {
            Debug.Assert(IsValidStartingCharForIdentifier || _ch == QueryConstants.AnnotationPrefix, "Expected valid starting char for identifier");
            do
            {
                this.NextChar();
            }
            while (IsValidNonStartingCharForIdentifier || (includingDots && _ch == '.'));
        }

        /// <summary>
        /// Tries to parse Guid from current text
        /// If it's not Guid, then this.textPos and this.ch are reset
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a Guid, false otherwise</returns>
        private bool TryParseGuid(int tokenPos)
        {
            int initialIndex = _textPos;

            string guidStr = ParseLiteral(tokenPos);
            if (Guid.TryParse(guidStr, out _))
            {
                return true;
            }
            else
            {
                _textPos = initialIndex;
                _ch = RawText[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Tries to parse Guid from current text
        /// If it's not Guid, then this.textPos and this.ch are reset
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a Guid, false otherwise</returns>
        private bool TryParseDateTimeoffset(int tokenPos)
        {
            int initialIndex = _textPos;

            string datetimeOffsetStr = ParseLiteral(tokenPos);

            if (DateTimeOffset.TryParse(datetimeOffsetStr, out _))
            {
                return true;
            }
            else
            {
                _textPos = initialIndex;
                _ch = RawText[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Tries to parse Date from current text
        /// If it's not Date (DateOnly), then this.textPos and this.ch are reset.
        /// We use new struct 'DateOnly' introduced since .NET 6.
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a Date (DateOnly), false otherwise</returns>
        private bool TryParseDateOnly(int tokenPos)
        {
            int initialIndex = _textPos;
            string dateStr = ParseLiteral(tokenPos);
            if (DateOnly.TryParse(dateStr, out _))
            {
                return true;
            }
            else
            {
                _textPos = initialIndex;
                _ch = RawText[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Tries to parse TimeOnly from current text
        /// If it's not TimeOnly, then this.textPos and this.ch are reset.
        /// We use new struct 'TimeOnly' introduced since .NET 6.
        /// </summary>
        /// <param name="tokenPos">Start index</param>
        /// <returns>True if the substring that starts from tokenPos is a TimeOnly, false otherwise</returns>
        private bool TryParseTimeOnly(int tokenPos)
        {
            int initialIndex = _textPos;

            string timeOfDayStr = ParseLiteral(tokenPos);
            if (TimeOnly.TryParse(timeOfDayStr, out _))
            {
                return true;
            }
            else
            {
                _textPos = initialIndex;
                _ch = RawText[initialIndex];
                return false;
            }
        }

        /// <summary>
        /// Parses a literal be checking for delimiting characters '\0', ',',')' and ' '
        /// </summary>
        /// <param name="tokenPos">Index from which the substring starts</param>
        /// <returns>Substring from this.text that has parsed the literal and ends in one of above delimiting characters</returns>
        private string ParseLiteral(int tokenPos)
        {
            do
            {
                this.NextChar();
            }
            while (_ch.HasValue && _ch != ',' && _ch != ')' && _ch != ' ');

            if (_ch == null)
            {
                this.NextChar();
            }

            string numericStr = RawText.Substring(tokenPos,_textPos - tokenPos);
            return numericStr;
        }

        /// <summary>
        /// Parses an expression of text that we do not know how to handle in this class, which is between a
        /// <paramref name="startingCharacter"></paramref> and an <paramref name="endingCharacter"/>.
        /// </summary>
        /// <param name="startingCharacter">the starting delimiter</param>
        /// <param name="endingCharacter">the ending delimiter.</param>
        private void AdvanceThroughBalancedExpression(char startingCharacter, char endingCharacter)
        {
            int currentBracketDepth = 1;

            bool parsingDoubleQuotedString = false;
            while (currentBracketDepth > 0)
            {
                if (_ch == '"')
                {
                    parsingDoubleQuotedString = DoubleQuotedStringCheckpoint(parsingDoubleQuotedString);
                }

                if (_ch == '\'' && !parsingDoubleQuotedString)
                {
                    AdvanceToNextOccurenceOf('\'');
                }

                if (_ch == startingCharacter)
                {
                    currentBracketDepth++;
                }
                else if (_ch == endingCharacter)
                {
                    currentBracketDepth--;
                }

                if (_ch == null)
                {
                    throw new OException(string.Format(CultureInfo.InvariantCulture, OResource.ExpressionLexer_UnbalancedRangeExpression, endingCharacter));
                }

                this.NextChar();
            }
        }

        /// <summary>
        /// Advance the pointer to the next occurence of the given value, swallowing all characters in between.
        /// </summary>
        /// <param name="endingValue">the ending delimiter.</param>
        protected void AdvanceToNextOccurenceOf(char endingValue)
        {
            NextChar();
            while (_ch != null && (_ch.Value != endingValue))
            {
                NextChar();
            }
        }

        /// <summary>
        /// Parses white spaces
        /// </summary>
        protected virtual void ParseWhitespace()
        {
            while (IsValidWhitespace)
            {
                this.NextChar();
            }
        }

        /// <summary>
        /// Sets the text position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        protected void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _length ? RawText[_textPos] : (char?)null;
        }

        private bool DoubleQuotedStringCheckpoint(bool parsingDoubleQuotedString)
        {
            if (_textPos != 0 && RawText[_textPos - 1] != '\\')
            {
                return !parsingDoubleQuotedString;
            }
            else
            {
                return parsingDoubleQuotedString;
            }
        }

        /// <summary>Validates the current character is a digit.</summary>
        private void ValidateDigit()
        {
            if (!IsValidDigit)
            {
                throw new OException(string.Format(CultureInfo.InvariantCulture, OResource.ExpressionLexer_DigitExpected, _textPos, RawText));
            }
        }

        /// <summary>This class implements IEqualityComparer for UnicodeCategory</summary>
        /// <remarks>
        /// Using this class rather than EqualityComparer&lt;T&gt;.Default
        /// saves from JIT'ing it in each AppDomain.
        /// </remarks>
        private sealed class UnicodeCategoryEqualityComparer : IEqualityComparer<UnicodeCategory>
        {
            /// <summary>
            /// Checks whether two unicode categories are equal
            /// </summary>
            /// <param name="x">first unicode category</param>
            /// <param name="y">second unicode category</param>
            /// <returns>true if they are equal, false otherwise</returns>
            public bool Equals(UnicodeCategory x, UnicodeCategory y)
            {
                return x == y;
            }

            /// <summary>
            /// Gets a hash code for the specified unicode category
            /// </summary>
            /// <param name="obj">the input value</param>
            /// <returns>The hash code for the given input unicode category, the underlying int</returns>
            public int GetHashCode(UnicodeCategory obj)
            {
                return (int)obj;
            }
        }
    }
}
