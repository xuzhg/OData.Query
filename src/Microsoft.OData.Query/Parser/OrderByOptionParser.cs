//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $orderby expression and produces the lexical object model.
/// </summary>
public class OrderByOptionParser : QueryOptionParser, IOrderByOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionParser" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public OrderByOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public OrderByToken ParseOrderBy(string orderBy, OrderByOptionParserContext context)
    {
        Debug.Assert(orderBy != null, "orderBy != null");

        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(orderBy, OTokenizerContext.Default);
        tokenizer.NextToken(); // move to first token

        OrderByToken headToken = null;
        OrderByToken previousToken = null;
        while (true)
        {
            QueryToken expression = ParseExpression(tokenizer, context);
            bool ascending = true;
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAscending, false))
            {
                tokenizer.NextToken();
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordDescending, false))
            {
                tokenizer.NextToken();
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

            if (tokenizer.CurrentToken.Kind != OTokenKind.Comma)
            {
                break;
            }

            tokenizer.NextToken();
        }

        tokenizer.ValidateToken(OTokenKind.EndOfInput);

        return headToken;
    }
}
