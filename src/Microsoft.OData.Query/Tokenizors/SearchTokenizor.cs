//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query.Tokenizors
{
    public class SearchTokenizor : ISearchTokenizor
    {
        /// <summary>
        /// The current recursion depth.
        /// </summary>
        private int _recursionDepth;

        /// <summary>
        /// The lexer being used for the parsing.
        /// </summary>
        private IExpressionLexer lexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTokenizor" /> class.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        public SearchTokenizor(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        public int MaxDepth { get; }


        public SearchToken TokenizeSearch(string search)
        {
            _recursionDepth = 0;
            lexer = new SearchLexer(search);
            QueryToken token = ParseExpression();
            ValidateToken(ExpressionTokenKind.End);

            return new SearchToken(token);
        }

        private QueryToken ParseExpression()
        {
            EnterRecurse();
            QueryToken token = ParseLogicalOr();
            ExitRecurse();
            return token;
        }

        /// <summary>
        /// Parses the or operator.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseLogicalOr()
        {
            EnterRecurse();

            QueryToken left = this.ParseLogicalAnd();
            while (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordOr))
            {
                this.lexer.Next();
                QueryToken right = this.ParseLogicalAnd();
                left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
            }

            ExitRecurse();
            return left;
        }

        /// <summary>
        /// Parses the and operator.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseLogicalAnd()
        {
            EnterRecurse();
            QueryToken left = this.ParseUnary();
            while (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordAnd)
                || this.TokenIdentifierIs(ExpressionConstants.SearchKeywordNot)
                || this.lexer.Token.Kind == ExpressionTokenKind.StringLiteral
                || this.lexer.Token.Kind == ExpressionTokenKind.OpenParen)
            {
                // Handle A NOT B, A (B)
                // Bypass only when next token is AND
                if (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordAnd))
                {
                    this.lexer.Next();
                }

                QueryToken right = this.ParseUnary();
                left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
            }

            ExitRecurse();
            return left;
        }

        /// <summary>
        /// Parses the -, not unary operators.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseUnary()
        {
            EnterRecurse();
            if (this.TokenIdentifierIs(ExpressionConstants.SearchKeywordNot))
            {
                this.lexer.Next();
                QueryToken operand = this.ParseUnary();

                ExitRecurse();
                return new UnaryOperatorToken(UnaryOperatorKind.Not, operand);
            }

            ExitRecurse();
            return this.ParsePrimary();
        }

        /// <summary>
        /// Parses the primary expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParsePrimary()
        {
            QueryToken expr = null;
            EnterRecurse();

            switch (this.lexer.Token.Kind)
            {
                case ExpressionTokenKind.OpenParen:
                    expr = this.ParseParenExpression();
                    break;
                case ExpressionTokenKind.StringLiteral:
                    expr = new StringLiteralToken(this.lexer.Token.Text);
                    this.lexer.Next();
                    break;
                default:
                    throw new OException();
                   // throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            ExitRecurse();
            return expr;
        }

        /// <summary>
        /// Parses parenthesized expressions.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        private QueryToken ParseParenExpression()
        {
            if (this.lexer.Token.Kind != ExpressionTokenKind.OpenParen)
            {
                throw new OException();
                // throw ParseError(ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            this.lexer.Next();
            QueryToken result = this.ParseExpression();
            if (lexer.Token.Kind != ExpressionTokenKind.CloseParen)
            {
                throw new OException();
                //throw ParseError(ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
            }

            lexer.Next();
            return result;
        }

        /// <summary>
        /// Checks that the current token has the specified identifier.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <returns>true if the current token is an identifier with the specified text.</returns>
        private bool TokenIdentifierIs(string id)
        {
            return lexer.Token.IdentifierIs(id, false);
        }

        /// <summary>
        /// Marks the fact that a recursive method was entered, and checks that the depth is allowed.
        /// </summary>
        private void EnterRecurse()
        {
            _recursionDepth++;
            if (_recursionDepth > MaxDepth)
            {
                //throw new OException(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// Marks the fact that a recursive method is leaving.
        /// </summary>
        private void ExitRecurse()
        {
            --_recursionDepth;
        }

        /// <summary>Validates the current token is of the specified kind.</summary>
        /// <param name="t">Expected token kind.</param>
        private void ValidateToken(ExpressionTokenKind t)
        {
            if (lexer.Token.Kind != t)
            {
               // throw ParseError(ODataErrorStrings.ExpressionLexer_SyntaxError(this.textPos, this.Text));
            }
        }
    }
}
