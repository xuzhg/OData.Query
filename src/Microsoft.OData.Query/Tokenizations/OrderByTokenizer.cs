//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// Tokenizes the $orderby query expression and produces the lexical object model.
/// </summary>
public class OrderByTokenizer : QueryTokenizer, IOrderByTokenizer
{
    /// <summary>
    /// Tokenizes the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to Tokenize.</param>
    /// <param name="context">The context for tokenizing the $orderby expression.</param>
    /// <returns>The order by token tokenized.</returns>
    public virtual async ValueTask<OrderByToken> TokenizeAsync(ReadOnlyMemory<char> orderBy, QueryTokenizerContext context)
    {
        if (orderBy.IsEmpty)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = context.CreateLexer(orderBy);
        lexer.NextToken(); // move to first token

        OrderByToken headToken = null;
        OrderByToken previousToken = null;
        while (true)
        {
            IQueryToken expression = TokenizeExpression(lexer, context);
            bool ascending = true;
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAscending, false))
            {
                lexer.NextToken();
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordDescending, false))
            {
                lexer.NextToken();
                ascending = false;
            }

            OrderByToken orderByToken = new OrderByToken(expression, ascending ? OrderByDirection.Ascending : OrderByDirection.Descending);
            if (previousToken == null)
            {
                headToken = orderByToken;
            }
            else
            {
                previousToken.ThenBy = orderByToken;
            }

            previousToken = orderByToken;

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return headToken;
    }
}
