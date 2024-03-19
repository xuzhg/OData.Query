//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenizes the $orderby query expression and produces the lexical object model.
/// </summary>
public class OrderByOptionTokenizer : QueryTokenizer, IOrderByOptionTokenizer
{
    internal static OrderByOptionTokenizer Default = new OrderByOptionTokenizer(ExpressionLexerFactory.Default);

    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public OrderByOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize the $orderby expression.
    /// </summary>
    /// <param name="orderBy">The $orderby expression string to Tokenize.</param>
    /// <returns>The order by token tokenized.</returns>
    public virtual async ValueTask<OrderByToken> TokenizeAsync(string orderBy, QueryTokenizerContext context)
    {
        Debug.Assert(orderBy != null, "orderBy != null");

        IExpressionLexer lexer = _lexerFactory.CreateLexer(orderBy, LexerOptions.Default);
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

        return await ValueTask.FromResult(headToken);
    }
}
