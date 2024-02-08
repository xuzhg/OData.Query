//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $apply query expression and produces the lexical object model.
/// </summary>
public class ApplyOptionTokenizer : QueryTokenizer, IApplyOptionTokenizer
{
    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public ApplyOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize the $apply expression.
    /// </summary>
    /// <param name="apply">The $apply expression string to Tokenize.</param>
    /// <returns>The order by token tokenized.</returns>
    public virtual ApplyToken ParseApply(string apply, QueryTokenizerContext context)
    {
        Debug.Assert(apply != null, "apply != null");

        IExpressionLexer lexer = _lexerFactory.CreateLexer(apply, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        IList<QueryToken> transformationTokens = new List<QueryToken>();

        context.EnterRecurse();

        while (true)
        {
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAggregate, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseAggregate(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordFilter, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseFilter(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordGroupBy, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseGroupBy(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseCompute(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordExpand, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(ParseExpand(lexer, context));
            }
            else
            {
                // ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                throw new OQueryParserException("TODO: ");
            }

            // '/' indicates there are more transformations
            if (lexer.CurrentToken.Kind != ExpressionKind.Slash)
            {
                break;
            }

            lexer.NextToken();
        }

        context.LeaveRecurse();

        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return new ApplyToken(transformationTokens);
    }

    // parses $apply aggregate transformation (.e.g. aggregate(UnitPrice with sum as TotalUnitPrice))
    protected virtual AggregateToken ParseAggregate(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordAggregate), "token identifier is aggregate");
        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // series of statements separates by commas
        List<AggregateTokenBase> statements = new List<AggregateTokenBase>();
        while (true)
        {
            statements.Add(ParseAggregateExpression(lexer, context));

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new AggregateToken(statements);
    }

    internal AggregateTokenBase ParseAggregateExpression(IExpressionLexer lexer, QueryTokenizerContext context)
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
    protected virtual GroupByToken ParseGroupBy(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        //  Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordGroupBy), "token identifier is groupby");
        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // properties
        var properties = new List<EndPathToken>();
        while (true)
        {
            var expression = ParsePrimary(lexer, context) as EndPathToken;

            if (expression == null)
            {
                throw new OQueryParserException("(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
            }

            properties.Add(expression);

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // optional child transformation
        ApplyTransformationToken transformationToken = null;

        // "," (comma)
        if (lexer.CurrentToken.Kind == ExpressionKind.Comma)
        {
            lexer.NextToken();

            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAggregate, context.EnableIdentifierCaseSensitive))
            {
                transformationToken = ParseAggregate(lexer, context);
            }
            else
            {
                throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(ExpressionConstants.KeywordAggregate, this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
            }
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new GroupByToken(properties, transformationToken);
    }

    /// <summary>
    /// parses $apply filter transformation (.e.g. filter(ProductName eq 'Aniseed Syrup'))
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual QueryToken ParseFilter(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordFilter), "token identifier is filter");
        lexer.NextToken();

        // '(' expression ')'
        // return ParseParenExpression(tokenizer, context);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parses $apply compute expression (.e.g. compute(UnitPrice mul SalesPrice as computePrice)
    /// </summary>
    /// <returns></returns>
    protected virtual ComputeToken ParseCompute(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        Debug.Assert(lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive), "token identifier is compute");

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

        while (true)
        {
            ComputeExpressionToken computed = ParseComputeExpression(lexer, context);

            transformationTokens.Add(computed);

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new ComputeToken(transformationTokens);
    }

    /// <summary>
    /// Parse compute expression text into a token.
    /// </summary>
    /// <returns>The lexical token representing the compute expression text.</returns>
    internal ComputeExpressionToken ParseComputeExpression(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // expression
        QueryToken expression = ParseExpression(lexer, context);

        // "as" alias
        // StringLiteralToken alias = ParseAggregateAs(tokenizer, context);

        // return new ComputeExpressionToken(expression, alias.Text);

        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual ExpandToken ParseExpand(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        //Debug.Assert(TokenIdentifierIs(TokenConstants.KeywordExpand), "token identifier is expand");

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        List<ExpandItemToken> termTokens = new List<ExpandItemToken>();

        // First token must be Path
        //var termParser = new SelectExpandTermParser(this.lexer, this.maxDepth - 1, false);
        PathSegmentToken pathToken = null;// Parse(allowRef: true);

        QueryToken filterToken = null;
        ExpandToken nestedExpand = null;

        // Followed (optionally) by filter and expand
        // Syntax for expand inside $apply is different (and much simpler)  from $expand clause => had to use different parsing approach
        while (lexer.CurrentToken.Kind == ExpressionKind.Comma)
        {
            lexer.NextToken();

            if (lexer.CurrentToken.Kind == ExpressionKind.Identifier)
            {
                switch (lexer.GetIdentifier().ToString())
                {
                    case TokenConstants.KeywordFilter:
                        filterToken = ParseFilter(lexer, context);
                        break;
                    case TokenConstants.KeywordExpand:
                        ExpandToken tempNestedExpand = ParseExpand(lexer, context);
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
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new ExpandToken(termTokens);
    }
}
