//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class ApplyParserContext : QueryOptionParserContext
{
    private int _parseAggregateExpressionDepth;

    public void IncreaseAggDepth() => ++_parseAggregateExpressionDepth;

    public void DecreaseAggDepth() => --_parseAggregateExpressionDepth;
}

public class ApplyOptionParser : QueryOptionParser, IApplyOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public ApplyOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public virtual ApplyToken ParseApply(string apply, ApplyParserContext context)
    {
        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(apply, OTokenizerContext.Default);

        context.EnterRecurse();

        List<QueryToken> transformationTokens = new List<QueryToken>();

        while (true)
        {
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAggregate, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseAggregate(tokenizer, context));
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordFilter, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseFilter(tokenizer, context));
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordGroupBy, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseGroupBy(tokenizer, context));
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseCompute(tokenizer, context));
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordExpand, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseExpand(tokenizer, context));
            }
            else
            {
                // ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                throw new OQueryParserException("TODO: ");
            }

            // '/' indicates there are more transformations
            if (tokenizer.CurrentToken.Kind != OTokenKind.Slash)
            {
                break;
            }

            tokenizer.NextToken();
        }

        context.LeaveRecurse();

        tokenizer.ValidateToken(OTokenKind.EndOfInput);

        return new ApplyToken(transformationTokens);
    }

    // parses $apply aggregate transformation (.e.g. aggregate(UnitPrice with sum as TotalUnitPrice))
    protected virtual AggregateToken ParseAggregate(IOTokenizer tokenizer, ApplyParserContext context)
    {
       // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordAggregate), "token identifier is aggregate");

        tokenizer.NextToken();

        return new AggregateToken(ParseAggregateExpressions(tokenizer, context));
    }

    internal List<AggregateTokenBase> ParseAggregateExpressions(IOTokenizer tokenizer, ApplyParserContext context)
    {
        // '('
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        // series of statements separates by commas
        List<AggregateTokenBase> statements = new List<AggregateTokenBase>();
        while (true)
        {
            statements.Add(ParseAggregateExpression(tokenizer, context));

            if (tokenizer.CurrentToken.Kind != OTokenKind.Comma)
            {
                break;
            }

            tokenizer.NextToken();
        }

        // ")"
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        return statements;
    }

    internal AggregateTokenBase ParseAggregateExpression(IOTokenizer tokenizer, ApplyParserContext context)
    {
        //try
        //{
        //    context.IncreaseAggDepth();

        //    // expression
        //    QueryToken expression = ParseLogicalOr(tokenizer, context);

        //    if (tokenizer.CurrentToken.Kind == OTokenKind.OpenParen)
        //    {
        //        // When there's a parenthesis after the expression we have a entity set aggregation.
        //        // The syntax is the same as the aggregate expression itself, so we recurse on ParseAggregateExpressions.
        //        this.aggregateExpressionParents.Push(expression);
        //        List<AggregateTokenBase> statements = ParseAggregateExpressions();
        //        this.aggregateExpressionParents.Pop();

        //        return new EntitySetAggregateToken(expression, statements);
        //    }

        //    AggregationMethodDefinition verb;

        //    // "with" verb
        //    EndPathToken endPathExpression = expression as EndPathToken;
        //    if (endPathExpression != null && endPathExpression.Identifier == TokenConstants.QueryOptionCount)
        //    {
        //        // e.g. aggregate($count as Count)
        //        verb = AggregationMethodDefinition.VirtualPropertyCount;
        //    }
        //    else
        //    {
        //        // e.g. aggregate(UnitPrice with sum as Total)
        //        verb = this.ParseAggregateWith();
        //    }

        //    // "as" alias
        //    StringLiteralToken alias = ParseAggregateAs();

        //    return new AggregateExpressionToken(expression, verb, alias.Text);
        //}
        //finally
        //{
        //    this.parseAggregateExpressionDepth--;
        //}
        throw new NotImplementedException();
    }

    /// <summary>
    /// parses $apply groupby transformation (.e.g. groupby(ProductID, CategoryId, aggregate(UnitPrice with sum as TotalUnitPrice))
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual GroupByToken ParseGroupBy(IOTokenizer tokenizer, ApplyParserContext context)
    {
      //  Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordGroupBy), "token identifier is groupby");
        tokenizer.NextToken();

        // '('
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        // '('
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        // properties
        var properties = new List<EndPathToken>();
        while (true)
        {
            var expression = ParsePrimary(tokenizer, context) as EndPathToken;

            if (expression == null)
            {
                throw new OQueryParserException("(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
            }

            properties.Add(expression);

            if (tokenizer.CurrentToken.Kind != OTokenKind.Comma)
            {
                break;
            }

            tokenizer.NextToken();
        }

        // ")"
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        // optional child transformation
        ApplyTransformationToken transformationToken = null;

        // "," (comma)
        if (tokenizer.CurrentToken.Kind == OTokenKind.Comma)
        {
            tokenizer.NextToken();

            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAggregate, context.EnableIdentifierCaseSensitive))
            {
                transformationToken = ParseAggregate(tokenizer, context);
            }
            else
            {
                throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(ExpressionConstants.KeywordAggregate, this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
            }
        }

        // ")"
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        return new GroupByToken(properties, transformationToken);
    }

    /// <summary>
    /// parses $apply filter transformation (.e.g. filter(ProductName eq 'Aniseed Syrup'))
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual QueryToken ParseFilter(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordFilter), "token identifier is filter");
        tokenizer.NextToken();

        // '(' expression ')'
        // return ParseParenExpression(tokenizer, context);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parses $apply compute expression (.e.g. compute(UnitPrice mul SalesPrice as computePrice)
    /// </summary>
    /// <returns></returns>
    protected virtual ComputeToken ParseCompute(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        Debug.Assert(tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive), "token identifier is compute");

        tokenizer.NextToken();

        // '('
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

        while (true)
        {
            ComputeExpressionToken computed = ParseComputeExpression(tokenizer, context);

            transformationTokens.Add(computed);

            if (tokenizer.CurrentToken.Kind != OTokenKind.Comma)
            {
                break;
            }

            tokenizer.NextToken();
        }

        // ")"
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        return new ComputeToken(transformationTokens);
    }

    /// <summary>
    /// Parse compute expression text into a token.
    /// </summary>
    /// <returns>The lexical token representing the compute expression text.</returns>
    internal ComputeExpressionToken ParseComputeExpression(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        // expression
        QueryToken expression = ParseExpression(tokenizer, context);

        // "as" alias
        // StringLiteralToken alias = ParseAggregateAs(tokenizer, context);

        // return new ComputeExpressionToken(expression, alias.Text);

        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual ExpandToken ParseExpand(IOTokenizer tokenizer, ApplyParserContext context)
    {
        //Debug.Assert(TokenIdentifierIs(TokenConstants.KeywordExpand), "token identifier is expand");

        tokenizer.NextToken();

        // '('
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();

        List<ExpandItemToken> termTokens = new List<ExpandItemToken>();

        // First token must be Path
        //var termParser = new SelectExpandTermParser(this.lexer, this.maxDepth - 1, false);
        PathSegmentToken pathToken = null;// Parse(allowRef: true);

        QueryToken filterToken = null;
        ExpandToken nestedExpand = null;

        // Followed (optionally) by filter and expand
        // Syntax for expand inside $apply is different (and much simpler)  from $expand clause => had to use different parsing approach
        while (tokenizer.CurrentToken.Kind == OTokenKind.Comma)
        {
            tokenizer.NextToken();

            if (tokenizer.CurrentToken.Kind == OTokenKind.Identifier)
            {
                switch (tokenizer.GetIdentifier().ToString())
                {
                    case TokenConstants.KeywordFilter:
                        filterToken = ParseFilter(tokenizer, context);
                        break;
                    case TokenConstants.KeywordExpand:
                        ExpandToken tempNestedExpand = ParseExpand(tokenizer, context);
                        nestedExpand = nestedExpand == null
                            ? tempNestedExpand
                            : new ExpandToken(nestedExpand.ExpandItems.Concat(tempNestedExpand.ExpandItems));
                        break;
                    default:
                        throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
                }
            }
        }

        // Leaf level expands require filter
        if (filterToken == null && nestedExpand == null)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_InnerMostExpandRequireFilter(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        ExpandItemToken expandTermToken = new ExpandItemToken(pathToken, filterToken, null, null, null, null, null, null, null, nestedExpand);
        termTokens.Add(expandTermToken);

        // ")"
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        tokenizer.NextToken();


        return new ExpandToken(termTokens);
    }
}
