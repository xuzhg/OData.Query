using Microsoft.OData.Query.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Tokenizors
{
    public class SearchLexer : ExpressionLexer
    {
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

                    this.AdvanceToNextOccurenceOfWithEscape(quote);

                    if (this.textPos == this.TextLen)
                    {
                        throw ParseError(Strings.ExpressionLexer_UnterminatedStringLiteral(this.textPos, this.Text));
                    }

                    this.NextChar();

                    t = ExpressionTokenKind.StringLiteral;
                    break;
                default:
                    if (this.textPos == this.TextLen)
                    {
                        t = ExpressionTokenKind.End;
                    }
                    else
                    {
                        t = ExpressionTokenKind.Identifier;
                        do
                        {
                            this.NextChar();
                        } while (this.ch.HasValue && IsValidSearchTermChar(this.ch.Value));
                    }

                    break;
            }

            this.token.Kind = t;
            this.token.Text = this.Text.Substring(tokenPos, this.textPos - tokenPos);
            this.token.Position = tokenPos;

            if (this.token.Kind == ExpressionTokenKind.StringLiteral)
            {
                this.token.Text = this.token.Text.Substring(1, this.token.Text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
                if (string.IsNullOrEmpty(this.token.Text))
                {
                    throw ParseError(Strings.ExpressionToken_IdentifierExpected(this.token.Position));
                }
            }

            if ((this.token.Kind == ExpressionTokenKind.Identifier) && !KeyWords.Contains(this.token.Text))
            {
                Match match = InvalidWordPattern.Match(this.token.Text);
                if (match.Success)
                {
                    int index = match.Groups[0].Index;
                    throw ParseError(Strings.ExpressionLexer_InvalidCharacter(this.token.Text[index], this.token.Position + index, this.Text));
                }

                this.token.Kind = ExpressionTokenKind.StringLiteral;
            }

            return this.token;
        }

        /// <summary>
        /// Move to next char, with escape char support.
        /// </summary>
        private void NextCharWithEscape()
        {
            this.isEscape = false;
            this.NextChar();
            if (this.ch == EscapeChar)
            {
                this.isEscape = true;
                this.NextChar();

                if (!this.ch.HasValue || EscapeSequenceSet.IndexOf(this.ch.Value) < 0)
                {
                    throw ParseError(Strings.ExpressionLexer_InvalidEscapeSequence(this.ch, this.textPos, this.Text));
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
            while (_ch.HasValue && !(_ch == endingValue && !this.isEscape))
            {
                this.NextCharWithEscape();
            }
        }
    }
}
