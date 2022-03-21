//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Query.Tokenizors
{
    public class SearchLexer : ExpressionLexer
    {
        /// <summary>
        /// Pattern for searchWord
        /// From ABNF rule:
        /// searchWord   = 1*ALPHA ; Actually: any character from the Unicode categories L or Nl,
        ///               ; but not the words AND, OR, and NOT
        ///
        /// \p{L} means any kind of letter from any language, include [Lo] such as CJK single character.
        /// </summary>
        internal static readonly Regex InvalidWordPattern = new Regex(@"([^\p{L}\p{Nl}])");

        /// <summary>
        /// Escape character used in search query
        /// </summary>
        private const char EscapeChar = '\\';

        /// <summary>
        /// Characters that could be escaped
        /// </summary>
        private const string EscapeSequenceSet = "\\\"";

        /// <summary>
        /// Keeps all keywords can be used in search query.
        /// </summary>
        private static readonly HashSet<string> KeyWords = new HashSet<string>(StringComparer.Ordinal) { ExpressionConstants.SearchKeywordAnd, ExpressionConstants.SearchKeywordOr, ExpressionConstants.SearchKeywordNot };

        /// <summary>
        /// Indicate whether current char is escaped.
        /// </summary>
        private bool _isEscape;

        public SearchLexer(string expression)
            : base(expression)
        {
        }

        public SearchLexer(string expression, ExpressionLexerSettings settings)
            : base(expression, settings)
        {
        }

        protected override ExpressionToken NextTokenImpl(out Exception error)
        {
            error = null;

            this.ParseWhitespace();

            ExpressionTokenKind t;
            int tokenPos = _textPos;
            switch (_ch)
            {
                case '(':
                    NextChar();
                    t = ExpressionTokenKind.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = ExpressionTokenKind.CloseParen;
                    break;
                case '"':
                    char quote =_ch.Value;

                    AdvanceToNextOccurenceOfWithEscape(quote);

                    if (_textPos == _length)
                    {
                        throw new OException();
                        //throw ParseError(Strings.ExpressionLexer_UnterminatedStringLiteral(this.textPos, this.Text));
                    }

                    NextChar();

                    t = ExpressionTokenKind.StringLiteral;
                    break;
                default:
                    if (_textPos == _length)
                    {
                        t = ExpressionTokenKind.End;
                    }
                    else
                    {
                        t = ExpressionTokenKind.Identifier;
                        do
                        {
                            this.NextChar();
                        } while (_ch.HasValue && IsValidSearchTermChar(_ch.Value));
                    }

                    break;
            }

            _token.Kind = t;
            _token.Text = RawText.Substring(tokenPos, _textPos - tokenPos);
            _token.Position = tokenPos;

            if (_token.Kind == ExpressionTokenKind.StringLiteral)
            {
                _token.Text = _token.Text.Substring(1, _token.Text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
                if (string.IsNullOrEmpty(_token.Text))
                {
                    throw new OException();
                    //throw ParseError(Strings.ExpressionToken_IdentifierExpected(this.token.Position));
                }
            }

            if ((_token.Kind == ExpressionTokenKind.Identifier) && !KeyWords.Contains(_token.Text))
            {
                Match match = InvalidWordPattern.Match(_token.Text);
                if (match.Success)
                {
                    int index = match.Groups[0].Index;

                    throw new OException();
                    //throw ParseError(Strings.ExpressionLexer_InvalidCharacter(this.token.Text[index], this.token.Position + index, this.Text));
                }

                _token.Kind = ExpressionTokenKind.StringLiteral;
            }

            return _token;
        }

        /// <summary>
        /// Evaluate whether the given char is valid for a SearchTerm
        /// </summary>
        /// <param name="val">The char to be evaluated on.</param>
        /// <returns>Whether the given char is valid for a SearchTerm</returns>
        private static bool IsValidSearchTermChar(char val) => !char.IsWhiteSpace(val) && val != ')';

        /// <summary>
        /// Move to next char, with escape char support.
        /// </summary>
        private void NextCharWithEscape()
        {
            _isEscape = false;
            NextChar();
            if (_ch == EscapeChar)
            {
                _isEscape = true;
                NextChar();

                if (!_ch.HasValue || EscapeSequenceSet.IndexOf(_ch.Value) < 0)
                {
                    throw new OException();
                    //throw ParseError(Strings.ExpressionLexer_InvalidEscapeSequence(this.ch, this.textPos, this.Text));
                }
            }
        }

        /// <summary>
        /// Advance to certain char, with escape char support.
        /// </summary>
        /// <param name="endingValue">the ending delimiter.</param>
        private void AdvanceToNextOccurenceOfWithEscape(char endingValue)
        {
            this.NextCharWithEscape();
            while (_ch.HasValue && !(_ch == endingValue && !_isEscape))
            {
                this.NextCharWithEscape();
            }
        }
    }
}
