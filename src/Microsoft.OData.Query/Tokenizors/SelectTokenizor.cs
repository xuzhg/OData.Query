//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tokens;

namespace Microsoft.OData.Query.Tokenizors
{
    public class SelectTokenizor : ISelectTokenizor
    {
        public virtual SelectToken TokenizeSelect(string select)
        {
            IExpressionLexer lexer = new ExpressionLexer(select);

            List<SelectItemToken> itemTokens = new List<SelectItemToken>();

            // This happens if it was just whitespace. e.g. ~/Customers?$select=     $top=2
            if (lexer.Token.Kind == ExpressionTokenKind.End)
            {
                return new SelectToken(itemTokens);
            }

            // Add the first select item
            itemTokens.Add(ParseSingleSelectItem());

            while (lexer.Token.Kind == ExpressionTokenKind.Comma)
            {
                lexer.Next(); // Move over the ',' to the next term

                if (lexer.Token.Kind != ExpressionTokenKind.End)
                {
                    itemTokens.Add(ParseSingleSelectItem());
                }
                else
                {
                    break;
                }
            }

            if (lexer.Token.Kind != ExpressionTokenKind.End)
            {
                throw new OException();
                // throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            return new SelectToken(itemTokens);
        }

        /// <summary>
        /// Parses a single term in a comma separated list of things to select.
        /// </summary>
        /// <returns>A token representing thing to select.</returns>
        protected virtual SelectItemToken ParseSingleSelectItem()
        {
            //var termParser = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect);
            //PathSegmentToken pathToken = termParser.ParseTerm();

            //string optionsText = null;
            //if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            //{
            //    optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();

            //    // Move lexer to what is after the parenthesis expression. Now CurrentToken will be the next thing.
            //    this.lexer.NextToken();
            //}

            //return this.SelectExpandOptionParser.BuildSelectTermToken(pathToken, optionsText);
            return null;
        }
    }
}
