//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// The common methods to tokenize the $select and $expand query expression and produces the query token object model.
/// </summary>
public abstract class SelectExpandTokenizer : QueryTokenizer
{
    /// <summary>
    /// Tokenizes the filter option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the orderby option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the top option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the skip option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the count option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the search option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the select option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
    /// <returns>The select option for select/expand</returns>
    protected virtual SelectToken TokenizeInnerSelect(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenizes the expand option in the select/expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
    /// <returns>The expand option for select/expand</returns>
    protected virtual ExpandToken TokenizeInnerExpand(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenizes the compute option in the expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes the apply option in the expand option text.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
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
    /// Tokenizes a select or expand term into a <see cref="SegmentToken">.
    /// Assumes the lexer is positioned at the beginning of the term to parse.
    /// When done, the lexer will be positioned at whatever is after the identifier.
    /// </summary>
    /// <param name="lexer">The lexer for the query tokenizer.</param>
    /// <param name="context">The context for the query tokenizer.</param>
    /// <param name="isSelect">Whether this is a $select operation.</param>
    /// <returns>parsed query token</returns>
    protected virtual SegmentToken TokenizePathSegment(IExpressionLexer lexer, QueryTokenizerContext context, bool isSelect)
    {
        int pathLength;
        SegmentToken token = TokenizeSegment(lexer, context, null, isSelect);
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

            token = TokenizeSegment(lexer, context, token, isSelect);
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
    private SegmentToken TokenizeSegment(IExpressionLexer lexer, QueryTokenizerContext context, SegmentToken previousSegment, bool isSelect)
    {
        ReadOnlySpan<char> span = lexer.CurrentToken.Span;

        if (span.StartsWith("$", StringComparison.Ordinal)
            && (!isSelect || !span.Equals("$ref", context.GetStringComparison()))
            && !span.Equals("$count", context.GetStringComparison()))
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_InvalidSegmentTokenInSelectExpand, lexer.CurrentToken.Text, isSelect ? "$select" : "$expand"));
        }

        // Some check here to throw exception, prop1/*/prop2 and */$ref/prop and prop1/$count/prop2 will throw exception, all are $expand cases.
        if (!isSelect)
        {
            if (previousSegment != null && previousSegment.Identifier == "*" && !span.Equals("$ref", context.GetStringComparison()))
            {
                // Star can only be followed with $ref. $count is not supported with star as expand option
                throw new QueryTokenizerException(SRResources.QueryTokenizer_OnlyDollarRefAllowedWithStarInExpand);
            }
            else if (previousSegment != null && previousSegment.Identifier.Equals("$ref", context.GetStringComparison()))
            {
                // $ref should not have more property followed.
                throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_NoSegmentAllowedAfterSegment, lexer.CurrentToken.Text, "$ref"));
            }
            else if (previousSegment != null && previousSegment.Identifier == "$count")
            {
                // $count should not have more property followed. e.g $expand=NavProperty/$count/MyProperty
                throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_NoSegmentAllowedAfterSegment, lexer.CurrentToken.Text, "$count"));
            }
        }

        ReadOnlyMemory<char> propertyName = default;

        lexer.PeekNextToken(out ExpressionToken nextToken);

        if (nextToken.Kind == ExpressionKind.Dot)
        {
            propertyName = lexer.ReadDottedIdentifier(isSelect);
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

            propertyName = lexer.CurrentToken.Text;
            lexer.NextToken();
        }
        else
        {
            propertyName = lexer.CurrentToken.Text;
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
