//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query.Tokenizors
{
    public class FilterTokenizor : IFilterTokenizor
    {
        /// <summary>
        /// The current recursion depth.
        /// </summary>
        private int _recursionDepth;

        public virtual FilterToken TokenizeFilter(string filter)
        {
            _recursionDepth = 0;
            //this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(expressionText);
            //QueryToken result = ParseExpression();
            //this.lexer.ValidateToken(ExpressionTokenKind.End);

            //return result;

            return null;
        }

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <returns>The lexical token representing the expression.</returns>
        internal QueryToken ParseExpression()
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
            //QueryToken left = this.ParseLogicalAnd();
            //while (this.TokenIdentifierIs(ExpressionConstants.KeywordOr))
            //{
            //    this.lexer.NextToken();
            //    QueryToken right = this.ParseLogicalAnd();
            //    left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
            //}

            ExitRecurse();
            return null;
        }

        /// <summary>
        /// Marks the fact that a recursive method was entered, and checks that the depth is allowed.
        /// </summary>
        private void EnterRecurse()
        {
            _recursionDepth++;
            //if (_recursionDepth > MaxDepth)
            //{
            //    //throw new OException(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
            //}
        }

        /// <summary>
        /// Marks the fact that a recursive method is leaving.
        /// </summary>
        private void ExitRecurse()
        {
            --_recursionDepth;
        }
    }
}
