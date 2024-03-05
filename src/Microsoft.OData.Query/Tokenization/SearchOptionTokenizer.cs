﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $search query expression and produces the lexical object model.
/// </summary>
public class SearchOptionTokenizer : QueryTokenizer, ISearchOptionTokenizer
{
    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchOptionTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public SearchOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize the $search expression.
    /// </summary>
    /// <param name="search">The $search expression string to tokenize.</param>
    /// <returns>The search token tokenized.</returns>
    public virtual async ValueTask<QueryToken> TokenizeAsync(string search, QueryTokenizerContext context)
    {
        IExpressionLexer lexer = _lexerFactory.CreateLexer(search, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        QueryToken result = TokenizeExpression(lexer, context);

        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return await ValueTask.FromResult(result);
    }

    /// <summary>
    /// Tokenize the or operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override QueryToken TokenizeLogicalOr(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        QueryToken left = TokenizeLogicalAnd(lexer, context);
        while (lexer.IsCurrentTokenIdentifier("or", true))
        {
            lexer.NextToken();
            QueryToken right = TokenizeLogicalAnd(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the and operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override QueryToken TokenizeLogicalAnd(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        QueryToken left = TokenizeUnary(lexer, context);

        while (lexer.IsCurrentTokenIdentifier("and", true)
            || lexer.IsCurrentTokenIdentifier("not", true)
            || lexer.CurrentToken.Kind == ExpressionKind.StringLiteral
            || lexer.CurrentToken.Kind == ExpressionKind.OpenParen)
        {
            // Handle A NOT B, A (B)
            // Bypass only when next token is AND
            if (lexer.IsCurrentTokenIdentifier("and", true))
            {
                lexer.NextToken();
            }

            QueryToken right = TokenizeUnary(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the -, not unary operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override QueryToken TokenizeUnary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        if (lexer.IsCurrentTokenIdentifier("not", true))
        {
            lexer.NextToken();
            QueryToken operand = TokenizeUnary(lexer, context);

            context.LeaveRecurse();
            return new UnaryOperatorToken(UnaryOperatorKind.Not, operand);
        }

        context.LeaveRecurse();
        return TokenizePrimary(lexer, context);
    }

    /// <summary>
    /// Tokenize the primary expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override QueryToken TokenizePrimary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        QueryToken expr;
        context.EnterRecurse();

        switch (lexer.CurrentToken.Kind)
        {
            case ExpressionKind.OpenParen:
                expr = TokenizeParenExpression(lexer, context);
                break;

            case ExpressionKind.StringLiteral:
                expr = new StringLiteralToken(lexer.CurrentToken.Text.ToString());
                lexer.NextToken();
                break;

            default:
                throw new QueryTokenizerException("ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
        }

        context.LeaveRecurse();
        return expr;
    }
}
