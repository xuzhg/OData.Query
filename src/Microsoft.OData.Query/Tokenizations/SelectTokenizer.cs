//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $select query expression and produces the lexical object model.
/// </summary>
public class SelectTokenizer : SelectExpandTokenizer, ISelectTokenizer
{
    /// <summary>
    /// Tokenizes the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The select token tokenized.</returns>
    public virtual async ValueTask<SelectToken> TokenizeAsync(ReadOnlyMemory<char> select, QueryTokenizerContext context)
    {
        if (select.IsEmpty)
        {
            throw new ArgumentNullException(nameof(select));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = context.CreateLexer(select);
        lexer.NextToken(); // move to first token

        SelectToken selectToken = new SelectToken();

        // This happens if it was just whitespace. e.g. ~/odata/Customers?$select=     &...
        if (lexer.CurrentToken.Kind == ExpressionKind.EndOfInput)
        {
            return selectToken;
        }

        // Process first term
        selectToken.Add(TokenizeSelectItem(lexer, context));

        // If it was a list of terms, then commas will be separating them
        while (lexer.CurrentToken.Kind == ExpressionKind.Comma)
        {
            // Move over the ',' to the next item
            lexer.NextToken();

            if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput)
            {
                selectToken.Add(TokenizeSelectItem(lexer, context));
            }
            else
            {
                break;
            }
        }

        // If there isn't a comma, then we must be done. Otherwise there is a syntax error
        if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_TermIsNotValid(lexer.ExpressionText)");
        }

        return await ValueTask.FromResult(selectToken);
    }

    /// <summary>
    /// Tokenizes a single term in a comma separated list of things to select.
    /// </summary>
    /// <returns>A token representing thing to select.</returns>
    protected virtual SelectItemToken TokenizeSelectItem(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        /*
selectItem     = STAR
               / allOperationsInSchema 
               / [ ( qualifiedEntityTypeName / qualifiedComplexTypeName ) "/" ] 
                 ( selectProperty
                 / qualifiedActionName  
                 / qualifiedFunctionName  
                 )
         * */
        //var termParser = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect);
        SegmentToken pathToken = TokenizePathSegment(lexer, context, isSelect: true);

        // QueryToken selectItem = TokenizePrimary(lexer, context);

        IQueryToken filterOption = null;
        IEnumerable<OrderByToken> orderByOptions = null;
        long? topOption = null;
        long? skipOption = null;
        bool? countOption = null;
        IQueryToken searchOption = null;
        SelectToken selectOption = null;
        ComputeToken computeOption = null;

      //  string optionsText = null;
        if (lexer.CurrentToken.Kind == ExpressionKind.OpenParen)
        {
            //optionsText = lexer.AdvanceThroughBalancedParentheticalExpression();

            // advance past the '('
            lexer.NextToken();

            // Check for (), which is not allowed.
            if (lexer.CurrentToken.Kind == ExpressionKind.CloseParen)
            {
                throw new QueryTokenizerException("ODataErrorStrings.UriParser_MissingSelectOption(pathToken.Identifier)");
            }

            while (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
            {
                string text = context.EnableCaseInsensitive
                   ? lexer.CurrentToken.Text.ToString().ToLowerInvariant()
                   : lexer.CurrentToken.Text.ToString();

                // Prepend '$' prefix if needed.
                if (context.EnableNoDollarPrefix && !text.StartsWith("$", StringComparison.Ordinal))
                {
                    text = $"${text}";
                }

                switch (text)
                {
                    case "$filter": // inner $filter
                        filterOption = TokenizeInnerFilter(lexer, context);
                        break;

                    case "$orderby": // inner $orderby
                        orderByOptions = TokenizeInnerOrderBy(lexer, context);
                        break;

                    case "$top": // inner $top
                        topOption = TokenizeInnerTop(lexer, context);
                        break;

                    case "$skip": // inner $skip
                        skipOption = TokenizeInnerSkip(lexer, context);
                        break;

                    case "$count": // inner $count
                        countOption = TokenizeInnerCount(lexer, context);
                        break;

                    case "$search": // inner $search
                        searchOption = TokenizeInnerSearch(lexer, context);
                        break;

                    case "$select": // inner $select
                        selectOption = TokenizeInnerSelect(lexer, context);
                        break;

                    case "$compute": // inner $compute
                        computeOption = TokenizeInnerCompute(lexer, context);
                        break;

                    default:
                        throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_TermIsNotValid(lexer.ExpressionText)");
                }
            }

            // Verify
            if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
            {
                throw new QueryTokenizerException("ODataErrorStrings.UriParser_MissingSelectOption(pathToken.Identifier)");
            }

            // Move past the ')'
            lexer.NextToken();
        }

        return new SelectItemToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countOption, searchOption, selectOption, computeOption);
     //   return this.SelectExpandOptionParser.BuildSelectTermToken(pathToken, optionsText);
    }
}
