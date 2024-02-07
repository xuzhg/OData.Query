//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Parser which consumes the query $orderby expression and produces the lexical object model.
/// </summary>
public class OrderByOptionParser : QueryOptionParser, IOrderByOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public OrderByOptionParser()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByOptionParser" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public OrderByOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }
    /*
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
    }*/

    public virtual OrderByClause ParseOrderBy(OrderByToken orderBy, QueryOptionParserContext context)
    {
        OrderByClause head = null;
        OrderByClause previous = null;

        OrderByToken token = orderBy;

        // Go through the orderby tokens starting from the last one
        while (token != null)
        {
            OrderByClause orderByClause = ProcessSingleOrderBy(token, context);

            if (head == null)
            {
                head = orderByClause;
            }
            else
            {
                previous.ThenBy = orderByClause;
            }

            previous = orderByClause;

            token = token.ThenBy;
        }

        return head;
    }

    /// <summary>
    /// Processes the specified order-by token.
    /// </summary>
    /// <param name="state">State to use for binding.</param>
    /// <param name="thenBy"> The next OrderBy node, or null if there is no orderby after this.</param>
    /// <param name="orderByToken">The order-by token to bind.</param>
    /// <returns>Returns the combined entityCollection including the ordering.</returns>
    private OrderByClause ProcessSingleOrderBy(OrderByToken orderByToken, QueryOptionParserContext context)
    {
       // ExceptionUtils.CheckArgumentNotNull(state, "state");
       // ExceptionUtils.CheckArgumentNotNull(orderByToken, "orderByToken");

        QueryNode expressionNode = Bind(orderByToken.Expression, context);

        //// The order-by expressions need to be primitive / enumeration types
        SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
        //if (expressionResultNode == null ||
        //    (expressionResultNode.TypeReference != null &&
        //    !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind() &&
        //    !expressionResultNode.TypeReference.IsODataEnumTypeKind() &&
        //    !expressionResultNode.TypeReference.IsODataTypeDefinitionTypeKind()))
        //{
        //    throw new ODataException(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
        //}

        OrderByClause orderByNode = new OrderByClause(
            //thenBy,
            expressionResultNode,
            orderByToken.Direction,
            context.ImplicitRangeVariable);

        return orderByNode;
    }
}
