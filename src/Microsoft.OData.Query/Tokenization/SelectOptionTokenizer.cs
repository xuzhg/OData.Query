//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $select query expression and produces the lexical object model.
/// </summary>
public class SelectOptionTokenizer : SelectExpandOptionTokenizer, ISelectOptionTokenizer
{
    internal static SelectOptionTokenizer Default = new SelectOptionTokenizer(ExpressionLexerFactory.Default);

    private ILexerFactory _lexerFactory;

    public SelectOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize the $select expression.
    /// </summary>
    /// <param name="select">The $select expression string to tokenize.</param>
    /// <returns>The select token tokenized.</returns>
    public virtual async ValueTask<SelectToken> TokenizeAsync(string select, QueryTokenizerContext context)
    {
        IExpressionLexer lexer = _lexerFactory.CreateLexer(select, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        List<SelectItemToken> itemTokens = new List<SelectItemToken>();

        // This happens if it was just whitespace. e.g. fake.svc/Customers?$select=     &...
        if (lexer.CurrentToken.Kind == ExpressionKind.EndOfInput)
        {
            return new SelectToken(itemTokens);
        }

        // Process first term
        itemTokens.Add(TokenizeSelectItem(lexer, context));

        // If it was a list of terms, then commas will be separating them
        while (lexer.CurrentToken.Kind == ExpressionKind.Comma)
        {
            // Move over the ',' to the next term
            lexer.NextToken();

            if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput)
            {
                itemTokens.Add(TokenizeSelectItem(lexer, context));
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

        return await ValueTask.FromResult(new SelectToken(itemTokens));
    }

    /// <summary>
    /// Parses a single term in a comma separated list of things to select.
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
        SegmentToken pathToken = TokenizePathSegment(lexer, context);

        // QueryToken selectItem = TokenizePrimary(lexer, context);

        QueryToken filterOption = null;
        IEnumerable<OrderByToken> orderByOptions = null;
        long? topOption = null;
        long? skipOption = null;
        bool? countOption = null;
        QueryToken searchOption = null;
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
                string text = context.EnableIdentifierCaseSensitive
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
