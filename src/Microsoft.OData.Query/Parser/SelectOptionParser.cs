//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class SelectOptionParser : SelectExpandOptionParser, ISelectOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public SelectOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public virtual SelectToken ParseSelect(string select, QueryOptionParserContext context)
    {
        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(select, OTokenizerContext.Default);

        List<SelectItemToken> termTokens = new List<SelectItemToken>();

        // Move to the first token
        tokenizer.NextToken();

        // This happens if it was just whitespace. e.g. fake.svc/Customers?$expand=     &$filter=IsCool&$orderby=ID
        if (tokenizer.CurrentToken.Kind == OTokenKind.EndOfInput)
        {
            return new SelectToken(termTokens);
        }

        // Process first term
        termTokens.Add(ParseSingleSelectTerm(tokenizer, context));

        // If it was a list of terms, then commas will be separating them
        while (tokenizer.CurrentToken.Kind == OTokenKind.Comma)
        {
            // Move over the ',' to the next term
            tokenizer.NextToken();

            if (tokenizer.CurrentToken.Kind != OTokenKind.EndOfInput)
            {
                termTokens.Add(ParseSingleSelectTerm(tokenizer, context));
            }
            else
            {
                break;
            }
        }

        // If there isn't a comma, then we must be done. Otherwise there is a syntax error
        if (tokenizer.CurrentToken.Kind != OTokenKind.EndOfInput)
        {
            throw new OQueryParserException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
        }

        return new SelectToken(termTokens);
    }


    /// <summary>
    /// Parses a single term in a comma separated list of things to select.
    /// </summary>
    /// <returns>A token representing thing to select.</returns>
    protected virtual SelectItemToken ParseSingleSelectTerm(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        throw new NotImplementedException();
        //PathSegmentToken pathToken = ParsePathToken(tokenizer, true);

        //string optionsText = null;
        //if (tokenizer.CurrentToken.Kind == OTokenKind.OpenParen)
        //{
        //    optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();

        //    // Move lexer to what is after the parenthesis expression. Now CurrentToken will be the next thing.
        //    tokenizer.NextToken();
        //}

        //return this.SelectExpandOptionParser.BuildSelectTermToken(pathToken, optionsText);
    }
#if false
    /// <summary>
    /// Building off a PathSegmentToken, continue parsing any select options (nested $filter, $expand, etc)
    /// to build up an SelectTermToken which fully represents the tree that makes up this select term.
    /// </summary>
    /// <param name="pathToken">The PathSegmentToken representing the parsed select path whose options we are now parsing.</param>
    /// <param name="optionsText">A string of the text between the parenthesis after a select option.</param>
    /// <returns>The select term token based on the path token, and all available select options.</returns>
    internal SelectItemToken BuildSelectTermToken(IOTokenizer tokenizer, PathSegmentToken pathToken, string optionsText)
    {
        // Setup a new lexer for parsing the optionsText
        this.lexer = new ExpressionLexer(optionsText ?? "", true /*moveToFirstToken*/, true /*useSemicolonDelimiter*/);

        QueryToken filterOption = null;
        IEnumerable<OrderByToken> orderByOptions = null;
        long? topOption = null;
        long? skipOption = null;
        bool? countOption = null;
        QueryToken searchOption = null;
        SelectToken selectOption = null;
        ComputeToken computeOption = null;

        if (tokenizer.CurrentToken.Kind == OTokenKind.OpenParen)
        {
            // advance past the '('
            tokenizer.NextToken();

            // Check for (), which is not allowed.
            if (tokenizer.CurrentToken.Kind == OTokenKind.CloseParen)
            {
                throw new ODataException(ODataErrorStrings.UriParser_MissingSelectOption(pathToken.Identifier));
            }

            // Look for all the supported query options
            while (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
            {
                string text = this.enableCaseInsensitiveBuiltinIdentifier
                    ? tokenizer.CurrentToken.Text.ToLowerInvariant()
                    : tokenizer.CurrentToken.Text;

                // Prepend '$' prefix if needed.
                if (this.enableNoDollarQueryOptions && !text.StartsWith(UriQueryConstants.DollarSign, StringComparison.Ordinal))
                {
                    text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", UriQueryConstants.DollarSign, text);
                }

                switch (text)
                {
                    case TokenConstants.QueryOptionFilter: // inner $filter
                        filterOption = ParseInnerFilter();
                        break;

                    case TokenConstants.QueryOptionOrderby: // inner $orderby
                        orderByOptions = ParseInnerOrderBy();
                        break;

                    case TokenConstants.QueryOptionTop: // inner $top
                        topOption = ParseInnerTop();
                        break;

                    case TokenConstants.QueryOptionSkip: // innner $skip
                        skipOption = ParseInnerSkip();
                        break;

                    case TokenConstants.QueryOptionCount: // inner $count
                        countOption = ParseInnerCount();
                        break;

                    case TokenConstants.QueryOptionSearch: // inner $search
                        searchOption = ParseInnerSearch();
                        break;

                    case TokenConstants.QueryOptionSelect: // inner $select
                        selectOption = ParseInnerSelect(pathToken);
                        break;

                    case TokenConstants.QueryOptionCompute: // inner $compute
                        computeOption = ParseInnerCompute();
                        break;

                    default:
                        throw new OQueryParserException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
                }
            }

            // Move past the ')'
            tokenizer.NextToken();
        }

        // Either there was no '(' at all or we just read past the ')' so we should be at the end
        if (tokenizer.CurrentToken.Kind != OTokenKind.EndOfInput)
        {
            throw new OQueryParserException("ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText)");
        }

        return new SelectItemToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countOption, searchOption, selectOption, computeOption);
    }
}

public abstract class SelectExpandOptionParser
{
    protected virtual PathSegmentToken ParsePathToken(IOTokenizer tokenizer, bool isSelect)
    {
        PathSegmentToken token = ParseSegment(null, allowRef);

        // If this property was a path, walk that path. e.g. SomeComplex/SomeInnerComplex/SomeNavProp
        while (tokenizer.CurrentToken.Kind == OTokenKind.Slash)
        {
            // Move from '/' to the next segment
            tokenizer.NextToken();

            // TODO: Could remove V4 if we don't want to allow a trailing '/' character
            // Allow a single trailing slash for backwards compatibility with the WCF DS Server parser.
            if (pathLength > 1 && tokenizer.CurrentToken.Kind == OTokenKind.End)
            {
                break;
            }

            token = ParseSegment(token, allowRef);

            if (token != null)
            {
                CheckPathLength(++pathLength);
            }
        }

        return token;
    }

    /// <summary>
    /// Uses the ExpressionLexer to visit the next ExpressionToken, and delegates parsing of segments, type segments, identifiers,
    /// and the star token to other methods.
    /// </summary>
    /// <param name="previousSegment">Previously parsed PathSegmentToken, or null if this is the first token.</param>
    /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
    /// <returns>A parsed PathSegmentToken representing the next segment in this path.</returns>
    private PathSegmentToken ParseSegment(IOTokenizer tokenizer, PathSegmentToken previousSegment, bool allowRef, bool isSelect)
    {
        if (this.lexer.CurrentToken.Text.StartsWith("$", StringComparison.Ordinal)
            && (!allowRef || this.lexer.CurrentToken.Text != UriQueryConstants.RefSegment)
            && this.lexer.CurrentToken.Text != UriQueryConstants.CountSegment)
        {
            throw new ODataException(ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand(this.lexer.CurrentToken.Text, this.lexer.ExpressionText));
        }

        // Some check here to throw exception, prop1/*/prop2 and */$ref/prop and prop1/$count/prop2 will throw exception, all are $expand cases.
        if (!isSelect)
        {
            if (previousSegment != null && previousSegment.Identifier == UriQueryConstants.Star && this.lexer.CurrentToken.GetIdentifier() != UriQueryConstants.RefSegment)
            {
                // Star can only be followed with $ref. $count is not supported with star as expand option
                throw new ODataException(ODataErrorStrings.ExpressionToken_OnlyRefAllowWithStarInExpand);
            }
            else if (previousSegment != null && previousSegment.Identifier == UriQueryConstants.RefSegment)
            {
                // $ref should not have more property followed.
                throw new ODataException(ODataErrorStrings.ExpressionToken_NoPropAllowedAfterRef);
            }
            else if (previousSegment != null && previousSegment.Identifier == UriQueryConstants.CountSegment)
            {
                // $count should not have more property followed. e.g $expand=NavProperty/$count/MyProperty
                throw new ODataException(ODataErrorStrings.ExpressionToken_NoPropAllowedAfterDollarCount);
            }
        }

        if (this.lexer.CurrentToken.Text == UriQueryConstants.CountSegment && isSelect)
        {
            // $count is not allowed in $select e.g $select=NavProperty/$count
            throw new ODataException(ODataErrorStrings.ExpressionToken_DollarCountNotAllowedInSelect);
        }

        string propertyName;

        if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
        {
            propertyName = this.lexer.ReadDottedIdentifier(this.isSelect);
        }
        else if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
        {
            // "*/$ref" is supported in expand
            if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash && isSelect)
            {
                throw new ODataException(ODataErrorStrings.ExpressionToken_IdentifierExpected(this.lexer.Position));
            }
            else if (previousSegment != null && !isSelect)
            {
                // expand option like "customer?$expand=VIPCustomer/*" is not allowed as specification does not allowed any property before *.
                throw new ODataException(ODataErrorStrings.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
            }

            propertyName = this.lexer.CurrentToken.Text;
            this.lexer.NextToken();
        }
        else
        {
            propertyName = this.lexer.CurrentToken.GetIdentifier();
            this.lexer.NextToken();
        }

        return new NonSystemToken(propertyName, null, previousSegment);
    }

    /// <summary>
    /// Parse the filter option in the select/expand option text.
    /// </summary>
    /// <returns>The filter option for select/expand</returns>
    protected virtual QueryToken ParseInnerFilter(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();

        string filterText = UriParserHelper.ReadQueryOption(this.lexer);

        UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxFilterDepth, enableCaseInsensitiveBuiltinIdentifier);
        return filterParser.ParseFilter(filterText);
    }

    /// <summary>
    /// Parse the orderby option in the select/expand option text.
    /// </summary>
    /// <returns>The orderby option for select/expand</returns>
    private OrderByToken ParseInnerOrderBy(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string orderByText = UriParserHelper.ReadQueryOption(this.lexer);

        UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        return orderbyParser.ParseOrderBy(orderByText);
    }

    /// <summary>
    /// Parse the top option in the select/expand option text.
    /// </summary>
    /// <returns>The top option for select/expand</returns>
    protected virtual long? ParseInnerTop(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string topText = UriParserHelper.ReadQueryOption(this.lexer);

        // TryParse requires a non-nullable non-negative long.
        long top;
        if (!long.TryParse(topText, out top) || top < 0)
        {
            throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidTopOption(topText));
        }

        return top;
    }

    /// <summary>
    /// Parse the skip option in the select/expand option text.
    /// </summary>
    /// <returns>The skip option for select/expand</returns>
    protected virtual long? ParseInnerSkip(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string skipText = UriParserHelper.ReadQueryOption(this.lexer);

        // TryParse requires a non-nullable non-negative long.
        long skip;
        if (!long.TryParse(skipText, out skip) || skip < 0)
        {
            throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText));
        }

        return skip;
    }

    /// <summary>
    /// Parse the count option in the select/expand option text.
    /// </summary>
    /// <returns>The count option for select/expand</returns>
    protected virtual bool? ParseInnerCount(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string countText = UriParserHelper.ReadQueryOption(this.lexer);
        switch (countText)
        {
            case ExpressionConstants.KeywordTrue:
                return true;

            case ExpressionConstants.KeywordFalse:
                return false;

            default:
                throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidCountOption(countText));
        }
    }

    /// <summary>
    /// Parse the search option in the select/expand option text.
    /// </summary>
    /// <returns>The search option for select/expand</returns>
    protected virtual QueryToken ParseInnerSearch(IOTokenizer tokenizer)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string searchText = UriParserHelper.ReadQueryOption(this.lexer);

        SearchParser searchParser = new SearchParser(this.MaxSearchDepth);
        return searchParser.ParseSearch(searchText);
    }

    /// <summary>
    /// Parse the select option in the select/expand option text.
    /// </summary>
    /// <param name="pathToken">The path segment token</param>
    /// <returns>The select option for select/expand</returns>
    protected virtual SelectToken ParseInnerSelect(IOTokenizer tokenizer, PathSegmentToken pathToken)
    {
        // advance to the equal sign
        tokenizer.NextToken();
        string selectText = UriParserHelper.ReadQueryOption(this.lexer);

        IEdmStructuredType targetStructuredType = null;
        if (this.resolver != null && this.parentStructuredType != null)
        {
            var parentProperty = this.resolver.ResolveProperty(parentStructuredType, pathToken.Identifier);

            // It is a property, need to find the type.
            // or for select query like: $select=Address($expand=City)
            if (parentProperty != null)
            {
                targetStructuredType = parentProperty.Type.ToStructuredType();
            }
        }

        SelectExpandParser innerSelectParser = new SelectExpandParser(
            resolver,
            selectText,
            targetStructuredType,
            this.maxRecursionDepth - 1,
            this.enableCaseInsensitiveBuiltinIdentifier,
            this.enableNoDollarQueryOptions);

        return innerSelectParser.ParseSelect();
    }

    /// <summary>
    /// Parse the compute option in the expand option text.
    /// </summary>
    /// <returns>The compute option for expand</returns>
    protected virtual ComputeToken ParseInnerCompute(IOTokenizer tokenizer)
    {
        tokenizer.NextToken();
        string computeText = UriParserHelper.ReadQueryOption(this.lexer);

        UriQueryExpressionParser computeParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        return computeParser.ParseCompute(computeText);
    }
#endif 
}