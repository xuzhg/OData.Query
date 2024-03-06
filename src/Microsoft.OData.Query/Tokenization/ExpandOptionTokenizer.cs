//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $expand query expression and produces the lexical object model.
/// </summary>
public class ExpandOptionTokenizer : SelectExpandOptionTokenizer, IExpandOptionTokenizer
{
    private ILexerFactory _lexerFactory;

    public ExpandOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize the $expand expression.
    /// </summary>
    /// <param name="expand">The $expand expression string to tokenize.</param>
    /// <returns>The expand token tokenized.</returns>
    public virtual async ValueTask<ExpandToken> TokenizeAsync(string expand, QueryTokenizerContext context)
    {
        IExpressionLexer lexer = _lexerFactory.CreateLexer(expand, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        List<ExpandItemToken> itemTokens = new List<ExpandItemToken>();
        ExpandItemToken starItemToken = null;

        // This happens if it was just whitespace. e.g. ~/Customers?$expand=     &...
        if (lexer.CurrentToken.Kind == ExpressionKind.EndOfInput)
        {
            return new ExpandToken(itemTokens);
        }

        // Process first term
        if (lexer.CurrentToken.Kind == ExpressionKind.Star)
        {
            starItemToken = TokenizeExpandItem(lexer, context);

            // ???
            itemTokens.Add(starItemToken);
        }
        else
        {
            itemTokens.Add(TokenizeExpandItem(lexer, context));
        }

        // If it was a list of terms, then commas will be separating them
        while (lexer.CurrentToken.Kind == ExpressionKind.Comma)
        {
            // Move over the ',' to the next term
            lexer.NextToken();

            if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput && lexer.CurrentToken.Kind != ExpressionKind.Star)
            {
                itemTokens.Add(TokenizeExpandItem(lexer, context));
            }
            else if (lexer.CurrentToken.Kind == ExpressionKind.Star)
            {
                // Multiple stars is not allowed here.
                if (starItemToken != null)
                {
                    throw new QueryTokenizerException("ODataErrorStrings.UriExpandParser_TermWithMultipleStarNotAllowed(this.lexer.ExpressionText)");
                }

                starItemToken = TokenizeExpandItem(lexer, context);
            }
            else
            {
                break;
            }
        }

        // If there isn't a comma, then we must be done. Otherwise there is a syntax error
        if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
        }

        return await ValueTask.FromResult(new ExpandToken(itemTokens));
    }

    /// <summary>
    /// Parses a single term in a comma separated list of things to expand.
    /// </summary>
    /// <returns>A token representing thing to expand.</returns>
    protected virtual ExpandItemToken TokenizeExpandItem(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        SegmentToken pathToken = TokenizePathSegment(lexer, context);

        QueryToken filterOption = null;
        IEnumerable<OrderByToken> orderByOptions = null;
        long? topOption = null;
        long? skipOption = null;
        bool? countOption = null;
        long? levelsOption = null;
        QueryToken searchOption = null;
        SelectToken selectOption = null;
        ExpandToken expandOption = null;
        ComputeToken computeOption = null;
        ApplyToken applyOptions = null;

        if (lexer.CurrentToken.Kind == ExpressionKind.OpenParen)
        {
            // advance past the '('
            lexer.NextToken();

            // Check for (), which is not allowed.
            if (lexer.CurrentToken.Kind == ExpressionKind.CloseParen)
            {
                throw new QueryTokenizerException("ODataErrorStrings.UriParser_MissingExpandOption(pathToken.Identifier)");
            }

            // Look for all the supported query options
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

                    case "$level": // inner $level
                        levelsOption = TokenizeInnerLevel(lexer, context);
                        break;

                    case "$select": // inner $select
                        selectOption = TokenizeInnerSelect(lexer, context);
                        break;

                    case "$expand": // inner $expand
                        expandOption = TokenizeInnerExpand(lexer, context);
                        break;

                    case "$compute": // inner $compute
                        computeOption = TokenizeInnerCompute(lexer, context);
                        break;

                    case "$apply": // inner $apply
                        applyOptions = TokenizeInnerApply(lexer, context);
                        break;

                    default:
                        throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
                }
            }

            // Move past the ')'
            lexer.NextToken();
        }

        // Either there was no '(' at all or we just read past the ')' so we should be at the end
        if (lexer.CurrentToken.Kind != ExpressionKind.EndOfInput)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
        }

        // TODO, there should be some check here in case pathToken identifier is $ref, select, expand and levels options are not allowed.
        ExpandItemToken currentToken = new ExpandItemToken(pathToken, filterOption, orderByOptions, topOption,
            skipOption, countOption, levelsOption, searchOption, selectOption, expandOption, computeOption, applyOptions);

        return currentToken;
    }

    /// <summary>
    /// Parse the level option in the expand option text.
    /// </summary>
    /// <returns>The level option for expand in long type</returns>
    protected virtual long? TokenizeInnerLevel(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        long? levelsOption = null;

        // advance to the equal sign
        lexer.NextToken();

        //string levelsText = UriParserHelper.ReadQueryOption(this.lexer);
        //long level;

        //if (string.Equals(
        //    ExpressionConstants.KeywordMax,
        //    levelsText,
        //    this.enableCaseInsensitiveBuiltinIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
        //{
        //    levelsOption = long.MinValue;
        //}
        //else if (!long.TryParse(levelsText, NumberStyles.None, CultureInfo.InvariantCulture, out level) || level < 0)
        //{
        //    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidLevelsOption(levelsText));
        //}
        //else
        //{
        //    levelsOption = level;
        //}

        //return levelsOption;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse the expand option in the select/expand option text.
    /// </summary>
    /// <param name="pathToken">The path segment token</param>
    /// <returns>The expand option for select/expand</returns>
    protected virtual ExpandToken TokenizeInnerExpand(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        //string expandText = UriParserHelper.ReadQueryOption(this.lexer);

        //IEdmStructuredType targetStructuredType = null;
        //if (this.resolver != null && this.parentStructuredType != null)
        //{
        //    var parentProperty = this.resolver.ResolveProperty(parentStructuredType, pathToken.Identifier);

        //    // it is a property, need to find the type.
        //    // Like $expand=Friends($expand=Trips($expand=*)), when expandText becomes "Trips($expand=*)",
        //    // find navigation property Trips of Friends, then get Entity type of Trips.
        //    // or for select query like: $select=Address($expand=City)
        //    if (parentProperty != null)
        //    {
        //        targetStructuredType = parentProperty.Type.ToStructuredType();
        //    }
        //}

        //SelectExpandParser innerExpandParser = new SelectExpandParser(
        //    resolver,
        //    expandText,
        //    targetStructuredType,
        //    this.maxRecursionDepth - 1,
        //    this.enableCaseInsensitiveBuiltinIdentifier,
        //    this.enableNoDollarQueryOptions);

        //return innerExpandParser.ParseExpand();

        throw new NotImplementedException();
    }
}
