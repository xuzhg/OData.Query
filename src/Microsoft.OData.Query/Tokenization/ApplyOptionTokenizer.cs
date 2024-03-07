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
    internal static ApplyOptionTokenizer Default = new ApplyOptionTokenizer(ExpressionLexerFactory.Default);

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
    public virtual async ValueTask<ApplyToken> TokenizeAsync(string apply, QueryTokenizerContext context)
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
                transformationTokens.Add(TokenizeAggregate(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordFilter, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(TokenizeFilter(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordGroupBy, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(TokenizeGroupBy(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(TokenizeCompute(lexer, context));
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordExpand, context.EnableIdentifierCaseSensitive))
            {
                transformationTokens.Add(TokenizeExpand(lexer, context));
            }
            else
            {
                // ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText));
                throw new QueryParserException("TODO: ");
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

        return await ValueTask.FromResult(new ApplyToken(transformationTokens));
    }

    // Tokenize $apply aggregate transformation (.e.g. aggregate(UnitPrice with sum as TotalUnitPrice))
    protected virtual AggregateToken TokenizeAggregate(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordAggregate), "token identifier is aggregate");
        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // series of statements separates by commas
        List<AggregateTokenBase> statements = new List<AggregateTokenBase>();
        while (true)
        {
            statements.Add(TokenizeAggregateExpression(lexer, context));

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new AggregateToken(statements);
    }

    internal AggregateTokenBase TokenizeAggregateExpression(IExpressionLexer lexer, QueryTokenizerContext context)
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
    /// Tokenize $apply groupby transformation (.e.g. groupby(ProductID, CategoryId, aggregate(UnitPrice with sum as TotalUnitPrice))
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual GroupByToken TokenizeGroupBy(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        //  Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordGroupBy), "token identifier is groupby");
        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        // properties
        var properties = new List<EndPathToken>();
        while (true)
        {
            var expression = TokenizePrimary(lexer, context) as EndPathToken;

            if (expression == null)
            {
                throw new QueryParserException("(ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
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
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrOperatorExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
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
                transformationToken = TokenizeAggregate(lexer, context);
            }
            else
            {
                throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(ExpressionConstants.KeywordAggregate, this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
            }
        }

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new GroupByToken(properties, transformationToken);
    }

    /// <summary>
    /// Tokenize $apply filter transformation (.e.g. filter(ProductName eq 'Aniseed Syrup'))
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual QueryToken TokenizeFilter(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // Debug.Assert(TokenIdentifierIs(ExpressionConstants.KeywordFilter), "token identifier is filter");
        lexer.NextToken();

        // '(' expression ')'
        // return ParseParenExpression(tokenizer, context);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize $apply compute expression (.e.g. compute(UnitPrice mul SalesPrice as computePrice)
    /// </summary>
    /// <returns></returns>
    protected virtual ComputeToken TokenizeCompute(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        Debug.Assert(lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordCompute, context.EnableIdentifierCaseSensitive), "token identifier is compute");

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        List<ComputeItemToken> transformationTokens = new List<ComputeItemToken>();

        while (true)
        {
            ComputeItemToken computed = TokenizeComputeExpression(lexer, context);

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
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new ComputeToken(transformationTokens);
    }

    /// <summary>
    /// Tokenize compute expression text into a token.
    /// </summary>
    /// <returns>The lexical token representing the compute expression text.</returns>
    internal ComputeItemToken TokenizeComputeExpression(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // expression
        QueryToken expression = TokenizeExpression(lexer, context);

        // "as" alias
        // StringLiteralToken alias = ParseAggregateAs(tokenizer, context);

        // return new ComputeExpressionToken(expression, alias.Text);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Tokenize the 'expand' expression within $apply.
    /// </summary>
    /// <returns></returns>
    protected virtual ExpandToken TokenizeExpand(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        //Debug.Assert(TokenIdentifierIs(TokenConstants.KeywordExpand), "token identifier is expand");

        lexer.NextToken();

        // '('
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_OpenParenExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        List<ExpandItemToken> termTokens = new List<ExpandItemToken>();

        // First token must be Path
        //var termParser = new SelectExpandTermParser(this.lexer, this.maxDepth - 1, false);
        SegmentToken pathToken = null;// Parse(allowRef: true);

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
                        filterToken = TokenizeFilter(lexer, context);
                        break;
                    case TokenConstants.KeywordExpand:
                        ExpandToken tempNestedExpand = TokenizeExpand(lexer, context);
                        nestedExpand = nestedExpand == null
                            ? tempNestedExpand
                            : new ExpandToken(nestedExpand.ExpandItems.Concat(tempNestedExpand.ExpandItems));
                        break;
                    default:
                        throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_KeywordOrIdentifierExpected(supportedKeywords, this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
                }
            }
        }

        // Leaf level expands require filter
        if (filterToken == null && nestedExpand == null)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_InnerMostExpandRequireFilter(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        ExpandItemToken expandTermToken = new ExpandItemToken(pathToken, filterToken, null, null, null, null, null, null, null, nestedExpand);
        termTokens.Add(expandTermToken);

        // ")"
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new QueryParserException("ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        lexer.NextToken();

        return new ExpandToken(termTokens);
    }
}
