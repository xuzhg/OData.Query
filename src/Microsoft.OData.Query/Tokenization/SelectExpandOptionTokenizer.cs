//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// The common methods to tokenize the $select and $expand query expression and produces the lexical object model.
/// </summary>
public abstract class SelectExpandOptionTokenizer : QueryTokenizer
{
    /// <summary>
    /// Tokenize the filter option in the select/expand option text.
    /// </summary>
    /// <returns>The filter option for select/expand</returns>
    protected virtual IQueryToken TokenizeInnerFilter(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        //string filterText = UriParserHelper.ReadQueryOption(this.lexer);

        //UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxFilterDepth, enableCaseInsensitiveBuiltinIdentifier);
        //return filterParser.ParseFilter(filterText);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize the orderby option in the select/expand option text.
    /// </summary>
    /// <returns>The orderby option for select/expand</returns>
    protected virtual IEnumerable<OrderByToken> TokenizeInnerOrderBy(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        //string orderByText = UriParserHelper.ReadQueryOption(this.lexer);

        //UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        //return orderbyParser.ParseOrderBy(orderByText);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse the top option in the select/expand option text.
    /// </summary>
    /// <returns>The top option for select/expand</returns>
    protected virtual long? TokenizeInnerTop(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        ReadOnlySpan<char> identifier = lexer.GetIdentifier();
        // QueryToken topToken = TokenizePrimary(lexer, context);

        long top;
        if (!long.TryParse(identifier, out top) || top < 0)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_InvalidTopOption(topText)");
        }

        lexer.NextToken();
        if (lexer.CurrentToken.Kind == ExpressionKind.SemiColon)
        {
            // Move over the ';' separator
            lexer.NextToken();
        }

        return top;
    }

    /// <summary>
    /// Tokenize the skip option in the select/expand option text.
    /// </summary>
    /// <returns>The skip option for select/expand</returns>
    protected virtual long? TokenizeInnerSkip(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        ReadOnlySpan<char> identifier = lexer.GetIdentifier();

        // TryParse requires a non-nullable non-negative long.
        long skip;
        if (!long.TryParse(identifier, out skip) || skip < 0)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText)");
        }

        lexer.NextToken();
        if (lexer.CurrentToken.Kind == ExpressionKind.SemiColon)
        {
            // Move over the ';' separator
            lexer.NextToken();
        }

        return skip;
    }

    /// <summary>
    /// Tokenize the count option in the select/expand option text.
    /// </summary>
    /// <returns>The count option for select/expand</returns>
    protected virtual bool? TokenizeInnerCount(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        ReadOnlySpan<char> identifier = lexer.GetIdentifier();

        if (!bool.TryParse(identifier, out bool result))
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText)");
        }

        lexer.NextToken();
        if (lexer.CurrentToken.Kind == ExpressionKind.SemiColon)
        {
            // Move over the ';' separator
            lexer.NextToken();
        }

        return result;
    }

    /// <summary>
    /// Tokenize the search option in the select/expand option text.
    /// </summary>
    /// <returns>The search option for select/expand</returns>
    protected virtual IQueryToken TokenizeInnerSearch(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // advance to the equal sign
        lexer.NextToken();

        // TODO: 
        ReadOnlySpan<char> identifier = lexer.GetIdentifier();

        lexer.NextToken();
        if (lexer.CurrentToken.Kind == ExpressionKind.SemiColon)
        {
            // Move over the ';' separator
            lexer.NextToken();
        }

        return null;
    }

    /// <summary>
    /// Tokenize the select option in the select/expand option text.
    /// </summary>
    /// <param name="pathToken">The path segment token</param>
    /// <returns>The select option for select/expand</returns>
    protected virtual SelectToken TokenizeInnerSelect(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize the expand option in the select/expand option text.
    /// </summary>
    /// <param name="pathToken">The path segment token</param>
    /// <returns>The expand option for select/expand</returns>
    protected virtual SelectToken TokenizeInnerExpand(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize the compute option in the expand option text.
    /// </summary>
    /// <returns>The compute option for expand</returns>
    protected virtual ComputeToken TokenizeInnerCompute(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        lexer.NextToken();
        //string computeText = UriParserHelper.ReadQueryOption(this.lexer);

        //UriQueryExpressionParser computeParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        //return computeParser.ParseCompute(computeText);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize the apply option in the expand option text.
    /// </summary>
    /// <returns>The apply option for expand</returns>
    protected virtual ApplyToken TokenizeInnerApply(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        lexer.NextToken();

        //string applyText = UriParserHelper.ReadQueryOption(this.lexer);

        //UriQueryExpressionParser applyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        //return applyParser.ParseApply(applyText);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parses a select or expand term into a PathSegmentToken.
    /// Assumes the lexer is positioned at the beginning of the term to parse.
    /// When done, the lexer will be positioned at whatever is after the identifier.
    /// </summary>
    /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
    /// <returns>parsed query token</returns>
    protected virtual SegmentToken TokenizePathSegment(IExpressionLexer lexer, QueryTokenizerContext context, bool allowRef = false)
    {
        int pathLength;
        SegmentToken token = TokenizeSegment(lexer, context, null, allowRef, true);
        if (token != null)
        {
            pathLength = 1;
        }
        else
        {
            return null;
        }

        CheckPathLength(pathLength);

        // If this property was a path, walk that path. e.g. SomeComplex/SomeInnerComplex/SomeNavProp
        while (lexer.CurrentToken.Kind == ExpressionKind.Slash)
        {
            // Move from '/' to the next segment
            lexer.NextToken();

            // TODO: Could remove V4 if we don't want to allow a trailing '/' character
            // Allow a single trailing slash for backwards compatibility with the WCF DS Server parser.
            if (pathLength > 1 && lexer.CurrentToken.Kind == ExpressionKind.EndOfInput)
            {
                break;
            }

            token = TokenizeSegment(lexer, context, token, allowRef, true);
            if (token != null)
            {
                CheckPathLength(++pathLength);
            }
        }

        return token;
    }

    /// <summary>
    /// Check that the current path length is less than the maximum path length
    /// </summary>
    /// <param name="pathLength">the current path length</param>
    private void CheckPathLength(int pathLength, int maxPathLength = int.MaxValue)
    {
        if (pathLength > maxPathLength)
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriQueryExpressionParser_TooDeep");
        }
    }

    /// <summary>
    /// Uses the ExpressionLexer to visit the next ExpressionToken, and delegates parsing of segments, type segments, identifiers,
    /// and the star token to other methods.
    /// </summary>
    /// <param name="previousSegment">Previously parsed PathSegmentToken, or null if this is the first token.</param>
    /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
    /// <returns>A parsed PathSegmentToken representing the next segment in this path.</returns>
    private SegmentToken TokenizeSegment(IExpressionLexer lexer, QueryTokenizerContext context, SegmentToken previousSegment, bool allowRef, bool isSelect)
    {
        if (lexer.CurrentToken.Text.StartsWith("$", StringComparison.Ordinal)
            && (!allowRef || lexer.CurrentToken.Text != "$ref")
            && lexer.CurrentToken.Text != "$count")
        {
            throw new QueryTokenizerException("ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand(lexer.CurrentToken.Text, lexer.ExpressionText)");
        }

        // Some check here to throw exception, prop1/*/prop2 and */$ref/prop and prop1/$count/prop2 will throw exception, all are $expand cases.
        if (!isSelect)
        {
            if (previousSegment != null && previousSegment.Identifier == "*" && lexer.GetIdentifier() != "$ref")
            {
                // Star can only be followed with $ref. $count is not supported with star as expand option
                throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_OnlyRefAllowWithStarInExpand");
            }
            else if (previousSegment != null && previousSegment.Identifier == "$ref")
            {
                // $ref should not have more property followed.
                throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_NoPropAllowedAfterRef");
            }
            else if (previousSegment != null && previousSegment.Identifier == "$count")
            {
                // $count should not have more property followed. e.g $expand=NavProperty/$count/MyProperty
                throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_NoPropAllowedAfterDollarCount");
            }
        }

        if (lexer.CurrentToken.Text == "$count" && isSelect)
        {
            // $count is not allowed in $select e.g $select=NavProperty/$count
            throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_DollarCountNotAllowedInSelect");
        }

        ReadOnlySpan<char> propertyName = default;

        lexer.TryPeekNextToken(out ExpressionToken nextToken);

        if (nextToken.Kind == ExpressionKind.Dot)
        {
            // propertyName = lexer.ReadDottedIdentifier(isSelect);
        }
        else if (lexer.CurrentToken.Kind == ExpressionKind.Star)
        {
            // "*/$ref" is supported in expand
            if (nextToken.Kind == ExpressionKind.Slash && isSelect)
            {
                throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_IdentifierExpected(lexer.Position)");
            }
            else if (previousSegment != null && !isSelect)
            {
                // expand option like "customer?$expand=VIPCustomer/*" is not allowed as specification does not allowed any property before *.
                throw new QueryTokenizerException("ODataErrorStrings.ExpressionToken_NoSegmentAllowedBeforeStarInExpand");
            }

            propertyName = lexer.GetIdentifier();
            lexer.NextToken();
        }
        else
        {
            propertyName = lexer.GetIdentifier();
            lexer.NextToken();
        }

        //  return new NonSystemToken(propertyName, null, previousSegment);
        SegmentToken segment = new SegmentToken(propertyName.ToString());
        if (previousSegment != null)
        {
            previousSegment.Next = segment;
        }

        return segment;
    }
}
