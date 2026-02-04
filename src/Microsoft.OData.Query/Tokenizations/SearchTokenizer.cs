//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $search query expression and produces the lexical object model.
/// </summary>
public class SearchTokenizer : QueryTokenizer, ISearchTokenizer
{
    /// <summary>
    /// Tokenizes the $search expression.
    /// </summary>
    /// <param name="search">The $search expression string to tokenize.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The search token tokenized.</returns>
    public virtual async ValueTask<IQueryToken> TokenizeAsync(ReadOnlyMemory<char> search, QueryTokenizerContext context)
    {
        if (search.IsEmpty)
        {
            throw new ArgumentNullException(nameof(search));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = context.CreateLexer(search);
        if (lexer == null)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_FailToCreateLexer, "$search"));
        }

        lexer.NextToken(); // move to first token

        IQueryToken result = TokenizeExpression(lexer, context);

        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return result;
    }

    /// <summary>
    /// Tokenize the or operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override IQueryToken TokenizeLogicalOr(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeLogicalAnd(lexer, context);
        while (lexer.IsCurrentTokenIdentifier("or", true))
        {
            lexer.NextToken();
            IQueryToken right = TokenizeLogicalAnd(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the and operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override IQueryToken TokenizeLogicalAnd(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeUnary(lexer, context);

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

            IQueryToken right = TokenizeUnary(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the -, not unary operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected override IQueryToken TokenizeUnary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        if (lexer.IsCurrentTokenIdentifier("not", true))
        {
            lexer.NextToken();
            IQueryToken operand = TokenizeUnary(lexer, context);

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
    protected override IQueryToken TokenizePrimary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        IQueryToken expr;
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
